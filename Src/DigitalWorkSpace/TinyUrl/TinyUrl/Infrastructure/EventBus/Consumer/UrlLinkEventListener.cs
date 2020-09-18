using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrlManaging.Core;
using UrlManaging.Core.Contracts;
using UrlManaging.Core.Event;

namespace UrlManaging.Infrastructure.EventBus.Consumer
{
    public class UrlLinkEventListener : BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private IConsumer<Null, string> _consumer;
        public UrlLinkEventListener(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            InitializeConsumer();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_consumer == null || _consumer.Subscription == null || !_consumer.Subscription.Any())
                {
                    InitializeConsumer();
                }
                ListenMessage();
                await Task.Delay(10000, stoppingToken);//10 seconds
            }
        }

        private void InitializeConsumer()
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("Producer"),
                GroupId = Environment.GetEnvironmentVariable("GroupId"),
                AllowAutoCreateTopics = true,
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            try
            {
                _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
                _consumer.Subscribe(Environment.GetEnvironmentVariable("UrlLinked"));
            }
            catch (Exception ex)
            {
                var scope = _serviceScopeFactory.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<UrlLinkEventListener>>();
                logger.LogInformation("Subcription to event broker failed {0}", ex.Message);
            }

        }

        public void ListenMessage()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var message = _consumer.Consume(10000);
                if (message != null)
                {
                    var urlOperations = scope.ServiceProvider.GetRequiredService<ITinyUrlOperations>();
                    UrlLinkedEvent linkedUrl = JsonConvert.DeserializeObject<UrlLinkedEvent>(message.Message.Value);
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UrlLinkedEvent>>();
                    if (linkedUrl.IsLinked)
                    {
                        logger.LogInformation("Url Linked event recieved for {linkedUrl}", linkedUrl.Url);
                        urlOperations.LinkUrl(linkedUrl.Url);
                    }
                    else
                    {
                        logger.LogInformation("Url unLinked event recieved for {linkedUrl}", linkedUrl.Url);
                        urlOperations.UnLinkUrl(linkedUrl.Url);
                    }
                    _consumer.Commit(message);
                }
            }
        }
    }
}
