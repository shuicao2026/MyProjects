using AntdUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YZV25.UI.Rpc;
using YZV25UI.Dto;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace YZV25.UI.UserControls
{
    public partial class MesSettingUCtrl : UserControl
    {
        DeviceRpc deviceRpc = new DeviceRpc();
        MesSettingRpc mesSettingRpc = new MesSettingRpc();
        AntdUI.Window _window;
        public MesSettingUCtrl(AntdUI.Window window)
        {
            InitializeComponent();
            this._window = window;
        }

        private async Task InitControls()
        {
            Dictionary<string, bool> dicBlock = new Dictionary<string, bool>();

            var mesSettingRes = await mesSettingRpc.GetSetting();

            var mesSetting = mesSettingRes.data;

            var devRes = await deviceRpc.GetAllDevices();

            cbEnable.Checked = mesSetting.Enable == null ? false : mesSetting.Enable.Value;
            InMESAddr.Text = mesSetting.Hostname;
            this.gridPanel1.Controls.Clear();
            foreach (var item in devRes.data)
            {

                Checkbox checkbox = new Checkbox();

                checkbox.Checked = mesSetting.Block == null ? false : mesSetting.Block.Contains(item.Name);
                checkbox.Tag = item;
                checkbox.Text = $"{item.Title}({item.Name})";
                checkbox.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
                checkbox.Dock = DockStyle.Fill;


                this.gridPanel1.Controls.Add(checkbox);

            }








        }
        private async void MesSettingUCtrl_Load(object sender, EventArgs e)
        {
            await InitControls();
        }

        private async void btnOK_Click(object sender, EventArgs e)
        {

            MesSettingDto mesSettingDto = new MesSettingDto();

            mesSettingDto.Enable = cbEnable.Checked;

            if (!string.IsNullOrEmpty(InMESAddr.Text))
            {
                mesSettingDto.Hostname = InMESAddr.Text;
            }
            var blocks = new List<string>();

            foreach (var ctrl in this.gridPanel1.Controls)
            {
                var c = ((Checkbox)ctrl);

                if (c.Checked != true)
                {
                    continue;
                }

                var d = c.Tag as DeviceDto;

                blocks.Add(d.Name);
            }

            mesSettingDto.Block = blocks.ToArray();


            var res = await mesSettingRpc.Update(mesSettingDto);
            if (res.code == 0)
            {
                AntdUI.Message.success(_window, "修改成功！");

            }
            else
            {
                AntdUI.Message.error(_window, "修改失败！");
            }
        }



        private async void btnfresh_Click(object sender, EventArgs e)
        {
            await InitControls();
        }
    }
}
