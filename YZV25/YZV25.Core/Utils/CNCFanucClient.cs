using HslCommunication;
using HslCommunication.CNC.Fanuc;
using YZV25.Dto;

namespace YZV25.Utils
{
    public class CNCFanucClient
    {
        public readonly FanucSeries0i fanuc;
        public CNCFanucClient(string ip, int port)
        {
            fanuc = new FanucSeries0i(ip, port);

        }

        public bool Connect()
        {
            fanuc.ConnectClose();
            OperateResult connect = fanuc.ConnectServer();
            if (connect.IsSuccess)
            {
                Console.WriteLine("fannuc连接成功");
            }
            else
            {
                Console.WriteLine("fanuc连接失败: " + connect.Message);
            }

            return connect.IsSuccess;
        }


        public CNC ReadCncData()
        {
            //如果读取系统状态信息成功，则继续读取其他数据
            OperateResult<SysStatusInfo> read = fanuc.ReadSysStatusInfo();
            if (read.IsSuccess) {

                CNC data = new CNC();

                data.工艺_轴数_实际值 = fanuc.ReadSysInfo().Content?.Axes.ToString() ?? "";
                data.工艺_机床软件版本_实际值 = fanuc.ReadSysInfo().Content?.Version ?? "";
                data.工艺_程序名称_设定值 = fanuc.ReadSystemProgramCurrent().Content1;
                data.工艺_当前刀号_实际值 = fanuc.ReadCutterNumber().Content.ToString();
                data.工艺_主轴负载率_实际值 = fanuc.ReadSpindleLoad().Content.ToString();
                data.工艺_实际进给_实际值 = fanuc.ReadSpindleSpeedAndFeedRate().Content2.ToString();
                data.生产_产量 = fanuc.ReadCurrentProduceCount().Content.ToString();

                return data;
            }
            return new CNC();
        }

        public SysStatusInfo ReadSysStatus() {

            OperateResult<SysStatusInfo> read = fanuc.ReadSysStatusInfo();

            if (read.IsSuccess && read.Content != null)
            {
                return read.Content;
            }
            return null;



        }
        public int ReadWarnStatus()
        {

            OperateResult<int> read = fanuc.ReadAlarmStatus();

            return read.Content;

        }
        public List<SysAlarm> ReadCncWarn()
        {
            OperateResult<SysAlarm[]> read = fanuc.ReadSystemAlarm();

            if (read.IsSuccess && read.Content != null && read.Content.Length > 0)
            {
                return read.Content.ToList();
            }

            return new List<SysAlarm>();
        }



    }
}
