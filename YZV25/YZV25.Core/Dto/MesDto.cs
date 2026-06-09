using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YZV25.Dto
{
    public class MesDto<T> where T:class
    {
        public string ApiType { get; set; }
        public List<Parameter<T>> Parameters { get; set; }
        public string Method { get; set; }
        public Context Context { get; set; }
    }

    public class Context
    {
        public string Ticket { get; set; }
        public int InvOrgId { get; set; }
    }

    public class Parameter<T> where T : class
    {
        public T Value { get; set; }
    }

    public class Value
    {
        public int invOrg { get; set; } = 1;
        public string deviceCode { get; set; }
        public int deviceType { get; set; }
        public string actualValue { get; set; }
        public string detectionParamValue { get; set; }


        public string sn { get; set; }
        public int moveType { get; set; }
        public DateTime dataGenTime { get; set; }
        public bool isFirstProcess { get; set; }
        public string detectionResult { get; set; }
    }


    public class ValueStatus
    {
        public int invOrg { get; set; } = 1;
        public string deviceCode { get; set; }
        public int deviceType { get; set; }
        public int runningState { get; set; }


        public int isOnline { get; set; }

        public string faultConditions { get; set; }

        public DateTime stateGenTime { get; set; }


    }


    public class DeviceInfo
    {

        /// <summary>
        /// 设备对应pda编号
        /// </summary>
        public int PdaNo { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设备唯一编码
        /// </summary>

        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }


        public string Barcode { get; set; }



    }

    public class DeviceStatusInfo : DeviceInfo
    {
        public int IsOnline { get; set; } = 1;

        public string FaultConditions { get; set; }

        public int RunningState { get; set; } = 1;
        public DateTime StateGenTime { get; set; }

    }

    public class MesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }
        public Context Context { get; set; }
    }
}
