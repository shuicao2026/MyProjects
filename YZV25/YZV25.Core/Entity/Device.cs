using SqlSugar;
using System.ComponentModel;

namespace YZV25.Entity
{
    [SugarTable("device", "dbo")]
    public class Device
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// pda编号
        /// </summary>
        [SugarColumn(ColumnName = "pda_no")]
        [Description("pda编号")]
        public int PdaNo { get; set; }

        /// <summary>
        /// 设备名称 用于在网关上显示
        /// </summary>
        [SugarColumn(ColumnName = "name")]
        [Description("设备名称 用于在网关上显示")]
        public string Name { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [SugarColumn(ColumnName = "title", IsNullable = true)]
        [Description("设备名称")]
        public string? Title { get; set; }

        /// <summary>
        /// 设备唯一编码
        /// </summary>
        [SugarColumn(ColumnName = "device_code")]
        [Description("设备唯一编码")]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [SugarColumn(ColumnName = "device_type")]
        [Description("设备类型")]
        public string DeviceType { get; set; }

        /// <summary>
        /// 所在车间
        /// </summary>
        [SugarColumn(ColumnName = "workshop", IsNullable = true)]
        [Description("所在车间")]
        public string? Workshop { get; set; }

        /// <summary>
        /// 启动命令地址点 对应夹具的S7地址点
        /// </summary>
        [SugarColumn(ColumnName = "start_signal_point", IsNullable = true)]
        [Description("启动命令地址点 对应夹具的S7地址点")]
        public string? StartSignalPoint { get; set; }

        /// <summary>
        /// 状态信号点
        /// </summary>
        [SugarColumn(ColumnName = "status_signal_point", IsNullable = true)]
        [Description("状态信号点")]
        public string? StatusSignalPoint { get; set; }

        /// <summary>
        /// 完成监听信号点
        /// </summary>
        [SugarColumn(ColumnName = "finish_signal_point", IsNullable = true)]
        [Description("完成监听信号点")]
        public string? FinishSignalPoint { get; set; }

        /// <summary>
        /// 工件面位 CNC 为 正 反
        /// </summary>
        [SugarColumn(ColumnName = "face", IsNullable = true)]
        [Description("工件面位 CNC 为 正 反")]
        public string? Face { get; set; }

        /// <summary>
        /// 屏蔽夹具信号 0 不屏蔽 1 屏蔽
        /// 则不发送夹具加紧信号，以及不监听夹具加工完成信号
        /// </summary>
        [SugarColumn(ColumnName = "mask_signal", IsNullable = true)]
        [Description("屏蔽夹具信号 0 不屏蔽 1 屏蔽")]
        public int? MaskSignal { get; set; } = 0; // 根据文档，默认值为0
    }
}