using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Software.WebApi.AuthHelper;
using Software.Model.ParmModels;
using Senparc.Weixin;
using Software.IServices;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Software.WebApi.Controllers
{
    /// <summary>
    /// 登陆
    /// </summary>
    public class LoginController : ApiBaseController
    {
        private readonly PermissionRequirement _requirement;
        private readonly IMembersServices _membersServices;

        private readonly ILogger<LoginController> _logger;

        public static readonly string WxOpenAppId = Config.SenparcWeixinSetting.WxOpenAppId;//与微信小程序后台的AppId设置保持一致，区分大小写。
        public static readonly string WxOpenAppSecret = Config.SenparcWeixinSetting.WxOpenAppSecret;//与微信小程序账号后台的AppId设置保持一致，区分大小写。
        readonly Func<string> _getRandomFileName = () => SystemTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="requirement"></param>
        /// <param name="membersServices"></param>
        /// <param name="logger"></param>
        public LoginController(PermissionRequirement requirement, IMembersServices membersServices, ILogger<LoginController> logger)
        {
            _requirement = requirement;
            _membersServices = membersServices;
            _logger = logger;
        }

        /// <summary>
        /// 登陆
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "Chuang"),
                    new Claim(ClaimTypes.NameIdentifier, "1000"),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };

            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaims(claims);
            var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
            return new JsonResult(token);
        }

        /// <summary>
        /// 小程序登陆
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        //public async Task<IActionResult> Login(string code,string headurl = "",string nickname = "",string sex = "",string province = "",string city = "")
        public async Task<IActionResult> Post([FromBody] LoginModel loginModel)
        {
            //Todo 这里要去通过code 调取微信接口获取openId
            //https://blog.csdn.net/qq_39851704/article/details/79025557
            //code: res.code,//获取openid的话 需要向后台传递code,利用code请求api获取openid

            _logger.LogDebug($"获取到的Code = {loginModel.code}；WxOpenAppId = {WxOpenAppId}；WxOpenAppSecret = {WxOpenAppSecret} ");

#if DEBUG
            var openId = loginModel.code;
            var session_key = loginModel.code;
            loginModel.nickname = "Software";
            loginModel.headurl = "https://ss0.bdstatic.com/70cFvHSh_Q1YnxGkpoWK1HF6hhy/it/u=1079592840,2327344920&fm=27&gp=0.jpg";
            loginModel.sex = "男";
            loginModel.province = "河南";
            loginModel.city = "郑州";
#else

            var openId = string.Empty;
            var session_key = string.Empty;
            var jsonResult = await SnsApi.JsCode2JsonAsync(WxOpenAppId, WxOpenAppSecret, loginModel.code);
            if (jsonResult.errcode == ReturnCode.请求成功)
            {
                openId = jsonResult.openid;
                session_key = jsonResult.session_key;
            }
#endif

            var member = await _membersServices.Login(openId, session_key, loginModel.headurl, loginModel.nickname, loginModel.sex, loginModel.province, loginModel.city);

            //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, loginModel.code),
                    new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };

            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaims(claims);
            var token = JwtToken.BuildJwtToken(claims.ToArray(), _requirement);
            return new JsonResult(token);
        }

    }
}
