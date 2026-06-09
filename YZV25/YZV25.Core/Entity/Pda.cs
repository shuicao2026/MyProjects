using SqlSugar;

namespace YZV25.Entity
{
    /// <summary>
    /// pda 设备/数据实体
    /// </summary>
    [SugarTable("pda", "PDA主表")]
    public class Pda
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(
            ColumnName = "id",
            IsPrimaryKey = true,
            IsIdentity = true,
            ColumnDescription = "自增主键"
        )]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(
            ColumnName = "name",
            Length = 100,
            IsNullable = true,
            ColumnDescription = "名称"
        )]
        public string Name { get; set; }

        /// <summary>
        /// 当前条码
        /// </summary>
        [SugarColumn(
            ColumnName = "barcode",
            Length = 100,
            IsNullable = true,
            ColumnDescription = "当前条码"
        )]
        public string Barcode { get; set; }

        /// <summary>
        /// 编号（唯一）
        /// </summary>
        [SugarColumn(
            ColumnName = "no",
            IsNullable = true,
            ColumnDescription = "编号（唯一）"
        )]
        public int? No { get; set; }


        /// <summary>
        /// 当前条码
        /// </summary>
        [SugarColumn(
            ColumnName = "password",
            Length = 100,
            IsNullable = true,
            ColumnDescription = "当前密码"
        )]
        public string Password { get; set; }


        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(
            ColumnName = "update_time",
            ColumnDescription = "更新时间",
            IsNullable = true
        )]
        public DateTime? UpdateTime { get; set; }



        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(
            ColumnName = "face",
            Length = 100,
            IsNullable = true,
            ColumnDescription = "工作台面名称"
        )]
        public string Face { get; set; }


        /// <summary>
        /// 0 - 未知，1-正常，2-屏蔽
        /// </summary>
        [SugarColumn(
            ColumnName = "mes_status",
            IsNullable = true,
            ColumnDescription = "MES状态"
        )]
        public int MesStatus { get; set; }

        }
}