using Confluent.Kafka;
using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CatalogManaging.Infrastructure.EventBus.Consumer
{
    public class CardEventListener : BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private IConsumer<Null, string> _consumer;
        public CardEventListener(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            InitializeConsumer();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_consumer == null)
                {
                    InitializeConsumer();
                }
                ListenMessage();
                await Task.Delay(10000, stoppingToken);//10 sec
            }
        }

        private void ListenMessage()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var message = _consumer.Consume(10000);
                if (message != null)
                {
                    var eventConsumerHandler = scope.ServiceProvider.GetRequiredService<IEventConsumerHandler>();
                    eventConsumerHandler.Handle(message);
                    _consumer.Commit(message);
                }
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
                _consumer.Subscribe(Environment.GetEnvironmentVariable("CardEdited"));
            }
            catch (Exception ex)
            {
                var scope = _serviceScopeFactory.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CardEventListener>>();
                logger.LogInformation("Subcription to event broker failed {0}", ex.Message);
            }
        }
    }
}
