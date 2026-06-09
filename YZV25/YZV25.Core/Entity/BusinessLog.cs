using SqlSugar;
using System;

namespace YZV25.Entity
{
    /// <summary>
    /// 业务日志表
    /// </summary>
    [SugarTable("business_log")]
    public class BusinessLog
    {
        /// <summary>
        /// 主键ID，自增
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// PDA设备编号/标识
        /// </summary>
        [SugarColumn(ColumnName = "pda_no", IsNullable = false)]
        public int PdaNo { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [SugarColumn(ColumnName = "device_name", IsNullable = true)]
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备唯一编号
        /// </summary>
        [SugarColumn(ColumnName = "device_code", IsNullable = true)]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 产品/物料条码
        /// </summary>
        [SugarColumn(ColumnName = "barcode", IsNullable = false)]
        public string Barcode { get; set; }

        /// <summary>
        /// MES操作类型：进站，出站，加工，状态
        /// </summary>
        [SugarColumn(ColumnName = "mes_operation", IsNullable = false)]
        public string MesOperation { get; set; }

        /// <summary>
        /// MES接口返回的原始内容或消息
        /// </summary>
        [SugarColumn(ColumnName = "mes_return_content", IsNullable = true, ColumnDataType = "nvarchar(max)")]
        public string MesReturnContent { get; set; }

        /// <summary>
        /// MES启用状态：1-启用，0-停用
        /// </summary>
        [SugarColumn(ColumnName = "mes_enabled_status", IsNullable = false, DefaultValue = "1")]
        public bool MesEnabledStatus { get; set; }

        /// <summary>
        /// 记录创建时间（行创建时间）
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = false, DefaultValue = "GETDATE()")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 操作时间戳（业务时间）
        /// </summary>
        [SugarColumn(ColumnName = "operation_timestamp", IsNullable = false, DefaultValue = "GETDATE()")]
        public DateTime OperationTimestamp { get; set; }

        /// <summary>
        /// MES返回结果：true-成功，false-失败
        /// </summary>
        [SugarColumn(ColumnName = "mes_result", IsNullable = false, DefaultValue = "0")]
        public bool MesResult { get; set; }

        /// <summary>
        /// 发送的数据
        /// </summary>
        [SugarColumn(ColumnName = "payload",IsNullable = true)]
        public string Payload { get; set; }
    }
}