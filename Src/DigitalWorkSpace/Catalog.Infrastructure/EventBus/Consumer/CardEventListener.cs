using Confluent.Kafka;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Core.Contracts;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogManaging.Infrastructure.EventBus.Consumer
{
    public class CardEventListener: BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private IConsumer<Null,string> _consumer;
        public CardEventListener(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("Producer"),
                GroupId = Environment.GetEnvironmentVariable("GroupId"),
                AllowAutoCreateTopics = true,
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            _consumer.Subscribe(Environment.GetEnvironmentVariable("CardEdited"));
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ListenMessage();
                await Task.Delay(10000, stoppingToken);//10 minutes
            }
        }

        private void ListenMessage()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                //using (var consumer = new ConsumerBuilder<Null, string>(_consumerConfig).Build())
                {
                   // consumer.Subscribe(Environment.GetEnvironmentVariable("CardEdited"));
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

            }
        }
    }
}
