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
            SuspendLayout();
            // 
            // tbLog
            // 
            tbLog.AutoSizeColumnsMode = AntdUI.ColumnsMode.Fill;
            tbLog.Dock = DockStyle.Fill;
            tbLog.Gap = 12;
            tbLog.Location = new Point(0, 0);
            tbLog.Name = "tbLog";
            tbLog.Size = new Size(563, 476);
            tbLog.TabIndex = 0;
            tbLog.Text = "table1";
            // 
            // RealTimeLogUCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tbLog);
            Name = "RealTimeLogUCtrl";
            Size = new Size(563, 476);
            Load += RealTimeLogUCtrl_Load;
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Table tbLog;
    }
}
