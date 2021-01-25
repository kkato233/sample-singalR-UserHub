using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue,
            ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected async override Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception exp)
                {
                    _logger.LogError(exp,
                       $"Error occurred executing {nameof(workItem)}.");

                    // 例外を収集
                    var propInfo = new Dictionary<string, string>() {
                        { "WorkItemName", nameof(workItem) },
                    };
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}
