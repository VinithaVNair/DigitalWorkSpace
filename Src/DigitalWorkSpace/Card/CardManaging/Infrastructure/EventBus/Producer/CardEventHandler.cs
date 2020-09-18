using CardManaging.Core.Event;
using CardManaging.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CardManaging.Infrastructure.EventBus.Producer
{
    public class CardEventHandler : ICardEventHandler
    {
        private readonly ProducerConfig _producerConfig;
        private readonly ILogger<CardEventHandler> _logger;

        public CardEventHandler(ProducerConfig producerConfig, ILogger<CardEventHandler> logger)
        {
            _producerConfig = producerConfig;
            _logger = logger;
        }
        public void Raise(CardEditedEvent cardEdited)
        {
            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                var eventMessage = JsonConvert.SerializeObject(cardEdited);
                _logger.LogInformation("Card edited event raised for cardId {cardId} and version {cardVersion}", cardEdited.Id, cardEdited.OldVersion);
                producer.ProduceAsync(Environment.GetEnvironmentVariable("CardEdited"), new Message<Null, string> { Value = eventMessage });
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }

        public void Raise(UrlLinkedEvent urlLinked)
        {
            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                var eventMessage = JsonConvert.SerializeObject(urlLinked);
                _logger.LogInformation("Url linked event raised for url {shortUrl}", urlLinked.Url);
                producer.ProduceAsync(Environment.GetEnvironmentVariable("UrlLinked"), new Message<Null, string> { Value = eventMessage });
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
