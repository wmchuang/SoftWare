
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Software.WebApi.Extensions
{
    /// <summary>
    /// 扩展方法获取userId
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// 获取用户userId
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return Convert.ToInt32(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
