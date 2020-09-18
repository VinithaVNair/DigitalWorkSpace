using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlManaging.Core.Contracts;

namespace UrlManaging.Infrastructure.EventBus.Producer
{
    public class UrlExpiredEventHandler : IUrlExpiredEventHandler
    {
        private readonly ProducerConfig _producerConfig;
        private readonly ILogger<UrlExpiredEventHandler> _logger;
        public UrlExpiredEventHandler(ILogger<UrlExpiredEventHandler> logger)
        {
            var prconfig = new Dictionary<string, string>
            {
                {"bootstrap.servers",Environment.GetEnvironmentVariable("Producer") }
            };
            _producerConfig = new ProducerConfig(prconfig);
            _logger = logger;
        }

        public void Raise(IList<string> expiredUrl)
        {
            foreach (var url in expiredUrl)
            {
                Raise(url);
            }
        }

        public void Raise(string expiredUrl)
        {
            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                var eventMessage = JsonConvert.SerializeObject(expiredUrl);
                producer.ProduceAsync(Environment.GetEnvironmentVariable("UrlDeleted"), new Message<Null, string> { Value = eventMessage });
                _logger.LogInformation("Url deletion event raised {shortUrl}", expiredUrl);
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
