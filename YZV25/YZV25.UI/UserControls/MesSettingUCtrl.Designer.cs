namespace YZV25.UI.UserControls
{
    partial class MesSettingUCtrl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MesSettingUCtrl));
            panel1 = new AntdUI.Panel();
            label2 = new AntdUI.Label();
            InMESAddr = new AntdUI.Input();
            cbEnable = new AntdUI.Checkbox();
            label1 = new AntdUI.Label();
            gridPanel1 = new AntdUI.GridPanel();
            panel2 = new AntdUI.Panel();
            btnOK = new AntdUI.Button();
            btnfresh = new AntdUI.Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(label2);
            panel1.Controls.Add(InMESAddr);
            panel1.Controls.Add(cbEnable);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(844, 49);
            panel1.TabIndex = 2;
            panel1.Text = "panel1";
            // 
            // label2
            // 
            label2.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label2.Location = new Point(32, 16);
            label2.Name = "label2";
            label2.Size = new Size(75, 23);
            label2.TabIndex = 2;
            label2.Text = "MES地址";
            // 
            // InMESAddr
            // 
            InMESAddr.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            InMESAddr.JoinMode = AntdUI.TJoinMode.Right;
            InMESAddr.Location = new Point(98, 11);
            InMESAddr.Name = "InMESAddr";
            InMESAddr.PlaceholderText = "请输入MES地址";
            InMESAddr.Size = new Size(334, 32);
            InMESAddr.TabIndex = 1;
            // 
            // cbEnable
            // 
            cbEnable.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            cbEnable.Location = new Point(514, 16);
            cbEnable.Name = "cbEnable";
            cbEnable.Size = new Size(162, 23);
            cbEnable.TabIndex = 0;
            cbEnable.Text = "启用MES";
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            label1.Location = new Point(0, 49);
            label1.Name = "label1";
            label1.Prefix = "";
            label1.PrefixColour = AntdUI.Colour.Primary;
            label1.PrefixSvg = resources.GetString("label1.PrefixSvg");
            label1.Size = new Size(844, 41);
            label1.TabIndex = 5;
            label1.Text = "屏蔽的设备";
            // 
            // gridPanel1
            // 
            gridPanel1.BackColor = Color.Transparent;
            gridPanel1.Dock = DockStyle.Top;
            gridPanel1.Gap = 20;
            gridPanel1.Location = new Point(0, 90);
            gridPanel1.Name = "gridPanel1";
            gridPanel1.Size = new Size(844, 367);
            gridPanel1.Span = "25% 25% 25% 25%;25% 25% 25% 25%;25% 25% 25% 25%;25% 25% 25% 25%-25% 25% 25% 25%";
            gridPanel1.TabIndex = 6;
            gridPanel1.Text = "gridPanel1";
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.Controls.Add(btnOK);
            panel2.Controls.Add(btnfresh);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 463);
            panel2.Name = "panel2";
            panel2.Size = new Size(844, 61);
            panel2.TabIndex = 7;
            panel2.Text = "panel2";
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.BackColor = Color.PaleGoldenrod;
            btnOK.BorderWidth = 1F;
            btnOK.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnOK.Location = new Point(711, 3);
            btnOK.Name = "btnOK";
            btnOK.Radius = 0;
            btnOK.Size = new Size(117, 46);
            btnOK.TabIndex = 1;
            btnOK.Text = "确定";
            btnOK.Click += btnOK_Click;
            // 
            // btnfresh
            // 
            btnfresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnfresh.BorderWidth = 1F;
            btnfresh.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            btnfresh.Location = new Point(548, 3);
            btnfresh.Name = "btnfresh";
            btnfresh.Radius = 0;
            btnfresh.Size = new Size(117, 46);
            btnfresh.TabIndex = 0;
            btnfresh.Text = "刷新";
            btnfresh.Click += btnfresh_Click;
            // 
            // MesSettingUCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(panel2);
            Controls.Add(gridPanel1);
            Controls.Add(label1);
            Controls.Add(panel1);
            Name = "MesSettingUCtrl";
            Size = new Size(844, 524);
            Load += MesSettingUCtrl_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Checkbox cbEnable;
        private AntdUI.Label label1;
        private AntdUI.GridPanel gridPanel1;
        private AntdUI.Panel panel2;
        private AntdUI.Button btnOK;
        private AntdUI.Button btnfresh;
        private AntdUI.Label label2;
        private AntdUI.Input InMESAddr;
    }
}
