using CardManaging.Core.Contracts;
using CardManaging.Core.Event;
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
    public class CardEventListener : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
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
                _consumer.Subscribe(Environment.GetEnvironmentVariable("CardLinked"));
            }
            catch(Exception ex)
            {
                var scope = _serviceScopeFactory.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CardEventListener>>();
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
                    var cardOperations = scope.ServiceProvider.GetRequiredService<ICardOperations>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<CardEventListener>>();
                    
                    CardLinkedEvent card = JsonConvert.DeserializeObject<CardLinkedEvent>(message.Message.Value);

                    if (card.IsLinked)
                    {
                        logger.LogInformation("Card Linked event recieved for card Id {cardId} version {cardVersion}",card.CardId, card.Version);
                        cardOperations.LinkToCatalog(card);
                    }
                    else
                    {
                        logger.LogInformation("Card unlink event recieved for card Id {cardId} version {cardVersion}", card.CardId, card.Version);
                        cardOperations.UnLinkCatalog(card);
                    }
                    _consumer.Commit(message);
                }
            }
        }
    }
}
