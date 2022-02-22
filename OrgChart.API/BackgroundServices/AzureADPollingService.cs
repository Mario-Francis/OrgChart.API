
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrgChart.API.DTOs;
using OrgChart.API.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrgChart.API.BackgroundServices
{
    public class AzureADPollingService : IHostedService, IDisposable
    {
        private long executionCount = 0;
        private Timer _timer;
        private readonly ILogger<ReportBackgroundService> logger;
        private readonly IOptionsMonitor<AppSettings> appSettingsDelegate;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IMicrosoftGraphService _microsoftGraphService;
        private readonly ISharePointService _sharePointService;

        public AzureADPollingService(
            IMicrosoftGraphService microsoftGraphService,
            ISharePointService sharePointService,
            ILogger<ReportBackgroundService> logger,
            IOptionsMonitor<AppSettings> appSettingsDelegate,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            _microsoftGraphService = microsoftGraphService;
            this.logger = logger;
            this.appSettingsDelegate = appSettingsDelegate;
            this.serviceScopeFactory = serviceScopeFactory;
            _sharePointService = sharePointService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!appSettingsDelegate.CurrentValue.UpdateAzureAD)
            {
                logger.LogInformation("AzureADPolling background service started running.");

                var interval = appSettingsDelegate.CurrentValue.ReportServiceExecutionInterval;
                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                    TimeSpan.FromMinutes(interval));
            }
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (!appSettingsDelegate.CurrentValue.UpdateAzureAD)
            {
                _ = DoWorkAsync(state);
            }
        }
        private async Task DoWorkAsync(object state)
        {
            if (executionCount > 1000000000) executionCount = 0;
            var count = Interlocked.Increment(ref executionCount);
            logger.LogInformation("AzureADPolling process background service started executing task {Count}", count);
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var list = await _sharePointService.GetApprovalPendingItemLocalADSyncCompleted();
                    if (list.Any())
                    {
                        foreach (var row in list)
                        {
                            // if manager is updated in azureAD as well
                            var data = await _microsoftGraphService.GetUser(row.EmployeeEmail);

                            if (data != null && data.Manager.UserPrincipalName.ToLower() == row.ToManagerEmail.ToLower())
                            {
                                // update sharepoint list with approved status
                                await _sharePointService.UpdateApprovalItem(row.Id, ApprovalStatus.APPROVED.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            finally
            {
                logger.LogInformation("AzureADPolling background service completed task {Count}", count);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("AzureADPolling background service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
