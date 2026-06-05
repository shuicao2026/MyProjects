namespace YZV25UI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            dgvRuningTimeLogs = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colLevel = new DataGridViewTextBoxColumn();
            colMessage = new DataGridViewTextBoxColumn();
            colTimeStamp = new DataGridViewTextBoxColumn();
            tabPage2 = new TabPage();
            splitContainer1 = new SplitContainer();
            panel2 = new Panel();
            dgvLog = new DataGridView();
            colId2 = new DataGridViewTextBoxColumn();
            colLevel2 = new DataGridViewTextBoxColumn();
            colMessage2 = new DataGridViewTextBoxColumn();
            colTimeStamp2 = new DataGridViewTextBoxColumn();
            panel1 = new Panel();
            btnQuery = new Button();
            label1 = new Label();
            tbText = new TextBox();
            rtbContent = new RichTextBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRuningTimeLogs).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1184, 708);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgvRuningTimeLogs);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1176, 678);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "实时日志";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvRuningTimeLogs
            // 
            dgvRuningTimeLogs.AllowUserToAddRows = false;
            dgvRuningTimeLogs.AllowUserToDeleteRows = false;
            dgvRuningTimeLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRuningTimeLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRuningTimeLogs.Columns.AddRange(new DataGridViewColumn[] { colId, colLevel, colMessage, colTimeStamp });
            dgvRuningTimeLogs.Dock = DockStyle.Fill;
            dgvRuningTimeLogs.Location = new Point(3, 3);
            dgvRuningTimeLogs.Name = "dgvRuningTimeLogs";
            dgvRuningTimeLogs.ReadOnly = true;
            dgvRuningTimeLogs.Size = new Size(1170, 672);
            dgvRuningTimeLogs.TabIndex = 0;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.HeaderText = "序号";
            colId.Name = "colId";
            colId.ReadOnly = true;
            // 
            // colLevel
            // 
            colLevel.DataPropertyName = "Level";
            colLevel.HeaderText = "日志级别";
            colLevel.Name = "colLevel";
            colLevel.ReadOnly = true;
            // 
            // colMessage
            // 
            colMessage.DataPropertyName = "Message";
            colMessage.HeaderText = "日志信息";
            colMessage.Name = "colMessage";
            colMessage.ReadOnly = true;
            // 
            // colTimeStamp
            // 
            colTimeStamp.DataPropertyName = "TimeStamp";
            colTimeStamp.HeaderText = "记入时间";
            colTimeStamp.Name = "colTimeStamp";
            colTimeStamp.ReadOnly = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(splitContainer1);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1176, 678);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "日志查询";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panel2);
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(rtbContent);
            splitContainer1.Size = new Size(1170, 672);
            splitContainer1.SplitterDistance = 738;
            splitContainer1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(dgvLog);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 52);
            panel2.Name = "panel2";
            panel2.Size = new Size(738, 620);
            panel2.TabIndex = 1;
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
            dgvLog.Size = new Size(738, 620);
            dgvLog.TabIndex = 0;
            dgvLog.Scroll += dgvLog_Scroll;
            dgvLog.SelectionChanged += dgvLog_SelectionChanged;
            // 
            // colId2
            // 
            colId2.DataPropertyName = "Id";
            colId2.HeaderText = "序号";
            colId2.Name = "colId2";
            colId2.ReadOnly = true;
            // 
            // colLevel2
            // 
            colLevel2.DataPropertyName = "Level";
            colLevel2.HeaderText = "日志级别";
            colLevel2.Name = "colLevel2";
            colLevel2.ReadOnly = true;
            // 
            // colMessage2
            // 
            colMessage2.DataPropertyName = "Message";
            colMessage2.HeaderText = "日志消息";
            colMessage2.Name = "colMessage2";
            colMessage2.ReadOnly = true;
            // 
            // colTimeStamp2
            // 
            colTimeStamp2.DataPropertyName = "Timestamp";
            colTimeStamp2.HeaderText = "记入时间";
            colTimeStamp2.Name = "colTimeStamp2";
            colTimeStamp2.ReadOnly = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnQuery);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(tbText);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(738, 52);
            panel1.TabIndex = 0;
            // 
            // btnQuery
            // 
            btnQuery.Location = new Point(372, 14);
            btnQuery.Name = "btnQuery";
            btnQuery.Size = new Size(75, 23);
            btnQuery.TabIndex = 2;
            btnQuery.Text = "查询";
            btnQuery.UseVisualStyleBackColor = true;
            btnQuery.Click += btnQuery_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 17);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 1;
            label1.Text = "查询内容";
            // 
            // tbText
            // 
            tbText.Location = new Point(89, 14);
            tbText.Name = "tbText";
            tbText.Size = new Size(265, 23);
            tbText.TabIndex = 0;
            // 
            // rtbContent
            // 
            rtbContent.Dock = DockStyle.Fill;
            rtbContent.Location = new Point(0, 0);
            rtbContent.Name = "rtbContent";
            rtbContent.ReadOnly = true;
            rtbContent.Size = new Size(428, 672);
            rtbContent.TabIndex = 0;
            rtbContent.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 708);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRuningTimeLogs).EndInit();
            tabPage2.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLog).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private DataGridView dgvRuningTimeLogs;
        private TabPage tabPage2;
        private SplitContainer splitContainer1;
        private Panel panel2;
        private DataGridView dgvLog;
        private Panel panel1;
        private Button btnQuery;
        private Label label1;
        private TextBox tbText;
        private RichTextBox rtbContent;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colLevel;
        private DataGridViewTextBoxColumn colMessage;
        private DataGridViewTextBoxColumn colTimeStamp;
        private DataGridViewTextBoxColumn colId2;
        private DataGridViewTextBoxColumn colLevel2;
        private DataGridViewTextBoxColumn colMessage2;
        private DataGridViewTextBoxColumn colTimeStamp2;
    }
}
