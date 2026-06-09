using AntdUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using YZV25UI.Dto;
using YZV25UI.Rpc;

namespace YZV25.UI.UserControls
{
    public partial class RealTimeLogUCtrl : UserControl
    {
        LogQueryRpc logQueryRpc = new LogQueryRpc();
        private System.Timers.Timer _timer;
        private int _counter = 0;
        private int lastId = 0;

        private int count = 0;
        private string text = string.Empty;
        private int pageIndex = 1;
        private int pageSize = 10;

        AntList<BusinessLogDto> logRts = new AntList<BusinessLogDto>();
        public RealTimeLogUCtrl()
        {
            InitializeComponent();
            InitialTableColumns();
            InitializeTimer();

            this.pagination1.PageSize = pageSize;
            this.pagination1.Enabled = false;


        }

        private void InitializeTimer()
        {
            tbLog.Binding(logRts);
            _timer = new System.Timers.Timer(1000); // 每1秒触发一次
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true; // 重复触发
            _timer.Enabled = false; // 初始不启动
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // 这是一个后台线程，不能直接更新UI
            _counter++;
            //string message = $"定时任务执行次数: {_counter}";

            var res = await logQueryRpc.GetRealTimeLog(lastId);

            if (res == null || res.data == null || res.data.Count == 0)
            {
                return;
            }
            lastId = res.data.Select(x => x.Id).Max();

            // 通过Invoke将更新UI的操作封送到UI线程
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    //logRts.Clear();

                    if (logRts.Count > 1000)
                    {
                        logRts.Clear();
                    }

                    foreach (var item in res.data)
                    {
                        logRts.Add(item);
                    }


                    //ScrollToBottom();


                }));
            }
            else
            {
                // label1.Text = message;
            }




        }


        private void InitialTableColumns()
        {
            var cols = new ColumnCollection() {
                new Column("Id", "序号"){
                Width="40"},
                new Column("PdaNo", "PDA编号")
                {
                    Width="80"
                },
                     new Column("DeviceName", "设备名称")
                {
                    Width="60",
                    MaxWidth="60"


                },
               new Column("DeviceCode", "设备代码")
                {
                    Width="100",
                    MaxWidth="100"
                },

               new Column("Barcode", "条码编号")
                {
                    Width="180",
                    MaxWidth="180"
                },
                new Column("MesEnabledStatus", "MES启用")
                {
                    Width="80",
                    MaxWidth="80"
                },
                new Column("MesOperation", "MES操作")
                {
                    Width="80",
                    MaxWidth="80"
                },

               new Column("MesResult", "MES结果")
                {
                    Width="80",
                    MaxWidth="80"
                },

                 new Column("MesReturnContent", "MES返回内容")
                {
                    MinWidth="160",


                },



            };
            this.tbLog.Columns = cols;
        }


        private void RealTimeLogUCtrl_Load(object sender, EventArgs e)
        {
            switch1.Checked = true;
            _timer.Start();

        }

        private void tbLog_CellClick(object sender, TableClickEventArgs e)
        {

        }

        private void switch1_CheckedChanged(object sender, BoolEventArgs e)
        {
            if (e.Value)
            {

                this.logRts.Clear();
                _timer.Start();

                this.pagination1.Enabled = false;
            }
            else
            {
                _timer.Stop();

                this.pagination1.Enabled = true;
            }

        }


        private async void btnQuery_Click_2(object sender, EventArgs e)
        {
            this.pagination1.Enabled = true;

            switch1.Checked = false;
            _timer.Stop();
            logRts.Clear();

            ReturnDataDto<List<BusinessLogDto>> res = await logQueryRpc.GetBusinessLogs(tbText.Text, pageIndex, pageSize);

            if (res.data == null)
            {
                return;
            }

            logRts.AddRange(res.data);


            this.pagination1.Total = res.Count;

        }

        private async void pagination1_ValueChanged(object sender, PagePageEventArgs e)
        {
            ReturnDataDto<List<BusinessLogDto>> res = await logQueryRpc.GetBusinessLogs(tbText.Text,
                e.Current, pageSize);
            logRts.Clear();

            logRts.AddRange(res.data);


            this.pagination1.Total = res.Count;
        }
    }
}
