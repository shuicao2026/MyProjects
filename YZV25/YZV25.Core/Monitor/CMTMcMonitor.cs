using Dm;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.MQTT;
using MediatR;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Text;
using WinformApp.Utils;
using YZV25.Controllers;
using YZV25.Core.Handler;
using YZV25.Core.Monitor;
using YZV25.Core.Utils;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Entity;
using YZV25.Utils;

namespace YZV25.Monitor
{
    public class CMTMcMonitor : BaseMonitor
    {
        private readonly CMTMcClient cmtMcClient;

        private readonly MesRpcClient mesRpcClient;


        private string face;

        public CMTMcMonitor(

            string deviceName,

            string[] signalPoints,
            MesRpcClient mesRpcClient,
            MqttClient mqttClient,
             CMTMcClient cmtMcClient,
            IMediator mediator,
            ILogger<CMTMonitor> logger) : base("CMT", deviceName, signalPoints, mqttClient, mediator, logger)
        {


            this.mesRpcClient = mesRpcClient;
            this.cmtMcClient = cmtMcClient;

        }


        protected override bool SendMesStatusData(DeviceStatusInfo deviceStatusInfo)
        {
            var mesResp = mesRpcClient.PostDeviceStatus(deviceStatusInfo.Barcode, deviceStatusInfo);

            this.mediator.Publish(new LogMesResultRequest()
            {
                Payload = deviceStatusInfo.FaultConditions,
                DeviceCode = deviceStatusInfo.DeviceCode,
                DeviceName = deviceStatusInfo.Name,
                DeviceType = deviceStatusInfo.DeviceType,
                MesOperation = "状态数据",
                BarCode = deviceStatusInfo.Barcode,
                PdaNo = deviceStatusInfo.PdaNo,
                Response = mesResp
            });

            return mesResp != null && mesResp.Success;
        }

        /// <summary>
        /// 发送MES 加工数据
        /// </summary>
        /// <returns></returns>
        protected override bool SendMesData(string barcode, DeviceInfo deviceInfo, string status)
        {
            var data = cmtMcClient.ReadCMTData(this.face);
            if (data == null)
            {
                logger.LogWarning($"{deviceInfo.DeviceType}设备{deviceInfo.Name}({deviceInfo.DeviceCode}),条码:{barcode},{status},获取数据失败。");

                data = new Newtonsoft.Json.Linq.JObject();
            }

            if (status == "加工中")
            {
                var mesResp = mesRpcClient.PostHandling(barcode, deviceInfo, data);

                //logger.LogInformation($"{devicetype}设备:{deviceName}({deviceInfo.DeviceCode}),条码:{barcode},{status},发送MES数据：{JsonConvert.SerializeObject(data)}");
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

        protected override bool HandleFinishedStatus(string barcode, DeviceInfo deviceInfo, object payLoad)
        {

            cmtMcClient.SendOKSignal(this.face);


            return SendMesData(barcode, deviceInfo, "加工完成");
        }

        protected override bool HandleAlarmStatus(string barcode, DeviceInfo deviceInfo)
        {

            if (string.IsNullOrEmpty(this.face))
            {
                logger.LogWarning($"{deviceInfo.DeviceType}设备{deviceInfo.Name}({deviceInfo.DeviceCode}),条码:{barcode},报警，工作面未设置。");
                return false;
            }


            var devStatus = cmtMcClient.ReadStatusData();

            //devStatus==-1 代表设备离线或者数据获取失败
            if (devStatus == -1)
            {
                var offline = new Dto.DeviceStatusInfo
                {
                    Barcode = barcode,

                    PdaNo = deviceInfo.PdaNo,
                    DeviceCode = deviceInfo.DeviceCode,
                    DeviceType = deviceInfo.DeviceType,
                    Name = deviceInfo.Name,
                    IsOnline = 0,
                    RunningState = 0,
                    StateGenTime = DateTime.Now,
                    FaultConditions = $""
                };


                return SendMesStatusData(offline);

            }



            var statusInfo = new Dto.DeviceStatusInfo
            {
                Barcode = barcode,
                PdaNo = deviceInfo.PdaNo,
                DeviceCode = deviceInfo.DeviceCode,
                DeviceType = deviceInfo.DeviceType,
                Name = deviceInfo.Name,
                IsOnline = 1,
                RunningState = devStatus,
                StateGenTime = DateTime.Now,
                FaultConditions = $"{devStatus}"
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


        public override void Start(MonitorContext monitorContext)
        {

            base.Start(monitorContext);

            this.cmtMcClient.SubFinishSignal(this.face, (face, paylod) =>
            {

                var 信号 = Convert.ToInt32(paylod);

                logger.LogInformation($"{devicetype}设备{deviceName}({deviceCode}),工作面:{face},收到完成信号:{信号}。");

                if (CheckFinished(信号))
                {
                    var result = HandleFinishedStatus(this.code, new DeviceInfo
                    {
                        PdaNo = this.device.PdaNo,
                        DeviceCode = this.device.DeviceCode,
                        DeviceType = this.device.DeviceType,
                        Name = this.device.Name
                    }, paylod);

                    logger.LogInformation($"{devicetype}设备:{deviceName}({deviceCode}),条码:{code},的信号值为{paylod},加工完成，完成发送MES数据:{result}");

                    // 通知处理完成，进行后续处理，比如更新数据库状态，或者触发其他业务逻辑等
                    if (result)
                    {
                        this.mediator.Publish(new CompleteProcesRequest()
                        {
                            Code = code,
                            Device = device
                        });
                    }

                    //停止监控任务
                    Task.Run(async () =>
                    {
                        this.cmtMcClient.UnSubFinishSignal(face);
                        await this.Stop();
                    });
                }
            });

        }

        //public override Task Stop()
        //{
        //    Task.Run(() =>
        //    {
        //        this.cmtMcClient.UnSubFinishSignal(this.face);
        //    });
        //    return base.Stop();
        //}


    }
}
