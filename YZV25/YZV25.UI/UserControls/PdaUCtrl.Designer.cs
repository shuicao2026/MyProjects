namespace YZV25.UI.UserControls
{
    partial class PdaUCtrl
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
            plContainer = new AntdUI.Panel();
            txtTime = new AntdUI.Input();
            label5 = new AntdUI.Label();
            txtBarCode = new AntdUI.Input();
            txtDev = new AntdUI.Input();
            txtPDANO = new AntdUI.Input();
            label4 = new AntdUI.Label();
            label3 = new AntdUI.Label();
            label2 = new AntdUI.Label();
            label1 = new AntdUI.Label();
            txtDevCode = new AntdUI.Input();
            label7 = new AntdUI.Label();
            plContainer.SuspendLayout();
            SuspendLayout();
            // 
            // plContainer
            // 
            plContainer.Controls.Add(txtDevCode);
            plContainer.Controls.Add(label7);
            plContainer.Controls.Add(txtTime);
            plContainer.Controls.Add(label5);
            plContainer.Controls.Add(txtBarCode);
            plContainer.Controls.Add(txtDev);
            plContainer.Controls.Add(txtPDANO);
            plContainer.Controls.Add(label4);
            plContainer.Controls.Add(label3);
            plContainer.Controls.Add(label2);
            plContainer.Controls.Add(label1);
            plContainer.Dock = DockStyle.Fill;
            plContainer.Location = new Point(0, 0);
            plContainer.Name = "plContainer";
            plContainer.Size = new Size(325, 252);
            plContainer.TabIndex = 0;
            plContainer.Text = "panel1";
            // 
            // txtTime
            // 
            txtTime.Location = new Point(111, 194);
            txtTime.Name = "txtTime";
            txtTime.Radius = 0;
            txtTime.ReadOnly = true;
            txtTime.Size = new Size(200, 32);
            txtTime.TabIndex = 9;
            txtTime.Text = "input4";
            // 
            // label5
            // 
            label5.Location = new Point(39, 199);
            label5.Name = "label5";
            label5.Size = new Size(75, 23);
            label5.TabIndex = 8;
            label5.Text = "更新时间";
            // 
            // txtBarCode
            // 
            txtBarCode.Location = new Point(111, 154);
            txtBarCode.Name = "txtBarCode";
            txtBarCode.Radius = 0;
            txtBarCode.ReadOnly = true;
            txtBarCode.Size = new Size(200, 32);
            txtBarCode.TabIndex = 6;
            txtBarCode.Text = "input3";
            // 
            // txtDev
            // 
            txtDev.Location = new Point(111, 74);
            txtDev.Name = "txtDev";
            txtDev.Radius = 0;
            txtDev.ReadOnly = true;
            txtDev.Size = new Size(200, 32);
            txtDev.TabIndex = 5;
            txtDev.Text = "input2";
            // 
            // txtPDANO
            // 
            txtPDANO.Location = new Point(111, 34);
            txtPDANO.Name = "txtPDANO";
            txtPDANO.Radius = 0;
            txtPDANO.ReadOnly = true;
            txtPDANO.Size = new Size(200, 32);
            txtPDANO.TabIndex = 4;
            txtPDANO.Text = "input1";
            // 
            // label4
            // 
            label4.Location = new Point(39, 159);
            label4.Name = "label4";
            label4.Size = new Size(75, 23);
            label4.TabIndex = 3;
            label4.Text = "当前条码";
            // 
            // label3
            // 
            label3.Location = new Point(39, 130);
            label3.Name = "label3";
            label3.Size = new Size(75, 23);
            label3.TabIndex = 2;
            label3.Text = "";
            // 
            // label2
            // 
            label2.Location = new Point(39, 79);
            label2.Name = "label2";
            label2.Size = new Size(75, 23);
            label2.TabIndex = 1;
            label2.Text = "关联设备";
            // 
            // label1
            // 
            label1.Location = new Point(39, 39);
            label1.Name = "label1";
            label1.Size = new Size(75, 23);
            label1.TabIndex = 0;
            label1.Text = "PDA编号";
            // 
            // txtDevCode
            // 
            txtDevCode.Location = new Point(111, 114);
            txtDevCode.Name = "txtDevCode";
            txtDevCode.Radius = 0;
            txtDevCode.ReadOnly = true;
            txtDevCode.Size = new Size(200, 32);
            txtDevCode.TabIndex = 11;
            txtDevCode.Text = "input2";
            // 
            // label7
            // 
            label7.Location = new Point(39, 119);
            label7.Name = "label7";
            label7.Size = new Size(75, 23);
            label7.TabIndex = 10;
            label7.Text = "设备编码";
            // 
            // PdaUCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(plContainer);
            Name = "PdaUCtrl";
            Size = new Size(325, 252);
            Load += PdaUCtrl_Load;
            plContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel plContainer;
        private AntdUI.Label label1;
        private AntdUI.Label label2;
        private AntdUI.Input txtTime;
        private AntdUI.Label label5;
        private AntdUI.Label label6;
        private AntdUI.Input txtBarCode;
        private AntdUI.Input txtDev;
        private AntdUI.Input txtPDANO;
        private AntdUI.Label label4;
        private AntdUI.Label label3;
        private AntdUI.Input txtDevCode;
        private AntdUI.Label label7;
    }
}
