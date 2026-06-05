namespace YZV25.UI.UserControls
{
    partial class LogQueryUCtrl
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
            panel1 = new AntdUI.Panel();
            rtbContent = new RichTextBox();
            panel2 = new AntdUI.Panel();
            btnQuery = new AntdUI.Button();
            tbText = new AntdUI.Input();
            panel3 = new AntdUI.Panel();
            dgvLog = new DataGridView();
            colId2 = new DataGridViewTextBoxColumn();
            colLevel2 = new DataGridViewTextBoxColumn();
            colMessage2 = new DataGridViewTextBoxColumn();
            colTimeStamp2 = new DataGridViewTextBoxColumn();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(rtbContent);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(525, 0);
            panel1.MaximumSize = new Size(480, 0);
            panel1.MinimumSize = new Size(480, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(480, 616);
            panel1.TabIndex = 0;
            panel1.Text = "panel1";
            // 
            // rtbContent
            // 
            rtbContent.Dock = DockStyle.Fill;
            rtbContent.Location = new Point(0, 0);
            rtbContent.Name = "rtbContent";
            rtbContent.Size = new Size(480, 616);
            rtbContent.TabIndex = 0;
            rtbContent.Text = "";
            // 
            // panel2
            // 
            panel2.Controls.Add(btnQuery);
            panel2.Controls.Add(tbText);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(525, 52);
            panel2.TabIndex = 1;
            panel2.Text = "panel2";
            // 
            // btnQuery
            // 
            btnQuery.BorderWidth = 1F;
            btnQuery.JoinMode = AntdUI.TJoinMode.Right;
            btnQuery.Location = new Point(203, 14);
            btnQuery.Name = "btnQuery";
            btnQuery.Size = new Size(62, 32);
            btnQuery.TabIndex = 2;
            btnQuery.Text = "查询";
            btnQuery.Click += btnQuery_Click;
            // 
            // tbText
            // 
            tbText.JoinMode = AntdUI.TJoinMode.Left;
            tbText.Location = new Point(14, 14);
            tbText.Name = "tbText";
            tbText.PlaceholderText = "请输入查询内容";
            tbText.Radius = 0;
            tbText.Size = new Size(192, 32);
            tbText.TabIndex = 1;
            // 
            // panel3
            // 
            panel3.Controls.Add(dgvLog);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 52);
            panel3.Name = "panel3";
            panel3.Size = new Size(525, 564);
            panel3.TabIndex = 2;
            panel3.Text = "panel3";
            // 
            // dgvLog
            // 
            dgvLog.AllowUserToAddRows = false;
            dgvLog.AllowUserToDeleteRows = false;
            dgvLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLog.Columns.AddRange(new DataGridViewColumn[] { colId2, colLevel2, colMessage2, colTimeStamp2 });
            dgvLog.Dock = DockStyle.Fill;
            dgvLog.Location = new Point(0, 0);
            dgvLog.MultiSelect = false;
            dgvLog.Name = "dgvLog";
            dgvLog.ReadOnly = true;
            dgvLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLog.Size = new Size(525, 564);
            dgvLog.TabIndex = 1;
            dgvLog.Scroll += dgvLog_Scroll;
            dgvLog.SelectionChanged += dgvLog_SelectionChanged;
            // 
            // colId2
            // 
            colId2.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            colId2.DataPropertyName = "Id";
            colId2.FillWeight = 79.18781F;
            colId2.HeaderText = "序号";
            colId2.Name = "colId2";
            colId2.ReadOnly = true;
            colId2.Width = 57;
            // 
            // colLevel2
            // 
            colLevel2.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            colLevel2.DataPropertyName = "Level";
            colLevel2.FillWeight = 162.436554F;
            colLevel2.HeaderText = "日志级别";
            colLevel2.Name = "colLevel2";
            colLevel2.ReadOnly = true;
            colLevel2.Width = 81;
            // 
            // colMessage2
            // 
            colMessage2.DataPropertyName = "Message";
            colMessage2.FillWeight = 79.18781F;
            colMessage2.HeaderText = "日志消息";
            colMessage2.Name = "colMessage2";
            colMessage2.ReadOnly = true;
            // 
            // colTimeStamp2
            // 
            colTimeStamp2.DataPropertyName = "Timestamp";
            colTimeStamp2.FillWeight = 79.18781F;
            colTimeStamp2.HeaderText = "记入时间";
            colTimeStamp2.Name = "colTimeStamp2";
            colTimeStamp2.ReadOnly = true;
            // 
            // LogQueryUCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "LogQueryUCtrl";
            Size = new Size(1005, 616);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLog).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Panel panel2;
        private AntdUI.Panel panel3;
        private RichTextBox rtbContent;
        private AntdUI.Label label1;
        private AntdUI.Button btnQuery;
        private AntdUI.Input tbText;
        private DataGridView dgvLog;
        private DataGridViewTextBoxColumn colId2;
        private DataGridViewTextBoxColumn colLevel2;
        private DataGridViewTextBoxColumn colMessage2;
        private DataGridViewTextBoxColumn colTimeStamp2;
    }
}
