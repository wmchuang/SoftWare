using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Software.IServices;
using Software.Model.Models;
using Software.Model.ParmModels;
using Software.Model.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Software.Admin.Controllers
{
    [Authorize]
    public class NLogController : Controller
    {
        private readonly INLogInfoServices _nLogInfoServices;

        public NLogController(INLogInfoServices nLogInfoServices)
        {
            _nLogInfoServices = nLogInfoServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult LoadData([FromQuery]PageModel model)
        {
            List<NLogInfo> list = new List<NLogInfo>();
            int totalCount = 0;
            Expression<Func<NLogInfo, bool>> whereExpression = null;

            if (!string.IsNullOrWhiteSpace(model.Key))
            {
                whereExpression = a => a.Message.Contains(model.Key);
            }
            list = _nLogInfoServices.QueryPage(ref totalCount, whereExpression, model.Page, model.Limit, "Id Desc");
            var result = new TableDataModel
            {
                count = totalCount,
                data = list
            };
            return Json(result);
        }

        [HttpPost]
        public async Task<Result<string>> Delete(int[] Id)
        {
            var result = new Result<string>();

            var flag = await _nLogInfoServices.DeleteByIds(Id);
            if (!flag)
            {
                result.status = false;
                result.message = "删除失败";
            }
            return result;
        }
    }
}
