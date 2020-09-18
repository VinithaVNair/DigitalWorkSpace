using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Infrastructure.EventBus.Consumer;
using Confluent.Kafka;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatalogManaging.Tests
{
    [TestFixture]
    public class ConsumerHandlerTests
    {
        private Mock<ICatalogRepository> _catalogRepoMock;
        private IEventConsumerHandler _eventConsumerHandler;

        [SetUp]
        public void Setup()
        {
            _catalogRepoMock = new Mock<ICatalogRepository>();
        }

        [Test]
        public void OnMessageArrivalQueueForApproval()
        {
            //Arrange
            var catalogsLinked = 1;
            var oldVersion = 1;
            var newVersion = 2;
            var cardId = 1;
            _eventConsumerHandler = new CardConsumerHandler(_catalogRepoMock.Object);
            ConsumeResult<Null, string> consumerResult = new ConsumeResult<Null, string>();
            var message = new Message<Null, string>();
            message.Value = "{id:" + cardId + ",oldversion:" + oldVersion+",version:"+newVersion+"}";
            consumerResult.Message = message;
            _catalogRepoMock.Setup(v=>v.GetCatalogLinkedToCards(cardId,oldVersion)).Returns(new List<int> {catalogsLinked});

            //Act
            _eventConsumerHandler.Handle(consumerResult);

            //Assert
            _catalogRepoMock.Verify(v => v.AddPendingCard(It.Is<IList<PendingCard>>(p => p.First().Id == cardId && p.First().Version == newVersion)));

        }
    }
}
