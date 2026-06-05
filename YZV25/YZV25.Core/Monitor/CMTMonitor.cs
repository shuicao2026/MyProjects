using Dm;
using HslCommunication.Core.Net;
using HslCommunication.MQTT;
using MediatR;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Text;
using WinformApp.Utils;
using YZV25.Controllers;
using YZV25.Core.Monitor;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Entity;
using YZV25.Utils;

namespace YZV25.Monitor
{
    public class CMTMonitor : BaseMonitor
    {


        private readonly CMTMqttRpcClient mqttRpcClient;
        private readonly MesRpcClient mesRpcClient;


        private string face;

        public CMTMonitor(

            string deviceName,

            string[] signalPoints,
            MesRpcClient mesRpcClient,
            MqttClient mqttClient,
            CMTMqttRpcClient mqttRpcClient,
            IMediator mediator,
            ILogger<CMTMonitor> logger) : base("CMT", deviceName, signalPoints, mqttClient, mediator, logger)
        {


            this.mesRpcClient = mesRpcClient;
            this.mqttRpcClient = mqttRpcClient;




        }


        protected override bool SendMesStatusData(DeviceStatusInfo deviceInfo)
        {
            var mesResp = mesRpcClient.PostDeviceStatus(code, deviceInfo);


            logger.LogInformation($"{devicetype}设备{deviceName},{statusSignalPoint},条码:{code},发送MES设备状态数据:{deviceInfo.RunningState},{deviceInfo.FaultConditions}");

            return mesResp != null && mesResp.Success;
        }

        /// <summary>
        /// 发送MES 加工数据
        /// </summary>
        /// <returns></returns>
        protected override bool SendMesData(string barcode, DeviceInfo deviceInfo, string status)
        {
            var data = mqttRpcClient.ReadCMTData();
            if (data == null)
            {
                logger.LogWarning($"{deviceInfo.DeviceType}设备{deviceInfo.Name},条码:{barcode},{status},获取数据失败。");
                //return false;

                data= new Newtonsoft.Json.Linq.JObject();
            }

            if (status == "加工中")
            {
                var mesResp = mesRpcClient.PostHandling(barcode, deviceInfo, data);

                logger.LogInformation($"{devicetype}设备:{deviceName},条码:{barcode},{status},发送MES数据：{JsonConvert.SerializeObject(data)}");

                return mesResp.Success;

            }


            if (status == "加工完成")
            {
                var mesResp = mesRpcClient.PostOutStation(barcode, deviceInfo, data);

                logger.LogInformation($"{devicetype}设备:{deviceName},条码:{barcode},{status},发送MES数据：{JsonConvert.SerializeObject(data)}");

                return mesResp.Success;

            }

            return false;

        }


        protected override bool HandleWorkingStatus(string barcode, DeviceInfo deviceInfo)
        {

            return SendMesData(barcode, deviceInfo, "加工中");
        }

        protected override bool HandleFinishedStatus(string barcode, DeviceInfo deviceInfo, object payLoad)
        {

            mqttRpcClient.SendOKSingnal(this.finishedSignalPoint);


            return SendMesData(barcode, deviceInfo, "加工完成");
        }

        protected override bool HandleAlarmStatus(string barcode, DeviceInfo deviceInfo)
        {

            if (string.IsNullOrEmpty(this.face))
            {
                logger.LogWarning($"{deviceInfo.DeviceType}设备{deviceInfo.Name},条码:{barcode},报警，工作面未设置。");
                return false;
            }

            
            var devStatus = mqttRpcClient.ReadStatusData($"{deviceInfo.Name}/设备状态{this.face}");

            //devStatus==-1 代表设备离线或者数据获取失败
            if (devStatus==-1)
            {
                var offline = new Dto.DeviceStatusInfo
                {
                    DeviceCode = deviceInfo.DeviceCode,
                    DeviceType = deviceInfo.DeviceType,
                    Name = deviceInfo.Name,
                    IsOnline =0,
                    RunningState = 0,
                    StateGenTime = DateTime.Now,
                    FaultConditions = $""
                };


                return SendMesStatusData(offline);

            }

            var warnStatus = mqttRpcClient.ReadStatusData($"{deviceInfo.Name}/设备报警代码{this.face}");

       

            var statusInfo = new Dto.DeviceStatusInfo
            {
                DeviceCode = deviceInfo.DeviceCode,
                DeviceType = deviceInfo.DeviceType,
                Name = deviceInfo.Name,
                IsOnline = 1,
                RunningState = devStatus,
                StateGenTime = DateTime.Now,
                FaultConditions = $"{warnStatus}"
            };


            return SendMesStatusData(statusInfo); 
        }

        protected override object PraseMqttPayload(byte[] Payload)
        {
            int 信号 = Convert.ToInt32(Encoding.UTF8.GetString(Payload));
            return 信号;
        }

        protected override bool CheckFinished(object payload)
        {
            var 信号 = (int)payload;

            return 信号 == 70;
        }

        protected override void BeforeStart(MonitorContext monitorContext)
        {
           
            this.finishedSignalPoint = monitorContext.FinishSigPoint;
            this.statusSignalPoint = monitorContext.StatusSigPoint;
            this.device = monitorContext.Device;
            this.deviceCode = monitorContext.DeviceCode;
            this.code = monitorContext.BarCode;
            this.face = monitorContext.Face;
        }
    }
}
