using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogManaging.Infrastructure.EventBus.Consumer
{
    public interface IEventConsumerHandler
    {
        void Handle(ConsumeResult<Null,string> message);
    }
}
