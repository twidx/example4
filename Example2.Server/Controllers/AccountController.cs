using Example2.Database.Context;
using Example2.Database.Models;
using Example2.Server.Models._Base;
using Example2.Server.Models.Account;
using Example2.Server.Models.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public AccountQuery_VModel Query()
        {
            var vm = new AccountQuery_VModel { Success = false };

            try
            {
                vm.Results = [..
                    db.Account.OrderBy(x => x.AccountNo).Select(x => new AccountItem {
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
    }
}
