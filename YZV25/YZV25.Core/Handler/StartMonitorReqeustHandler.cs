using HslCommunication.MQTT;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using YZV25.Core.Monitor;
using YZV25.Entity;
using YZV25.Monitor;
using YZV25.Utils;

namespace YZV25.Handler
{
    public class StartMonitorReqeustHandler : INotificationHandler<StartMonitorReqeust>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;


        private readonly MqttClient _mqttClient;

        private readonly MesRpcClient _mesRpcClient;

        public StartMonitorReqeustHandler(IServiceScopeFactory serviceScopeFactory,
             MesRpcClient mesRpcClient,
            MqttClient mqttClient)
        {
            this._serviceScopeFactory = serviceScopeFactory;
            _mqttClient = mqttClient;
            _mesRpcClient = mesRpcClient;
        }

        public Task Handle(StartMonitorReqeust notification, CancellationToken cancellationToken)
        {
            var device = notification.Device;

            Task.Run(() =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                switch (device.DeviceType)
                {
                    case "CNC":
                        {
                            MonitorContext monitorContext = new MonitorContext
                            {
                                BarCode = notification.BarCode,
                                Device = device,
                                DeviceCode= device.DeviceCode

                            };
                            var cncMonitor = scope.ServiceProvider.GetRequiredKeyedService<IMonitor>(device.Name);
                            cncMonitor.Start(monitorContext);


                        }
                        break;
                    case "FSW":
                        {
                            MonitorContext monitorContext = new MonitorContext
                            {
                                BarCode = notification.BarCode,
                                Device = device,
                                DeviceCode = device.DeviceCode


                            };
                            var fswMonitor = scope.ServiceProvider.GetRequiredKeyedService<IMonitor>(device.Name);
                            fswMonitor.Start(monitorContext);
                        }
                        break;

                }

            },cancellationToken);
       
            return Task.CompletedTask;

        }
    }

    public class StartMonitorReqeust : INotification
    {

        public string BarCode { get; set; }

        public Device Device { get; set; }

    }
}
