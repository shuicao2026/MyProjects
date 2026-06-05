using MediatR;
using YZV25.Entity;
using YZV25.Handler;
using YZV25.Service;

namespace YZV25.Core.Handler
{
    public class CompleteProcessRequestHandle : INotificationHandler<CompleteProcesRequest>
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;
        public CompleteProcessRequestHandle(IServiceScopeFactory serviceScopeFactory)
        {
            this._serviceScopeFactory = serviceScopeFactory;



        }
        public Task Handle(CompleteProcesRequest notification, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var scanService = scope.ServiceProvider.GetRequiredService<ScanService>();
                await scanService.FinishProcess(notification.Code, notification.Device);
            },cancellationToken);
            return Task.CompletedTask;


        }
    }

    public class CompleteProcesRequest : INotification
    {
        private Device device;
        private string code;

        public Device Device { get => device; set => device = value; }
        public string Code { get => code; set => code = value; }
    }
}
