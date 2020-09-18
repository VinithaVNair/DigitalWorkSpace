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
    public  class AdminApiTest
    {
        private Mock<ICardEventHandler> _cardEventHandlerMock;
        private Mock<ICatalogRepository> _catalogRepositoryMock;
        private Mock<ILogger<AdminsController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _cardEventHandlerMock = new Mock<ICardEventHandler>();
            _catalogRepositoryMock = new Mock<ICatalogRepository>();
            _loggerMock = new Mock<ILogger<AdminsController>>();
        }

        [Test]
        public void ShouldRetunSuccessWhenAdminAddedByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var userId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new AdminDto
            {
                UserId = userId,
                AdminId =adminUserId
            };
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId });
            _catalogRepositoryMock.Setup(v => v.AddAdmin(It.Is<Admin>(v=>v.Id== userId && v.CatalogId==catalogId))).Returns(new Admin(catalogId,1));


            var adminsController = new AdminsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _loggerMock.Object);

            //Act
            var response = adminsController.AddAdmin(input, catalogId);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldForbidWhenNonAdminTriesToAddAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var userId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new AdminDto
            {
                UserId = userId,
                AdminId = adminUserId
            };
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { userId });
            
            var adminsController = new AdminsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _loggerMock.Object);

            //Act
            var response = adminsController.AddAdmin(input, catalogId);

            //Assert
            Assert.IsNotNull(response.Result as ForbidResult);
        }

        [Test]
        public void ShouldRetunNotFoundWhenAdditionRequestIsMadeOnInvalidCatalog()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var userId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new AdminDto
            {
                UserId = userId,
                AdminId = adminUserId
            };
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            
            var adminsController = new AdminsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _loggerMock.Object);

            //Act
            var response = adminsController.AddAdmin(input, 2);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, (response.Result as NotFoundResult).StatusCode);
        }

        [Test]
        public void ShouldRetunSuccessWhenAdminDeletedByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var userId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new AdminDto
            {
                UserId = userId,
                AdminId = adminUserId
            };
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { adminUserId,userId });
            _catalogRepositoryMock.Setup(v => v.DeleteAdmin(It.Is<Admin>(v => v.Id == userId && v.CatalogId == catalogId))).Returns(true);
            _catalogRepositoryMock.Setup(v => v.GetAdmin(userId,catalogId)).Returns(new Admin(catalogId,userId));


            var adminsController = new AdminsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _loggerMock.Object);

            //Act
            var response = adminsController.DeleteAdmin(input, catalogId);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.OK, (response.Result as OkObjectResult).StatusCode);
        }

        [Test]
        public void ShouldRetunForBiddenWhenDeletionRequestIsnotByAdmin()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var userId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new AdminDto
            {
                UserId = userId,
                AdminId = adminUserId
            };
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            _catalogRepositoryMock.Setup(v => v.GetAllAdminIds(catalogId)).Returns(new List<int> { userId });
            
            var adminsController = new AdminsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _loggerMock.Object);

            //Act
            var response = adminsController.DeleteAdmin(input, catalogId);

            //Assert
            Assert.IsNotNull(response.Result as ForbidResult);
        }

        [Test]
        public void ShouldRetunNotFoundWhenDeletionRequestIsOnInvalidCatalog()
        {
            //Arrange
            var catalogId = 1;
            var adminUserId = 2;
            var userId = 1;
            var catalogForDb = new Catalog(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object);
            catalogForDb.Id = catalogId;
            var input = new AdminDto
            {
                UserId = userId,
                AdminId = adminUserId
            };
            _catalogRepositoryMock.Setup(v => v.GetCatalog(catalogId)).Returns(catalogForDb);
            
            var adminsController = new AdminsController(_catalogRepositoryMock.Object, _cardEventHandlerMock.Object, _loggerMock.Object);

            //Act
            var response = adminsController.DeleteAdmin(input, 2);

            //Assert
            Assert.AreEqual((int)HttpStatusCode.NotFound, (response.Result as NotFoundResult).StatusCode);
        }
    }
}
