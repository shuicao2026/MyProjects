namespace YZV25.UI
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            AntdUI.TagTabItem tagTabItem5 = new AntdUI.TagTabItem();
            AntdUI.TagTabItem tagTabItem6 = new AntdUI.TagTabItem();
            AntdUI.TagTabItem tagTabItem7 = new AntdUI.TagTabItem();
            AntdUI.TagTabItem tagTabItem8 = new AntdUI.TagTabItem();
            tabHeader1 = new AntdUI.TabHeader();
            plMain = new AntdUI.Panel();
            SuspendLayout();
            // 
            // tabHeader1
            // 
            tabHeader1.AccessibleRole = AccessibleRole.TitleBar;
            tabHeader1.Dock = DockStyle.Top;
            tabHeader1.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            tagTabItem5.ShowClose = false;
            tagTabItem5.Tag = null;
            tagTabItem5.Text = "PDA显示";
            tagTabItem6.ShowClose = false;
            tagTabItem6.Tag = null;
            tagTabItem6.Text = "实时日志";
            tagTabItem7.ShowClose = false;
            tagTabItem7.Tag = null;
            tagTabItem7.Text = "日志查询";
            tagTabItem8.ShowClose = false;
            tagTabItem8.Tag = null;
            tagTabItem8.Text = "MES配置";
            tabHeader1.Items.Add(tagTabItem5);
            tabHeader1.Items.Add(tagTabItem6);
            tabHeader1.Items.Add(tagTabItem7);
            tabHeader1.Items.Add(tagTabItem8);
            tabHeader1.LeftGap = 50;
            tabHeader1.Location = new Point(0, 0);
            tabHeader1.Name = "tabHeader1";
            tabHeader1.ShowButton = true;
            tabHeader1.ShowIcon = true;
            tabHeader1.Size = new Size(1121, 32);
            tabHeader1.TabIndex = 0;
            tabHeader1.Text = "V25数据采集";
            tabHeader1.TabChanged += tabHeader1_TabChanged;
            // 
            // plMain
            // 
            plMain.Dock = DockStyle.Fill;
            plMain.Location = new Point(0, 32);
            plMain.Name = "plMain";
            plMain.Size = new Size(1121, 615);
            plMain.TabIndex = 1;
            plMain.Text = "panel1";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1121, 647);
            Controls.Add(plMain);
            Controls.Add(tabHeader1);
            Name = "MainWindow";
            Text = "MainWindow";
            Load += MainWindow_Load;
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.TabHeader tabHeader1;
        private AntdUI.Panel plMain;
    }
}