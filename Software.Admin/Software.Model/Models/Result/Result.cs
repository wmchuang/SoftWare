using System;
using System.Collections.Generic;
using System.Text;

namespace Software.Model.Models
{
    /// <summary>
    /// 服务器请求结果
    /// </summary>
    public interface IResult
    {
        bool status { get; set; }
        string message { get; set; }
    }

    public class ResultBase : IResult
    {
        public ResultBase()
        {
            status = true;
            message = "成功";
        }

        /// <summary>
        /// 状态(是否成功)
        /// </summary>
        public bool status { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string message { get; set; }
    }

    public class Result<T> : ResultBase
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }
    }

    public class ResultList
    {
        /// <summary>
        /// 总数量
        /// </summary>
        public int totalCount { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<object> list { get; set; }
    }
   
}
