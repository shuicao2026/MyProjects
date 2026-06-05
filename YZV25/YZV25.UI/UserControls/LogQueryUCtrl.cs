using AntdUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YZV25UI.Dto;
using YZV25UI.Rpc;
using static System.Net.Mime.MediaTypeNames;

namespace YZV25.UI.UserControls
{
    public partial class LogQueryUCtrl : UserControl
    {
        LogQueryRpc logQueryRpc = new LogQueryRpc();
        private string text = string.Empty;
        private int pageIndex = 1;
        private int pageSize = 50;
        private readonly BindingList<LogDto> logQuerys = new();

        public LogQueryUCtrl()
        {
            InitializeComponent();
            dgvLog.AutoGenerateColumns = false;
            dgvLog.DataSource = logQuerys;
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


        private async void btnQuery_Click(object sender, EventArgs e)
        {
            text = tbText.Text.Trim();
            this.pageIndex = 1;

            var res = await logQueryRpc.QueryLogs(text, pageIndex, pageSize);
            logQuerys.Clear();
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
}
