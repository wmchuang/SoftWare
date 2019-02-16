using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Software.Common.Helper;
using Software.IServices;
using Software.Model.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Software.Admin.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly IManageServices _manageServices;

        public ManageController(IManageServices manageServices)
        {
            _manageServices = manageServices;
        }


        public IActionResult ChangePwd()
        {
            return View();
        }

        [HttpPost]
        public async Task<Result<string>> ChangePwd(string oldPwd, string newPwd)
        {
            Result<string> result = new Result<string>();
            var member = await _manageServices.QueryByID(1);

            if (member.Password != AESEncryptHelper.Encode(oldPwd))
            {
                result.status = false;
                result.message = "旧密码错误";
                return result;
            }
            member.Password = AESEncryptHelper.Encode(newPwd);

            result.message = "修改密码成功！";
            await _manageServices.Update(member);

            return result;
        }
    }
}
