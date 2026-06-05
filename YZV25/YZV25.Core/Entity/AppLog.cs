using SqlSugar;
using System;

namespace YZV25.Entity
{
    /// <summary>
    /// 应用日志表
    /// </summary>
    [SugarTable("AppLogs", "应用日志表")]
    public class AppLog
    {
        /// <summary>
        /// 自增主键ID（起始值1001）
        /// </summary>
        [SugarColumn(
            ColumnName = "Id",
            IsPrimaryKey = true,
            IsIdentity = true,
            ColumnDescription = "自增主键"
        )]
        public int Id { get; set; }

        /// <summary>
        /// 日志消息内容
        /// </summary>
        [SugarColumn(
            ColumnName = "Message",
            ColumnDataType = "nvarchar(max)",
            ColumnDescription = "日志消息内容",
            IsNullable = true
        )]
        public string Message { get; set; }



        /// <summary>
        /// 日志级别（如：Info/Warn/Error）
        /// </summary>
        [SugarColumn(
            ColumnName = "Level",
            Length = 16,
            ColumnDescription = "日志级别",
            IsNullable = true
        )]
        public string Level { get; set; }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        [SugarColumn(
            ColumnName = "TimeStamp",
            ColumnDescription = "日志记录时间",
            IsNullable = true
        )]
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        [SugarColumn(
            ColumnName = "Exception",
            ColumnDataType = "nvarchar(max)",
            ColumnDescription = "异常堆栈信息",
            IsNullable = true
        )]
        public string Exception { get; set; }


    }
}