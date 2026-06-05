using HslCommunication.Core.Net;
using HslCommunication.MQTT;
using MediatR;
using Newtonsoft.Json;
using OpcUaHelper;
using System.Text;
using WinformApp.Utils;
using YZV25.Core.Monitor;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Entity;

using YZV25.Utils;

namespace YZV25.Monitor
{
    public class FSWMonitor : BaseMonitor
    {


        private readonly FSWOpcUaClient _opcUaClient;
        private readonly MesRpcClient mesRpcClient;


        public FSWMonitor(
            string deviceName,
            string[] signalPoints,
            MesRpcClient mesRpcClient,
            MqttClient mqttClient,
            FSWOpcUaClient opcUaClient,
            IMediator mediator,
              ILogger<FSWMonitor> logger) : base("FSW", deviceName, signalPoints, mqttClient, mediator, logger)
        {

            this.mesRpcClient = mesRpcClient;

            this._opcUaClient = opcUaClient;
            this._opcUaClient.Connect();


        }




        /// <summary>
        /// 发送MES 加工数据
        /// </summary>
        /// <returns></returns>
        protected override bool SendMesData(string barcode, DeviceInfo deviceInfo, string status)
        {
            var data = _opcUaClient.ReadFSWData();
            if (data == null)
            {
                logger.LogWarning($"{devicetype}设备:{deviceName},条码:{barcode},{status},读取数据失败。");
                // return false;

                data = new Model.Common.搅拌焊();
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

        protected override bool HandleFinishedStatus(string barcode, DeviceInfo deviceInfo, object payload)
        {
            return SendMesData(barcode, deviceInfo, "加工完成");
        }

        protected override bool HandleAlarmStatus(string barcode, DeviceInfo deviceInfo)
        {
          
            var data = _opcUaClient.ReadFSWAlarmData();
            if (string.IsNullOrEmpty(data))
            {
                var offlineInfo = new Dto.DeviceStatusInfo
                {
                    DeviceCode = deviceInfo.DeviceCode,
                    DeviceType = deviceInfo.DeviceType,
                    Name = deviceInfo.Name,
                    IsOnline = 0,
                    RunningState = 1,
                    StateGenTime = DateTime.Now,
                    FaultConditions = data
                };


                return SendMesStatusData(offlineInfo);

            }

            var statusInfo = new Dto.DeviceStatusInfo
            {
                DeviceCode = deviceInfo.DeviceCode,
                DeviceType = deviceInfo.DeviceType,
                Name = deviceInfo.Name,
                IsOnline = 1,
                RunningState = 1,
                StateGenTime = DateTime.Now,
                FaultConditions = data
            };


            return SendMesStatusData(statusInfo);
        }
        protected override object PraseMqttPayload(byte[] Payload)
        {
            string finish = (Encoding.UTF8.GetString(Payload));
            return finish;
        }

        protected override bool CheckFinished(object payload)
        {
            string finish = payload as string;
            return string.Equals(finish, "True", StringComparison.OrdinalIgnoreCase);
        }

        protected override bool SendMesStatusData(DeviceStatusInfo deviceStatusInfo)
        {
            var result = this.mesRpcClient.PostDeviceStatus(this.code, deviceStatusInfo);
            logger.LogWarning($"{devicetype}设备:{deviceName},发送MES状态数据:{deviceStatusInfo.FaultConditions}，MES：{result.Success}，{result.Message}");

            return result.Success;
        }

        protected override void BeforeStart(MonitorContext monitorContext)
        {
            this.code = monitorContext.BarCode;
            this.device = monitorContext.Device;
            this.deviceCode = monitorContext.DeviceCode;
            this.finishedSignalPoint = device.FinishSignalPoint;
            this.statusSignalPoint = device.StatusSignalPoint;
        }
    }
}
