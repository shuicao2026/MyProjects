using SqlSugar;

namespace YZV25.Core.Dto
{
    public class BusinessLogDto
    {
        /// <summary>
        /// 主键ID，自增
        /// </summary>

        public int Id { get; set; }

        /// <summary>
        /// PDA设备编号/标识
        /// </summary>

        public int PdaNo { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>

        public string DeviceName { get; set; }

        /// <summary>
        /// 设备唯一编号
        /// </summary>

        public string DeviceCode { get; set; }

        /// <summary>
        /// 产品/物料条码
        /// </summary>

        public string Barcode { get; set; }

        /// <summary>
        /// MES操作类型：进站，出站，加工，状态
        /// </summary>

        public string MesOperation { get; set; }

        /// <summary>
        /// MES接口返回的原始内容或消息
        /// </summary>

        public string MesReturnContent { get; set; }

        /// <summary>
        /// MES启用状态：1-启用，0-停用
        /// </summary>

        public bool MesEnabledStatus { get; set; }

        /// <summary>
        /// 记录创建时间（行创建时间）
        /// </summary>
   
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 操作时间戳（业务时间）
        /// </summary>

        public DateTime OperationTimestamp { get; set; }

        /// <summary>
        /// MES返回结果：true-成功，false-失败
        /// </summary>

        public bool MesResult { get; set; }

        /// <summary>
        /// 发送的数据
        /// </summary>

        public string Payload { get; set; }
    }
}
