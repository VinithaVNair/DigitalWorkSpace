using AutoMapper;
using CatalogManaging.Controllers;
using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;


namespace CatalogManaging.Tests
{
    public class CatalogCardsApiTest
    {
        private Mock<ICardEventHandler> _cardEventHandlerMock;
        private Mock<ICatalogRepository> _catalogRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<CardsController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _cardEventHandlerMock = new Mock<ICardEventHandler>();
            _catalogRepositoryMock = new Mock<ICatalogRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CardsController>>();
        }

        [Test]
        public void ShouldRetunSuccessWhenCardsAddedByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = adminUserId,
                CardId = 1,
                CardVersion = 2
            };
            var cards = new List<Card> { new Card(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<Card>>(It.IsAny<IEnumerable<CardDto>>())).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId });
            _catalogRepositoryMock.Setup(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id && c.First().Version == cards.First().Version))).Returns(cards);


            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.AddCards(new List<CardDto> { input }, catalogId);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldForbidWhenCardsAddingRequestIsNotDoneByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = 1,
                CardId = 1,
                CardVersion = 2
            };
            var cards = new List<Card> { new Card(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<Card>>(It.IsAny<IEnumerable<CardDto>>())).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId });
            
            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.AddCards(new List<CardDto> { input }, catalogId);

            //Assert
            Assert.IsNotNull(response.Result as ForbidResult);
        }

        [Test]
        public void ShouldReturnNotFoundWhenCardsAddingCardsRequestIsMadeOnInvalidCatalog()
        {
            //Arrange
            var catalogId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = 1,
                CardId = 1,
                CardVersion = 2
            };
            var cards = new List<Card> { new Card(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<Card>>(It.IsAny<IEnumerable<CardDto>>())).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            
            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.AddCards(new List<CardDto> { input }, 2);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, (response.Result as NotFoundResult).StatusCode);
        }


        [Test]
        public void ShouldRetunSuccessWhenCardsDeletedByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = adminUserId,
                CardId = 1,
                CardVersion = 2
            };
            var cards = new List<Card> { new Card(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<Card>>(It.IsAny<IEnumerable<CardDto>>())).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id), catalogId)).Returns(cards);

            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId });
            _catalogRepositoryMock.Setup(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id && c.First().Version == cards.First().Version))).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.DeleteCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id && c.First().Version == cards.First().Version))).Returns(true);


            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.DeleteCard(new List<CardDto> { input }, catalogId);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldForbidWhenCardsDeletionRequestIsNotDoneByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = 1,
                CardId = 1,
                CardVersion = 2
            };
            var cards = new List<Card> { new Card(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<Card>>(It.IsAny<IEnumerable<CardDto>>())).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId });
            
            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.DeleteCard(new List<CardDto> { input }, catalogId);

            //Assert
            Assert.IsNotNull(response.Result as ForbidResult);
        }

        [Test]
        public void ShouldReturnNotFoundWhenCardsDeletingCardsRequestIsMadeOnInvalidCatalog()
        {
            //Arrange
            var catalogId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = 1,
                CardId = 1,
                CardVersion = 2
            };
            var cards = new List<Card> { new Card(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<Card>>(It.IsAny<IEnumerable<CardDto>>())).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);

            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.DeleteCard(new List<CardDto> { input }, 2);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, (response.Result as NotFoundResult).StatusCode);
        }


        [Test]
        public void ShouldRetunSuccessWhenEditIsMadeByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = adminUserId,
                CardId = 1,
                CardVersion = 2
            };
            var cards = new List<Card> { new Card(input.CardId, input.CardVersion) };
            var pendingCards = new List<PendingCard> { new PendingCard(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<PendingCard>>(It.IsAny<IEnumerable<CardDto>>())).Returns(pendingCards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetPendingCards(It.Is<IList<PendingCard>>(c => c.First().Id == pendingCards.First().Id), catalogId)).Returns(pendingCards);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId });
            _catalogRepositoryMock.Setup(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id && c.First().Version == cards.First().Version))).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.GetOriginalCards(catalogId,It.Is<IList<PendingCard>>(c => c.First().Id == pendingCards.First().Id))).Returns(cards);
            _catalogRepositoryMock.Setup(v => v.DeleteCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id && c.First().Version == cards.First().Version))).Returns(true);


            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.EditCard(new List<CardDto> { input }, catalogId);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldReturnNotFoundWhenEditIsCalledForInvalidCatalog()
        {
            //Arrange
            var catalogId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = 1,
                CardId = 1,
                CardVersion = 2
            };
            var pendingCards = new List<PendingCard> { new PendingCard(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<PendingCard>>(It.IsAny<IEnumerable<CardDto>>())).Returns(pendingCards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);

            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.EditCard(new List<CardDto> { input }, 2);


            //Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, (response.Result as NotFoundResult).StatusCode);
        }

        [Test]
        public void ShouldRetunForbidWhenEditIsNotMadeByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new CardDto
            {
                UserId = 1,
                CardId = 1,
                CardVersion = 2
            };
            var pendingCards = new List<PendingCard> { new PendingCard(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<PendingCard>>(It.IsAny<IEnumerable<CardDto>>())).Returns(pendingCards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetPendingCards(It.Is<IList<PendingCard>>(c => c.First().Id == pendingCards.First().Id), catalogId)).Returns(pendingCards);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId });
            _catalogRepositoryMock.Setup(v => v.DeletePendingCard(It.Is<IList<PendingCard>>(c => c.First().Id == pendingCards.First().Id && c.First().Version == pendingCards.First().Version))).Returns(true);

            var cardsController = new CardsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = cardsController.EditCard(new List<CardDto> { input }, catalogId);

            //Assert
            Assert.IsNotNull((response.Result as ForbidResult));
        }
    }
}
