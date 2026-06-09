namespace YZV25.UI.UserControls
{
    partial class RealTimeLogUCtrl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            tbLog = new AntdUI.Table();
            panel1 = new AntdUI.Panel();
            label1 = new AntdUI.Label();
            btnQuery = new AntdUI.Button();
            tbText = new AntdUI.Input();
            switch1 = new AntdUI.Switch();
            panel2 = new AntdUI.Panel();
            pagination1 = new AntdUI.Pagination();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tbLog
            // 
            tbLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbLog.AutoSizeColumnsMode = AntdUI.ColumnsMode.Fill;
            tbLog.Gap = 12;
            tbLog.Location = new Point(0, 41);
            tbLog.Name = "tbLog";
            tbLog.Size = new Size(776, 389);
            tbLog.TabIndex = 0;
            tbLog.Text = "table1";
            tbLog.CellClick += tbLog_CellClick;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnQuery);
            panel1.Controls.Add(tbText);
            panel1.Controls.Add(switch1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(776, 39);
            panel1.TabIndex = 1;
            panel1.Text = "panel1";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label1.Location = new Point(661, 8);
            label1.Name = "label1";
            label1.Size = new Size(55, 23);
            label1.TabIndex = 5;
            label1.Text = "实时查询";
            // 
            // btnQuery
            // 
            btnQuery.BorderWidth = 1F;
            btnQuery.JoinMode = AntdUI.TJoinMode.Right;
            btnQuery.Location = new Point(207, 3);
            btnQuery.Name = "btnQuery";
            btnQuery.Size = new Size(62, 32);
            btnQuery.TabIndex = 4;
            btnQuery.Text = "查询";
            btnQuery.Click += btnQuery_Click_2;
            // 
            // tbText
            // 
            tbText.JoinMode = AntdUI.TJoinMode.Left;
            tbText.Location = new Point(18, 3);
            tbText.Name = "tbText";
            tbText.PlaceholderText = "请输入查询内容";
            tbText.Radius = 0;
            tbText.Size = new Size(192, 32);
            tbText.TabIndex = 3;
            // 
            // switch1
            // 
            switch1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            switch1.CheckedText = "开";
            switch1.Location = new Point(722, 8);
            switch1.Name = "switch1";
            switch1.Size = new Size(37, 23);
            switch1.TabIndex = 0;
            switch1.Text = "switch1";
            switch1.UnCheckedText = "关";
            switch1.CheckedChanged += switch1_CheckedChanged;
            // 
            // panel2
            // 
            panel2.Controls.Add(pagination1);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 436);
            panel2.Name = "panel2";
            panel2.Size = new Size(776, 40);
            panel2.TabIndex = 3;
            panel2.Text = "panel2";
            // 
            // pagination1
            // 
            pagination1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pagination1.Location = new Point(499, 10);
            pagination1.Name = "pagination1";
            pagination1.Size = new Size(255, 23);
            pagination1.TabIndex = 0;
            pagination1.Text = "pagination1";
            pagination1.ValueChanged += pagination1_ValueChanged;
            // 
            // RealTimeLogUCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(tbLog);
            Name = "RealTimeLogUCtrl";
            Size = new Size(776, 476);
            Load += RealTimeLogUCtrl_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Table tbLog;
        private AntdUI.Panel panel1;
        private AntdUI.Switch switch1;
        private AntdUI.Label label1;
        private AntdUI.Button btnQuery;
        private AntdUI.Input tbText;

        private AntdUI.Panel panel2;
        private AntdUI.Pagination pagination1;
    }
}
