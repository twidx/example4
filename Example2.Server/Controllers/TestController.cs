using Example2.Database.Context;
using Microsoft.AspNetCore.Mvc;

namespace Example2.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TestController(WebDbContext db) : Controller
    {
        private readonly WebDbContext db = db;

        [HttpPost]
        public IActionResult Join()
        {
            
            var vList = db.TableA.Join(db.TableB, a => a.Code, b => b.Code, (a, b) => new
            {
                a.Code,
                a.Name,
                b.Val
            }).ToList();

            return Json(vList);
        }

        [HttpPost]
        public IActionResult LeftJoin()
        {
            var vList = db.TableA
                .GroupJoin(db.TableB, a => a.Code, b => b.Code, (a, b) => new { a, b })
                .SelectMany(x => x.b.DefaultIfEmpty(), (x, b) => new { x.a.Code, x.a.Name, b.Val }).ToList();

            return Json(vList);
        }

        [HttpGet]
        public IActionResult DapperTest(string? Code)
        {
            var sql = @"
                SELECT a.Code, a.Name, b.Val
                FROM TableA a
                    LEFT JOIN TableB b ON a.Code = b.Code
            ";

            if (!string.IsNullOrEmpty(Code))
            {
                sql += "WHERE a.Code = @Code";
            }

            var vList = db.Query<dynamic>(sql, new { Code = Code }).ToList();

            return Json(vList);
        }
    }
}
