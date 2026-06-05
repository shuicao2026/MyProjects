using AntdUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using YZV25.Dto;
using YZV25.UI.Rpc;

namespace YZV25.UI.UserControls
{
    public partial class PdaListUCtrl : UserControl
    {
        PdaRpc pdaRpc = new PdaRpc();
        private System.Timers.Timer _timer;

        public PdaListUCtrl()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer(5000); // 每1秒触发一次
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true; // 重复触发
            _timer.Enabled = false; // 初始不启动
        }

        private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            var result = await pdaRpc.GetRealTimePda();

            foreach (PdaUCtrl item in this.fpContainer.Controls)
            {
                if (item != null && item.Tag != null)
                {
                    var data = (item.Tag as PdaQueryDto);

                    if (data == null)
                    {
                        continue;
                    }
                    var newData = result.data.FirstOrDefault(d => d.Id == data.Id);
                    if (newData == null)
                    {
                        continue;
                    }
                    item.Update(newData);

                }

            }

        }

        private async void PdaListUserControl_Load(object sender, EventArgs e)
        {
            var result = await pdaRpc.GetRealTimePda();

            if (result != null && result.code == 0)
            {
                var pdaList = result.data.OrderByDescending(p => p.Id);
                foreach (var item in pdaList)
                {
                    var ctrl = new PdaUCtrl();
                    ctrl.Update(item);
                    this.fpContainer.Controls.Add(ctrl);

                }
            }

        }
    }
}
