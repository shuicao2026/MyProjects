using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Common
{
    public class 搅拌焊
    {


        public string 设备状态_状态 { get; set; }
        public string 运行状态_实际值 { get; set; }

        public string 工艺_主轴倍率_实际值 { get; set; }
        public string 工艺_进给倍率_实际值 { get; set; }
        public string 工艺_主轴速度_设定值 { get; set; }
        public string 工艺_主轴速度_实际值 { get; set; }

        public string 工艺_一号搅拌头位移距离_实际值 { get; set; }
        public string 工艺_一号搅拌头使用次数_实际值 { get; set; }
        public string 工艺_搅拌头最大位移_实际值 { get; set; }
        public string 工艺_搅拌头最大使用次数_实际值 { get; set; }

        public string 工艺_X轴坐标_实际值 { get; set; }
        public string 工艺_Y轴坐标_实际值 { get; set; }
        public string 工艺_Z轴坐标_实际值 { get; set; }
        public string 工艺_C轴坐标_实际值 { get; set; }

        public string 报警_报警数 { get; set; }
        public string NC报警_报警 { get; set; }
        public string PLC报警_报警 { get; set; }

        public string 工艺_NC程序刚结束的运行时间_实际值 { get; set; }
        public string 工艺_NC程序停止的时间_实际值 { get; set; }
        public string 工艺_NC程序的总运行时间_实际值 { get; set; }
        public string 工艺_NC程序加工程序名_设定值 { get; set; }

        public string 位移_实际值 { get; set; }
        public string 急停_报警 { get; set; }
        public string 实际压力_实际值 { get; set; }
        public string 启动使能 { get; set; }
        public string 工艺_NC程序加工程序名_实际值 { get; set; }
    }
}
