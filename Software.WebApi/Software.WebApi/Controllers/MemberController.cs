using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Software.Common;
using Software.IServices;
using Software.Model.Models;
using Software.Model.ParmModels;
using Software.Model.ViewModels;
using Software.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Software.WebApi.Controllers
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class MemberController : ApiBaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMembersServices _membersServices;
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="membersServices"></param>
        /// <param name="mapper"></param>
        public MemberController(IHostingEnvironment hostingEnvironment, IMembersServices membersServices, IMapper mapper)
        {
            _hostingEnvironment = hostingEnvironment;
            _membersServices = membersServices;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result<object>> Get()
        {
            var userId = this.User.GetUserId();
            Result<object> result = new Result<object>();
            var tmember = await _membersServices.QueryByID(userId);
            var member = _mapper.Map<MemberViewModels>(tmember);
            result.data = new { member};
            return result;
        }

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result<object>> List()
        {
            Result<object> result = new Result<object>();
            var list = await _membersServices.List();
            result.data = list;
            return result;
        }

        /// <summary>
        /// Post异常
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public Result<object> PostException([FromBody] LoginModel loginModel)
        {
            Result<object> result = new Result<object>();
            int j = 0;
            var s = loginModel.city;
            int i = 1 / j;
            return result;
        }

        /// <summary>
        /// Get异常
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public Result<object> GetException(string id, string name)
        {
            Result<object> result = new Result<object>();
            int j = 0;
            int i = 1 / j;
            return result;
        }

        /// <summary>
        /// Post不封装类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public Result<object> PostTest([FromBody]string name,string name1)
        {
            Result<object> result = new Result<object>();
            result.message = name;
            return result;
        }
    }
}
