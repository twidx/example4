using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Example2.Database.Context;
using Example2.Database.Models;
using Example2.Server.Models._Base;
using Example2.Server.Models.Account;
using Example2.Server.Models.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Example2.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class AccountController(WebDbContext db) : Controller
    {
        private readonly WebDbContext db = db;

        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public AccountQuery_VModel Query([FromBody] AccountQuery_PModel model)
        {
            var vm = new AccountQuery_VModel { Success = false };

            try
            {
                var q = db.Account.AsQueryable();

                if (!string.IsNullOrEmpty(model.Account))
                {
                    q = q.Where(x => x.AccountNo.Contains(model.Account));
                }

                vm.Total = q.Count();

                var size = 5;

                vm.Results = [..
                    q.OrderBy(x => x.AccountNo)
                    .Skip((model.Page - 1) * size).Take(size)
                    .Select(x => new AccountItem {
                        AccountNo = x.AccountNo,
                        Name = x.Name
                    })
                ];
                vm.Success = true;
            }
            catch (Exception ex)
            {
                vm.Message = ex.Message;
            }

            return vm;
        }

        /// <summary>
        /// 新增帳號
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public Base_VModel New([FromBody] AccountSave_PModel model)
        {
            var vm = new Base_VModel { Success = false };

            try
            {
                if (model.Item == null) throw new Exception("參數異常！");
                if (string.IsNullOrEmpty(model.Item.AccountNo)) throw new Exception("帳號不可空白！");
                if (string.IsNullOrEmpty(model.Item.Name)) throw new Exception("名稱不可空白！");
                if (string.IsNullOrEmpty(model.Item.Password)) throw new Exception("密碼不可空白！");
                if (db.Account.Where(x => x.AccountNo == model.Item.AccountNo).Any()) throw new Exception("帳號已存在！");

                db.Account.Add(new Account
                {
                    AccountNo = model.Item.AccountNo,
                    Name = model.Item.Name,
                    Password = model.Item.Password
                });

                db.SaveChanges();

                vm.Success = true;
            }
            catch (Exception ex)
            {
                vm.Message = ex.Message;
            }

            return vm;
        }

        /// <summary>
        /// 帳號修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public Base_VModel Save([FromBody] AccountSave_PModel model)
        {
            var vm = new Base_VModel { Success = false };

            try
            {
                if (model.Item == null) throw new Exception("參數異常！");
                if (string.IsNullOrEmpty(model.Item.AccountNo)) throw new Exception("帳號不可空白！");
                if (string.IsNullOrEmpty(model.Item.Name)) throw new Exception("名稱不可空白！");

                var item = db.Account.Where(x => x.AccountNo == model.Item.AccountNo).FirstOrDefault() ?? throw new Exception("帳號不存在！");
                item.Name = model.Item.Name;

                if (!string.IsNullOrEmpty(model.Item.Password)) item.Password = model.Item.Password;

                db.SaveChanges();

                vm.Success = true;
            }
            catch (Exception ex)
            {
                vm.Message = ex.Message;
            }

            return vm;
        }

        /// <summary>
        /// 帳號刪除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public Base_VModel Remove([FromBody] AccountRemove_PModel model)
        {
            var vm = new Base_VModel { Success = false };

            try
            {
                if (string.IsNullOrEmpty(model.AccountNo)) throw new Exception("參數異常！");

                var item = db.Account.Where(x => x.AccountNo == model.AccountNo).FirstOrDefault() ?? throw new Exception("帳號不存在！");

                db.Account.Remove(item);
                db.SaveChanges();
                vm.Success = true;
            }
            catch (Exception ex)
            {
                vm.Message = ex.Message;
            }

            return vm;
        }

        /// <summary>
        /// 匯出Excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Excel([FromBody] AccountExcel_PModel model)
        {
            var vm = new Base_VModel { Success = false };

            try
            {
                var q = db.Account.AsQueryable();

                if (!string.IsNullOrEmpty(model.Account))
                {
                    q = q.Where(x => x.AccountNo.Contains(model.Account));
                }

                var vList = q.OrderBy(x => x.AccountNo).Select(x => new AccountItem
                {
                    AccountNo = x.AccountNo,
                    Name = x.Name
                }).ToList();

                var header = new string[] { "帳號", "名稱" };

                var ms = new MemoryStream();

                // Create a spreadsheet document by supplying the filepath.
                // By default, AutoSave = true, Editable = true, and Type = xlsx.
                var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);

                // Add a WorkbookPart to the document.
                var wbp = doc.AddWorkbookPart();
                wbp.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                var wsp = wbp.AddNewPart<WorksheetPart>();
                wsp.Worksheet = new Worksheet(new SheetData());

                // Add Sheets to the Workbook.
                var sheets = wbp.Workbook.AppendChild(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                var sheet = new Sheet() { Id = wbp.GetIdOfPart(wsp), SheetId = 1, Name = "mySheet" };
                sheets.Append(sheet);

                var sheetData = wsp.Worksheet.GetFirstChild<SheetData>();

                int rowNo = 0;
                var row = new Row();

                for (int i = 0; i < header.Length; i++)
                {
                    row.InsertAt(new Cell()
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(header[i])
                    }, i);
                }

                sheetData?.InsertAt(row, rowNo++);

                vList.ForEach(item =>
                {
                    row = new Row();

                    if (!string.IsNullOrEmpty(item.AccountNo))
                    {
                        row.InsertAt<Cell>(new Cell()
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(item.AccountNo)
                        }, 0);
                    }

                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        row.InsertAt<Cell>(new Cell()
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(item.Name)
                        }, 1);
                    }

                    sheetData?.InsertAt(row, rowNo++);
                });

                wbp.Workbook.Save();

                // Dispose the document.
                doc.Dispose();

                var file = ms.ToArray();

                return File(file, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                vm.Message = ex.Message;
            }

            return Json(vm);
        }
    }
}
