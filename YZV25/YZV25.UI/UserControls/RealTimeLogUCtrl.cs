using AntdUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        AntList<LogDto> logRts = new AntList<LogDto>();
        public RealTimeLogUCtrl()
        {
            InitializeComponent();
            InitialTableColumns();
            InitializeTimer();
        

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
                new Column("Level", "日志级别")
                {
                    Width="120"
                },
                     new Column("TimeStamp", "日志时间")
                {
                    Width="160",
                    MaxWidth="160"


                },
                new Column("Message", "日志信息")
                {
                   MaxWidth="50%"
                },
           
            };
            this.tbLog.Columns = cols;
        }

       
        private void RealTimeLogUCtrl_Load(object sender, EventArgs e)
        {
            _timer.Start();

        }
    }
}
