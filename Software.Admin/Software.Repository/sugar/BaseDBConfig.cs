
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Software.Repository
{
    public class BaseDBConfig
    {
#if DEBUG
        public static string ConnectionString = "server=localhost;uid=sa;pwd=123456;database=Software";
#else
        public static string ConnectionString = "server=localhost;uid=sa;pwd=123456;database=Software";
#endif

    }
}
