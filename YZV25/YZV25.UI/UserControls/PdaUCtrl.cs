using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using YZV25.Dto;

namespace YZV25.UI.UserControls
{
    public partial class PdaUCtrl : UserControl
    {
        //PdaQueryDto dto;
        public PdaUCtrl()
        {
            InitializeComponent();

        }

        private void PdaUCtrl_Load(object sender, EventArgs e)
        {

            //this.txtBarCode.Text = this.dto?.Barcode;

            //this.txtDev.Text = $"{this.dto?.DeviceTitle}({this.dto?.DeviceName})-{this.dto?.DeviceCode}";

            //this.txtPDANO.Text = this.dto?.No.ToString();
            //this.txtTime.Text = this.dto?.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void Update(PdaQueryDto dto)
        {
            this.Tag = dto;
            this.txtBarCode.Text = dto?.Barcode;

            this.txtDev.Text = $"{dto?.DeviceTitle}({dto?.DeviceName})";

            this.txtDevCode.Text = dto?.DeviceCode;

            this.txtPDANO.Text = dto?.No.ToString();
            this.txtTime.Text = dto?.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss");

        }
    }
}
