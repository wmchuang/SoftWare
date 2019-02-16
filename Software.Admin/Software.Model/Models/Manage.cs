using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Software.Model.Models
{
    /// <summary>
    /// Manage
    /// </summary>	
    public class Manage//可以在这里加上基类等
    {

        /// <summary>
        ///    Id
        /// </summary>	
        public int Id { get; set; }

        /// <summary>
        ///    Name
        /// </summary>	
        public string Name { get; set; }

        /// <summary>
        ///    RoleId
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        ///    Password
        /// </summary>	
        public string Password { get; set; }

        /// <summary>
        ///    CreateTime
        /// </summary>	
        public DateTime? CreateTime { get; set; }

    }
}