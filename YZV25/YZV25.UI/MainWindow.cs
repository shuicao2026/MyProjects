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
using YZV25.UI.UserControls;

namespace YZV25.UI
{
    public partial class MainWindow : AntdUI.Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void tabHeader1_TabChanged(object sender, AntdUI.TabChangedEventArgs e)
        {

            // Handle the result as needed
            switch (e.Index)
            {
                case 0:
                    {
                        var ctrl = new PdaListUCtrl();
                        ctrl.Dock = DockStyle.Fill;
                        plMain.Controls.Clear();
                        plMain.Controls.Add(ctrl);

                    }
                    break;
                case 1:
                    {
                        var ctrl = new RealTimeLogUCtrl();
                        ctrl.Dock = DockStyle.Fill;
                        plMain.Controls.Clear();
                        plMain.Controls.Add(ctrl);

                    }
                    break;

                case 2:
                    {
                        var ctrl = new LogQueryUCtrl();
                        ctrl.Dock = DockStyle.Fill;
                        plMain.Controls.Clear();
                        plMain.Controls.Add(ctrl);

                    }
                    break;

                case 3:
                    {
                        var ctrl = new MesSettingUCtrl(this);
                        ctrl.Dock = DockStyle.Fill;
                        plMain.Controls.Clear();
                        plMain.Controls.Add(ctrl);
                    }

                    break;

            }




        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            this.tabHeader1.SelectedIndex = 1;
        }
    }
}



