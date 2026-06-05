using MediatR;
using YZV25.Core.Monitor;
using YZV25.Entity;
using YZV25.Handler;

namespace YZV25.Core.Handler
{
    public class StopMonitorRequestHandler : INotificationHandler<StopMonitorRequest>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public StopMonitorRequestHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task Handle(StopMonitorRequest notification, CancellationToken cancellationToken)
        {

            Task.Run(() =>
            {
                using var scope = this._serviceScopeFactory.CreateScope();  
                var monitor = scope.ServiceProvider.GetRequiredKeyedService<IMonitor>(notification.Device.Name);
                monitor.Stop();
            }, cancellationToken);
        

            return Task.CompletedTask;
        }
    }

    public class StopMonitorRequest : INotification
    {
        public int PdaNo { get; set; }
        public Device Device { get; set; }

        public string Face { get; set; }

    }
}
