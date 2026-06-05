using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Timers;
using System.Windows.Forms;
using YZV25UI.Dto;
using YZV25UI.Rpc;
using static System.Net.Mime.MediaTypeNames;

namespace YZV25UI
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer _timer;
        private int _counter = 0;

        LogQueryRpc logQueryRpc = new LogQueryRpc();

        private readonly BindingList<LogDto> logRts = new();


        private readonly BindingList<LogDto> logQuerys = new();
        private string text = string.Empty;
        private int pageIndex = 1;
        private int pageSize = 50;

        private int lastId = 0;
        public Form1()
        {
            InitializeComponent();

            InitializeTimer();
            dgvRuningTimeLogs.AutoGenerateColumns = false;
            dgvRuningTimeLogs.DataSource = logRts;

            dgvLog.AutoGenerateColumns = false;
            dgvLog.DataSource = logQuerys;

        }

        private void InitializeTimer()
        {
            _timer = new System.Timers.Timer(1000); // 每1秒触发一次
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true; // 重复触发
            _timer.Enabled = false; // 初始不启动
        }

        public void ScrollToBottom()
        {
            if (dgvRuningTimeLogs.Rows.Count > 0)
            {
                dgvRuningTimeLogs.FirstDisplayedScrollingRowIndex =
                    dgvRuningTimeLogs.Rows.Count - 1;
            }
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


                    ScrollToBottom();


                }));
            }
            else
            {
                // label1.Text = message;
            }




        }

        private void Form1_Load(object sender, EventArgs e)
        {

            _timer.Start();
        }

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            text = tbText.Text.Trim();
            this.pageIndex = 1;
            logQuerys.Clear();
            var res = await logQueryRpc.QueryLogs(text, pageIndex, pageSize);

            if (res == null || res.data == null || res.data.Count == 0)
            {
                return;
            }
            res.data.ForEach(x =>
            {
                logQuerys.Add(x);
            });
        }


        private bool CheckIfScrolledToBottom()
        {
            if (dgvLog.Rows.Count == 0) return false;

            int lastRowIndex = dgvLog.Rows.Count - 1;

            // 检查最后一行是否在可视区域内
            if (dgvLog.Rows[lastRowIndex].Displayed)
            {
                return true;
            }
            return false;
        }
        private bool IsScrolledToBottom()
        {
            // 方法1：通过 FirstDisplayedScrollingRowIndex
            int lastVisibleRowIndex = dgvLog.FirstDisplayedScrollingRowIndex
                                   + dgvLog.DisplayedRowCount(false) - 1;

            return lastVisibleRowIndex >= dgvLog.Rows.Count - 1;
        }


        private async void dgvLog_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation != ScrollOrientation.VerticalScroll)
            {

                return;
            }
            if (IsScrolledToBottom())
            {
                this.pageIndex++;
                //logQuerys.Clear();
                var res = await logQueryRpc.QueryLogs(text, pageIndex, pageSize);

                if (res == null || res.data == null || res.data.Count == 0)
                {
                    return;
                }
                res.data.ForEach(x =>
                {
                    logQuerys.Add(x);
                });

            }
        }

        private void dgvLog_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLog.CurrentRow != null &&
        !dgvLog.CurrentRow.IsNewRow)
            {
                var row = dgvLog.CurrentRow;

                //int id = Convert.ToInt32(row.Cells["Id"].Value);
                //string name = row.Cells["Name"].Value.ToString();

                // 或绑定对象时
                var item = row.DataBoundItem as LogDto;
                rtbContent.Clear();
                rtbContent.AppendText($"日志序号: {item.Id}\n");

                rtbContent.AppendText($"日志级别: {item.Level}\n");

                rtbContent.AppendText($"日志时间: {item.TimeStamp}\n");
                rtbContent.AppendText($"日志信息: {item.Message}\n");
                rtbContent.AppendText($"异常信息: {item.Exception}\n");

            }
        }
   
    
    }
}
