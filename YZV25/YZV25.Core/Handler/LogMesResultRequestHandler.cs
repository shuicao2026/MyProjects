using MediatR;
using YZV25.Core.Service;
using YZV25.Dto;
using YZV25.Monitor;

namespace YZV25.Core.Handler
{
    public class LogMesResultRequestHandler : INotificationHandler<LogMesResultRequest>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<LogMesResultRequestHandler> _logger;

        public LogMesResultRequestHandler(IServiceScopeFactory serviceScopeFactory,ILogger<LogMesResultRequestHandler> logger)
        {

            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        public Task Handle(LogMesResultRequest notification, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {

                try
                {

                    using var scope = _serviceScopeFactory.CreateScope();
                    var businessLogService = scope.ServiceProvider.GetRequiredService<BusinessLogService>();

                    var mesResponse = notification.Response;
                    switch (notification.MesOperation)
                    {
                        case "状态数据":
                            await businessLogService.LogMesDeviceStatus(notification.PdaNo, notification.BarCode,
                                    notification.DeviceType, notification.DeviceName, notification.DeviceCode,
                                    notification.mesEnabledStatus, mesResponse.Success, mesResponse.Message, notification.Payload);
                            break;
                        case "加工中":
                            await businessLogService.LogMesWorking(notification.PdaNo, notification.BarCode,
                                notification.DeviceType, notification.DeviceName, notification.DeviceCode,
                                notification.mesEnabledStatus, mesResponse.Success, mesResponse.Message, notification.Payload);
                            break;
                        case "加工完成":
                            await businessLogService.LogMesExit(notification.PdaNo, notification.BarCode,
                                notification.DeviceType, notification.DeviceName, notification.DeviceCode,
                                notification.mesEnabledStatus, mesResponse.Success, mesResponse.Message, notification.Payload);
                            break;
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,$"pda:{notification.PdaNo},{notification.DeviceType}设备{notification.DeviceName}({notification.DeviceCode}),插入业务日志报错:{ex.Message}");
                }


            }, cancellationToken);
            return Task.CompletedTask;
        }
    }

    public class LogMesResultRequest : INotification
    {
        public int PdaNo { get; set; }
        public string BarCode { get; set; }
        public string DeviceCode { get; set; }

        public string DeviceName { get; set; }
        public string DeviceType { get; set; }

        public object Payload { get; set; }

        public string MesOperation { get; set; }

        public bool mesEnabledStatus { get; set; }

        public MesResponse Response { get; set; }


    }
}
