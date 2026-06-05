
using System;

namespace YZV25UI.Dto
{

    public class LogDto
    {

        public int Id { get; set; }


        public string Message { get; set; }


        /// <summary>
        /// 日志级别（如：Info/Warn/Error）
        /// </summary>

        public string Level { get; set; }

        /// <summary>
        /// 日志记录时间
        /// </summary>

        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// 异常堆栈信息
        /// </summary>

        public string Exception { get; set; }

    }
}