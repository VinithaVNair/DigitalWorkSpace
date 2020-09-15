using AutoMapper;
using CatalogManaging.Controllers;
using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CatalogManaging.Tests
{
    public class CatalogApiTest
    {
        private Mock<ICardEventHandler> _cardEventHandlerMock;
        private Mock<ICatalogRepository> _catalogRepositoryMock;
        private Mock<ILogger<CatalogsController>> _loggerMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _cardEventHandlerMock = new Mock<ICardEventHandler>();
            _catalogRepositoryMock = new Mock<ICatalogRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CatalogsController>>();
           }

        [Test]
        public void ShouldCreateCatalogWithCreatedStatus()
        {
            //Arrange
            var input = new CatalogCreationDto();
            input.CatalogName = "Catalog1";
            input.UserId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = 1;
            _catalogRepositoryMock.Setup(v => v.AddCatalog(It.Is<Catalog>(v => v.Name == input.CatalogName))).Returns(catalogForDb);

            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);
            

            //Act
            var response=catalogController.CreateCatalog(input);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.Created, (response.Result  as CreatedAtRouteResult).StatusCode);

        }

        [Test]
        public void ShouldGetCatlogsWithSuccess()
        {
            //Arrange
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = 1;
            _catalogRepositoryMock.Setup(v => v.GetCatalog(1)).Returns(catalogForDb);
            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = catalogController.GetCatalogs();

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldGetAllUnApprovedCardsWithSuccess()
        {
            //Arrange
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = 1;
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogForDb.Id)).Returns(catalogForDb);
            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = catalogController.GetApprovalPendingCards(catalogForDb.Id);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldReturnNotFoundOnInvalidCatalogToGetPendingCards()
        {
            //Arrange
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = 1;
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogForDb.Id)).Returns(catalogForDb);
            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = catalogController.GetApprovalPendingCards(2);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, (response.Result as NotFoundResult).StatusCode);
        }

        [Test]
        public void ShouldReturnCatalogBasedOnIdWithSuccess()
        {
            //Arrange
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = 1;
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogForDb.Id)).Returns(catalogForDb);
            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = catalogController.GetCatalog(catalogForDb.Id);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldReturnNotFoundOnInvalidCatalogId()
        {
            //Arrange
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = 1;
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogForDb.Id)).Returns(catalogForDb);
            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = catalogController.GetCatalog(2);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, (response.Result as NotFoundResult).StatusCode);
        }

        [Test]
        public void ShouldRetunSuccessWhenRejectOnEditIsMadeByAdmin()
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
            var pendingCards = new List<PendingCard> { new PendingCard(input.CardId, input.CardVersion) };
            _mapperMock.Setup(v => v.Map<IList<PendingCard>>(It.IsAny<IEnumerable<CardDto>>())).Returns(pendingCards);
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetPendingCards(It.Is<IList<PendingCard>>(c => c.First().Id == pendingCards.First().Id), catalogId)).Returns(pendingCards);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId });
            _catalogRepositoryMock.Setup(v => v.DeletePendingCard(It.Is<IList<PendingCard>>(c => c.First().Id == pendingCards.First().Id && c.First().Version == pendingCards.First().Version))).Returns(true);


            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = catalogController.RejectEditedCard(new List<CardDto> { input }, catalogId);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldReturnNotFoundWhenRejectOnEditIsCalledForInvalidCatalog()
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
            
            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = catalogController.RejectEditedCard(new List<CardDto> { input }, 2);


            //Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, (response.Result as NotFoundResult).StatusCode);
        }

        [Test]
        public void ShouldRetunForbidWhenRejectOnEditIsNotMadeByAdmin()
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
           
            var catalogController = new CatalogsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _mapperMock.Object, _loggerMock.Object);

            //Act
            var response = catalogController.RejectEditedCard(new List<CardDto> { input }, catalogId);

            //Assert
            Assert.IsNotNull ((response.Result as ForbidResult));
        }
    }
}
