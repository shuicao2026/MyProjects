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
            AntdUI.TagTabItem tagTabItem1 = new AntdUI.TagTabItem();
            AntdUI.TagTabItem tagTabItem2 = new AntdUI.TagTabItem();
            AntdUI.TagTabItem tagTabItem3 = new AntdUI.TagTabItem();
            AntdUI.TagTabItem tagTabItem4 = new AntdUI.TagTabItem();
            tabHeader1 = new AntdUI.TabHeader();
            plMain = new AntdUI.Panel();
            SuspendLayout();
            // 
            // tabHeader1
            // 
            tabHeader1.AccessibleRole = AccessibleRole.TitleBar;
            tabHeader1.Dock = DockStyle.Top;
            tabHeader1.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            tagTabItem1.ShowClose = false;
            tagTabItem1.Tag = null;
            tagTabItem1.Text = "PDA显示";
            tagTabItem2.ShowClose = false;
            tagTabItem2.Tag = null;
            tagTabItem2.Text = "实时日志";
            tagTabItem3.ShowClose = false;
            tagTabItem3.Tag = null;
            tagTabItem3.Text = "日志查询";
            tagTabItem4.ShowClose = false;
            tagTabItem4.Tag = null;
            tagTabItem4.Text = "MES配置";
            tabHeader1.Items.Add(tagTabItem1);
            tabHeader1.Items.Add(tagTabItem2);
            tabHeader1.Items.Add(tagTabItem3);
            tabHeader1.Items.Add(tagTabItem4);
            tabHeader1.LeftGap = 50;
            tabHeader1.Location = new Point(0, 0);
            tabHeader1.Margin = new Padding(4);
            tabHeader1.Name = "tabHeader1";
            tabHeader1.ShowButton = true;
            tabHeader1.ShowIcon = true;
            tabHeader1.Size = new Size(1436, 38);
            tabHeader1.TabIndex = 0;
            tabHeader1.Text = "V25数据采集";
            tabHeader1.TabChanged += tabHeader1_TabChanged;
            // 
            // plMain
            // 
            plMain.AutoScroll = true;
            plMain.Dock = DockStyle.Fill;
            plMain.Location = new Point(0, 38);
            plMain.Margin = new Padding(4);
            plMain.Name = "plMain";
            plMain.Size = new Size(1436, 715);
            plMain.TabIndex = 1;
            plMain.Text = "panel1";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1436, 753);
            Controls.Add(plMain);
            Controls.Add(tabHeader1);
            Margin = new Padding(4);
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