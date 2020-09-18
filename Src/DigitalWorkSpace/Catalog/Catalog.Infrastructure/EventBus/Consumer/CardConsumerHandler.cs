using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model.CatalogAggregate;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatalogManaging.Infrastructure.EventBus.Consumer
{
    /// <summary>
    /// curently it just by passes the message but this needs
    /// to be enhanced as pub sub so that number of consumers can be reduced
    /// </summary>
    public class CardConsumerHandler : IEventConsumerHandler
    {
        private readonly ICatalogRepository _catalogRepository;
        public CardConsumerHandler(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }
        public void Handle(ConsumeResult<Null, string> message)
        {
            CardEditEventDto cardEvent = JsonConvert.DeserializeObject<CardEditEventDto>(message.Message.Value);
            var pendingCards = new List<PendingCard>();
            var catalogs = _catalogRepository.GetCatalogLinkedToCards(cardEvent.Id, cardEvent.OldVersion);
            foreach (var catalogId in catalogs)
            {
                var pendingCard = new PendingCard(cardEvent.Id, catalogId, cardEvent.Version);
                pendingCards.Add(pendingCard);
            }
            if (pendingCards.Count() > 0)
            {
                _catalogRepository.AddPendingCard(pendingCards);
            }
        }
    }
}
