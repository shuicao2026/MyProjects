namespace YZV25.UI.UserControls
{
    partial class PdaListUCtrl
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
            fpContainer = new AntdUI.FlowPanel();
            SuspendLayout();
            // 
            // fpContainer
            // 
            fpContainer.Dock = DockStyle.Fill;
            fpContainer.Location = new Point(0, 0);
            fpContainer.Name = "fpContainer";
            fpContainer.Size = new Size(805, 543);
            fpContainer.TabIndex = 0;
            fpContainer.Text = "flowPanel1";
            // 
            // PdaListUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(fpContainer);
            Name = "PdaListUserControl";
            Size = new Size(805, 543);
            Load += PdaListUserControl_Load;
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.FlowPanel fpContainer;
    }
}
