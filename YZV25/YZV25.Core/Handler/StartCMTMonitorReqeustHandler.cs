using HslCommunication.MQTT;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using YZV25.Core.Monitor;
using YZV25.Entity;
using YZV25.Handler;
using YZV25.Monitor;
using YZV25.Utils;

namespace YZV25.Handler
{
    public class StartCMTMonitorReqeustHandler : INotificationHandler<StartCMTMonitorReqeust>
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly MqttClient _mqttClient;

        private readonly MesRpcClient _mesRpcClient;

        public StartCMTMonitorReqeustHandler(IServiceScopeFactory serviceScopeFactory, MqttClient mqttClient, MesRpcClient mesRpcClient)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mqttClient = mqttClient;
            _mesRpcClient = mesRpcClient;
        }
        public Task Handle(StartCMTMonitorReqeust notification, CancellationToken cancellationToken)
        {

            var device = notification.Device;

            var singalPoint = notification.FinishSignalPoint;
            var statusSignalPoint = notification.StatusSignalPoint;


            Task.Run(() =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                MonitorContext monitorContext = new MonitorContext
                {
                    
                    BarCode = notification.BarCode,
                    Device = device,
                    StartSigPoint = singalPoint, //$"{device.Name}_{device.StartSignalPoint}",
                    FinishSigPoint = singalPoint,
                    StatusSigPoint = statusSignalPoint,
                    Face = notification.Face,
                    DeviceCode = notification.DeviceCode,



                };
                var cmtMonitor = scope.ServiceProvider.GetRequiredKeyedService<IMonitor>(device.Name);
                cmtMonitor.Start(monitorContext);

            }, cancellationToken);


            return Task.CompletedTask;
        }
    }

    public class StartCMTMonitorReqeust : INotification
    {

        public string DeviceCode { get; set; }
        public string BarCode { get; set; }

        public Device Device { get; set; }


        public string Face { get; set; }

        public string FinishSignalPoint { get; set; }

        public string StatusSignalPoint { get; set; }

    }
}
