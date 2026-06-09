using Dm;
using HslCommunication.Core.Net;
using HslCommunication.MQTT;
using MediatR;
using Newtonsoft.Json;
using OpcUaHelper;
using System.Text;
using WinformApp.Utils;
using YZV25.Core.Handler;
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


        protected override bool SendMesStatusData(DeviceStatusInfo deviceStatusInfo)
        {
            var result = this.mesRpcClient.PostDeviceStatus(this.code, deviceStatusInfo);
            // logger.LogWarning($"{devicetype}设备:{deviceName}({deviceCode}),发送MES状态数据:{deviceStatusInfo.FaultConditions}，MES：{result.Success}，{result.Message}");

            this.mediator.Publish(new LogMesResultRequest()
            {
                Payload = deviceStatusInfo.FaultConditions,
                DeviceCode = deviceStatusInfo.DeviceCode,
                DeviceName = deviceStatusInfo.Name,
                DeviceType = deviceStatusInfo.DeviceType,
                MesOperation = "状态数据",
                BarCode = deviceStatusInfo.Barcode,
                PdaNo = deviceStatusInfo.PdaNo,
                Response = result
            });

            return result.Success;
        }



        protected override bool SendMesData(string barcode, DeviceInfo deviceInfo, string status)
        {
            var data = _opcUaClient.ReadFSWData();
            if (data == null)
            {
                logger.LogWarning($"{devicetype}设备:{deviceName}({deviceCode}),条码:{barcode},{status},读取数据失败。");
                // return false;

                data = new Model.Common.搅拌焊();
            }

            if (status == "加工中")
            {
                var mesResp = mesRpcClient.PostHandling(barcode, deviceInfo, data);

              
                this.mediator.Publish(new LogMesResultRequest()
                {
                    Payload = data,
                    DeviceCode = deviceInfo.DeviceCode,
                    DeviceName = deviceInfo.Name,
                    DeviceType = deviceInfo.DeviceType,
                    MesOperation = "加工中",
                    BarCode = barcode,
                    PdaNo = deviceInfo.PdaNo,
                    Response = mesResp
                });
                return mesResp.Success;

            }


            if (status == "加工完成")
            {
                var mesResp = mesRpcClient.PostOutStation(barcode, deviceInfo, data);

              
                this.mediator.Publish(new LogMesResultRequest()
                {
                    Payload = data,
                    DeviceCode = deviceInfo.DeviceCode,
                    DeviceName = deviceInfo.Name,
                    DeviceType = deviceInfo.DeviceType,
                    MesOperation = "加工完成",
                    BarCode = barcode,
                    PdaNo = device.PdaNo,
                    Response = mesResp

                });

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
                    Barcode= barcode,
                    PdaNo =deviceInfo.PdaNo,
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
                Barcode=barcode,
                PdaNo = deviceInfo.PdaNo,
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
