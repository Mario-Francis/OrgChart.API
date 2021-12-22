using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrgChart.API.DTOs;
using OrgChart.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrgChart.API.BackgroundServices
{
    public class ReportBackgroundService : IHostedService, IDisposable
    {
        private long executionCount = 0;
        private Timer _timer;
        private readonly ILogger<ReportBackgroundService> logger;
        private readonly IOptionsMonitor<AppSettings> appSettingsDelegate;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ReportBackgroundService(ILogger<ReportBackgroundService> logger,
            IOptionsMonitor<AppSettings> appSettingsDelegate,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.appSettingsDelegate = appSettingsDelegate;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Report background service started running.");

            var interval = appSettingsDelegate.CurrentValue.ReportServiceExecutionInterval;
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(interval));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (appSettingsDelegate.CurrentValue.ReportServiceEnabled)
            {
                _ = DoWorkAsync(state);
            }
        }
        private async Task DoWorkAsync(object state)
        {
            if (executionCount > 1000000000) executionCount = 0;
            var count = Interlocked.Increment(ref executionCount);
            logger.LogInformation("Report background service started executing task {Count}", count);
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
                    await reportService.SendUnclaimedEmployeesReport();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            finally
            {
                logger.LogInformation("Report background service completed task {Count}", count);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Report background service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
