using System;
using System.Collections.Generic;
using System.Text;

namespace Software.Model.ViewModels
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class MemberViewModels
    {
        /// <summary>
        /// 昵称
        /// </summary>	
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>	
        public string HeadImg { get; set; }

        /// <summary>
        /// 性别；0：男 1:女
        /// </summary>	
        public int? Sex { get; set; }
    }
}
