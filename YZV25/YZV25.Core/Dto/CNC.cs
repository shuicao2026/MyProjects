using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YZV25.Dto
{
    public class CNC
    {
        [JsonProperty("工艺_轴数_实际值")]
        public string 工艺_轴数_实际值 { get; set; }

        [JsonProperty("工艺_程序名称_实际值")]
        public string 工艺_程序名称_实际值 { get; set; }

        [JsonProperty("工艺_机床软件版本_实际值")]
        public string 工艺_机床软件版本_实际值 { get; set; }

        [JsonProperty("设备状态_状态")]
        public string 设备状态_状态 { get; set; }

        [JsonProperty("工艺_当前刀号_实际值")]
        public string 工艺_当前刀号_实际值 { get; set; }

        [JsonProperty("工艺_实际主轴转速_实际值")]
        public string 工艺_实际主轴转速_实际值 { get; set; }

        [JsonProperty("工艺_运行模式_实际值")]
        public string 工艺_运行模式_实际值 { get; set; }

        [JsonProperty("工艺_主轴负载率_实际值")]
        public string 工艺_主轴负载率_实际值 { get; set; }

        [JsonProperty("工艺_实际进给_实际值")]
        public string 工艺_实际进给_实际值 { get; set; }

        [JsonProperty("生产_产量")]
        public string 生产_产量 { get; set; }

        [JsonProperty("急停_报警")]
        public string 急停_报警 { get; set; }

        [JsonProperty("工艺_机床软件版本_设定值")]
        public string 工艺_机床软件版本_设定值 { get; set; }

        [JsonProperty("工艺_程序名称_设定值")]
        public string 工艺_程序名称_设定值 { get; set; }

        [JsonProperty("工艺_机床软件版本_实际值1")]
        public string 工艺_机床软件版本_实际值1 { get; set; }
    }
}
