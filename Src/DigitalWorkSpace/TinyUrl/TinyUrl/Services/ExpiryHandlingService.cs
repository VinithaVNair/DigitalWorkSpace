
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrlManaging.Core.Contracts;

namespace UrlManaging.Services
{
    public class ExpiryHandlingService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ExpiryHandlingService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                RemoveExpiredDeals();
                await Task.Delay(600000, stoppingToken);//10 minutes
            }
        }

        private void RemoveExpiredDeals()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var urlOperations = scope.ServiceProvider.GetRequiredService<ITinyUrlOperations>();
                var expiredUrls = urlOperations.UrlsExpired(DateTime.Now);
                urlOperations.DeleteUrls(expiredUrls);
            }
        }
    }
}
