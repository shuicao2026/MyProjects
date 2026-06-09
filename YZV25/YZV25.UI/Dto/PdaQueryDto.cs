
using System;

namespace YZV25.Dto
{
    /// <summary>
    /// PDA 设备关联查询 DTO
    /// </summary>
    public class PdaQueryDto
    {
        /// <summary>
        /// PDA ID
        /// </summary>

        public int Id { get; set; }

        /// <summary>
        /// PDA 编号
        /// </summary>

        public int No { get; set; }

        /// <summary>
        /// PDA 条码
        /// </summary>
      
        public string Barcode { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>

        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
 
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>

        public string DeviceType { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>

        public string DeviceName { get; set; }

        /// <summary>
        /// 设备标题
        /// </summary>
 
        public string DeviceTitle { get; set; }


        /// <summary>
        /// 条码对应的工作台面
        /// </summary>
        public string Face { get; set; }
    }
}