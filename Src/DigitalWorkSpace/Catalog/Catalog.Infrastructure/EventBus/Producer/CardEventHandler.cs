using CatalogManaging.Core.Contracts;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogManaging.Infrastructure.EventBus.Producer
{
    public class CardEventHandler : ICardEventHandler
    {
        private readonly ProducerConfig _producerConfig;
        public CardEventHandler(ProducerConfig producerConfig)
        {
            _producerConfig = producerConfig;
        }
        public void Raise(ICardOperations cardOperations)
        {
            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                var eventMessage = JsonConvert.SerializeObject(cardOperations);
                producer.ProduceAsync(Environment.GetEnvironmentVariable("CardLinked"), new Message<Null, string> { Value = eventMessage });
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
