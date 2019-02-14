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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Software.WebApi.Controllers
{
    /// <summary>
    /// 登陆
    /// </summary>
    public class LoginController : ApiBaseController
    {
        private readonly PermissionRequirement _requirement;

        private readonly ILogger<LoginController> _logger;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="requirement"></param>
        /// <param name="logger"></param>
        public LoginController(PermissionRequirement requirement, ILogger<LoginController> logger)
        {
            _requirement = requirement;
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

    }
}
