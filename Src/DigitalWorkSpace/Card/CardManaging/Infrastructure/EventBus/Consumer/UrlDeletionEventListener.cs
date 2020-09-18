using CardManaging.Core.Contracts;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardManaging.Infrastructure.EventBus.Consumer
{
    public class UrlDeletionEventListener : BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private IConsumer<Null, string> _consumer;
        public UrlDeletionEventListener(IServiceScopeFactory serviceScopeFactory)
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
                await Task.Delay(10000, stoppingToken);//10 minutes
            }
        }

        private void InitializeConsumer()
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("Producer"),
                GroupId = Environment.GetEnvironmentVariable("UrlGroupId"),
                AllowAutoCreateTopics = true,
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            try
            {
                _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
                _consumer.Subscribe(Environment.GetEnvironmentVariable("UrlDeleted"));
            }
            catch (Exception ex)
            {
                var scope = _serviceScopeFactory.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<UrlDeletionEventListener>>();
                logger.LogInformation("Subcription to event broker failed {0}", ex.Message);
            }
        }

        private void ListenMessage()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var message = _consumer.Consume(10000);
                if (message != null)
                {
                    string shortUrl = JsonConvert.DeserializeObject<string>(message.Message.Value);
                    var cardOperations = scope.ServiceProvider.GetRequiredService<ICardOperations>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UrlDeletionEventListener>>();
                    logger.LogInformation("Url deletion event recieved {shortUrl}",shortUrl);
                    cardOperations.DeleteCard(shortUrl);

                    _consumer.Commit(message);
                }

            }
        }
    }
}