using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Infrastructure.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CatalogManaging.Tests
{
    public class CatalogTests
    {
        private Mock<ICatalogRepository> _catalogRepoMock;
        private Mock<ICardEventHandler> _cardEventHandlerMock;
        private Catalog _catalog;
        private int _catalogId = 1;

        [SetUp]
        public void Setup()
        {
            _catalogRepoMock = new Mock<ICatalogRepository>();
            _cardEventHandlerMock = new Mock<ICardEventHandler>();
            _catalog = new Catalog(_catalogRepoMock.Object, _cardEventHandlerMock.Object);
            _catalog.Id = 1;
            _catalogRepoMock.Setup(v => v.GetCatalog(1)).Returns(_catalog);
        }

        #region domain validation

        [Test]
        public void ShouldMakeCreaterAnAdmin()
        {
            //Arrange
            var userId = 1;
            var catalogName = "Catalog";
            _catalogRepoMock.Setup(v => v.AddCatalog(It.Is<Catalog>(v => v.Name == catalogName))).Returns(_catalog);

            //Act
            var catalog = new Catalog(catalogName, userId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);

            //Assert
            _catalogRepoMock.Verify(v => v.AddAdmin(It.Is<Admin>(a => a.Id == userId && a.CatalogId == _catalog.Id)), Times.Once);
        }

        [Test]
        public void ShouldAllowOnlyAdminToAddCards()
        {
            //Arrange
            var userId = 1;
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var cards = new List<Card> { new Card(1, 1) };
            var addedCards = new List<Card> { new Card(1, 1) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id))).Returns(addedCards);


            //Act
            var cardsAddedByAdmin = catalog.AddCards(cards, adminUserId);
            var cardsAddedByUser = catalog.AddCards(cards, userId);

            //Assert
            Assert.Null(cardsAddedByUser);
            Assert.NotNull(cardsAddedByAdmin);
            Assert.IsTrue(cardsAddedByAdmin.All(v => v.CatalogId == catalog.Id));
        }

        [Test]
        public void ShouldAllowOnlyAdminToDeleteCard()
        {
            //Arrange
            var userId = 1;
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var cards = new List<Card> { new Card(1, 1) };
            var addedCards = new List<Card> { new Card(1, 1) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.GetCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id), catalog.Id)).Returns(cards);

            _catalogRepoMock.Setup(v => v.DeleteCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id))).Returns(true);


            //Act
            var deletedByAdmin = catalog.RemoveCard(cards, adminUserId);
            var deletedByUser = catalog.RemoveCard(cards, userId);

            //Assert
            Assert.IsFalse(deletedByUser);
            Assert.IsTrue(deletedByAdmin);
        }

        [Test]
        public void ShouldAllowOnlyAdminToApproveEditOnCard()
        {
            //Arrange
            var userId = 1;
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var oldcards = new List<Card> { new Card(1, 1) };
            var pendingCard = new List<PendingCard> { new PendingCard(1, 2) };
            var newCards = new List<Card> { new Card(1, 2) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.GetOriginalCards(catalog.Id, It.Is<IList<PendingCard>>(c => c.First().Id == pendingCard.First().Id))).Returns(oldcards);
            _catalogRepoMock.Setup(v => v.DeleteCards(It.Is<IList<Card>>(c => c.First().Id == oldcards.First().Id && c.First().Version == oldcards.First().Version))).Returns(true);
            _catalogRepoMock.Setup(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == pendingCard.First().Id && c.First().Version == pendingCard.First().Version))).Returns(newCards);


            //Act
            var cardsAddedByAdmin = catalog.EditCard(pendingCard, adminUserId);
            var cardsAddedByUser = catalog.EditCard(pendingCard, userId);

            //Assert
            Assert.Null(cardsAddedByUser);
            Assert.NotNull(cardsAddedByAdmin);
        }

        [Test]
        public void ShouldUpdateTheCardOnApprovalOfEdit()
        {
            //Arrange
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var oldcards = new List<Card> { new Card(1, 1) };
            var pendingCard = new List<PendingCard> { new PendingCard(1, 2) };
            var newCards = new List<Card> { new Card(1, 2) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.GetOriginalCards(catalog.Id, It.Is<IList<PendingCard>>(c => c.First().Id == pendingCard.First().Id))).Returns(oldcards);
            _catalogRepoMock.Setup(v => v.DeleteCards(It.Is<IList<Card>>(c => c.First().Id == oldcards.First().Id && c.First().Version == oldcards.First().Version))).Returns(true);
            _catalogRepoMock.Setup(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == pendingCard.First().Id && c.First().Version == pendingCard.First().Version))).Returns(newCards);


            //Act
            var cardsAddedByAdmin = catalog.EditCard(pendingCard, adminUserId);

            //Assert
            _catalogRepoMock.Verify(v => v.DeleteCards(It.Is<IList<Card>>(c => c.First().Id == oldcards.First().Id && c.First().Version == oldcards.First().Version)), Times.Once);
            _catalogRepoMock.Verify(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == pendingCard.First().Id && c.First().Version == pendingCard.First().Version)), Times.Once);

        }

        [Test]
        public void ShouldRaiseLinkAndUnlinkEventOnSuccessfullEdit()
        {
            //Arrange
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var oldcards = new List<Card> { new Card(1, 1) };
            var pendingCard = new List<PendingCard> { new PendingCard(1, 2) };
            var newCards = new List<Card> { new Card(1, 2) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.GetOriginalCards(catalog.Id, It.Is<IList<PendingCard>>(c => c.First().Id == pendingCard.First().Id))).Returns(oldcards);
            _catalogRepoMock.Setup(v => v.DeleteCards(It.Is<IList<Card>>(c => c.First().Id == oldcards.First().Id && c.First().Version == oldcards.First().Version))).Returns(true);
            _catalogRepoMock.Setup(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == pendingCard.First().Id && c.First().Version == pendingCard.First().Version))).Returns(newCards);


            //Act
            var cardsAddedByAdmin = catalog.EditCard(pendingCard, adminUserId);

            //Assert
            _cardEventHandlerMock.Verify(e => e.Raise(It.Is<ICardOperations>(c => c.CardId == pendingCard.First().Id && c.CatalogId == catalog.Id && c.Version == pendingCard.First().Version
          && c.IsLinked == true)));
            _cardEventHandlerMock.Verify(e => e.Raise(It.Is<ICardOperations>(c => c.CardId == pendingCard.First().Id && c.CatalogId == catalog.Id && c.Version == oldcards.First().Version
         && c.IsLinked == false)));

        }

        [Test]
        public void ShouldAllowOnlyAdminToRejectEditOnCard()
        {
            //Arrange
            var userId = 1;
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var pendingCard = new List<PendingCard> { new PendingCard(1, 2) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.GetPendingCards(It.Is<IList<PendingCard>>(c => c.First().Id == pendingCard.First().Id), catalog.Id)).Returns(pendingCard);
            _catalogRepoMock.Setup(v => v.DeletePendingCard(It.Is<IList<PendingCard>>(c => c.First().Id == pendingCard.First().Id && c.First().Version == pendingCard.First().Version))).Returns(true);


            //Act
            var deletedByAdmin = catalog.RejectEditOnCard(pendingCard, adminUserId);
            var deletedByUser = catalog.RejectEditOnCard(pendingCard, userId);

            //Assert
            Assert.IsTrue(deletedByAdmin);
            Assert.IsFalse(deletedByUser);

        }

        [Test]
        public void ShouldAllowOnlyAdminToAddAnotherAdmin()
        {
            //Arrange
            var userId = 1;
            var adminUserId = 2;
            var userToBeAddedId = 3;

            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.AddAdmin(It.IsAny<Admin>())).Returns(new Admin(catalog.Id, userToBeAddedId));


            //Act
            var adminAddedByAdmin = catalog.AddAdmin(adminUserId, userToBeAddedId);
            var adminAddedByUser = catalog.AddAdmin(userId, userToBeAddedId);

            //Assert
            Assert.Null(adminAddedByUser);
            Assert.NotNull(adminAddedByAdmin);
        }

        [Test]
        public void ShouldAllowOnlyAdminToRemovedAnotherAdmin()
        {
            //Arrange
            var userId = 1;
            var adminUserId = 2;
            var userToBeRemovedId = 3;

            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId, 20 });
            _catalogRepoMock.Setup(v => v.GetAdmin(userToBeRemovedId, catalog.Id)).Returns(new Admin(catalog.Id, userToBeRemovedId));
            _catalogRepoMock.Setup(v => v.DeleteAdmin(It.IsAny<Admin>())).Returns(true);


            //Act
            var adminAddedByAdmin = catalog.RemoveAdmin(adminUserId, userToBeRemovedId);
            var adminAddedByUser = catalog.RemoveAdmin(userId, userToBeRemovedId);

            //Assert
            Assert.IsFalse(adminAddedByUser);
            Assert.IsTrue(adminAddedByAdmin);
        }

        [Test]
        public void ShouldnotAllowDeletionIfOnlyOneAdminIsAvailable()
        {
            //Arrange
            var adminUserId = 2;
            var userToBeRemovedId = 2;

            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);

            //Only one admin
            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.GetAdmin(userToBeRemovedId, catalog.Id)).Returns(new Admin(catalog.Id, userToBeRemovedId));
            _catalogRepoMock.Setup(v => v.DeleteAdmin(It.IsAny<Admin>())).Returns(true);


            //Act
            var adminAddedByAdmin = catalog.RemoveAdmin(adminUserId, userToBeRemovedId);

            //Assert
            Assert.IsFalse(adminAddedByAdmin);
        }

        [Test]
        public void ShouldRaiseAnEventOnNewCardAdded()
        {
            //Arrange
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var cards = new List<Card> { new Card(1, 1) };
            var addedCards = new List<Card> { new Card(1, 1) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.AddCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id))).Returns(addedCards);


            //Act
            var cardsAddedByAdmin = catalog.AddCards(cards, adminUserId);

            //Assert
            _cardEventHandlerMock.Verify(e => e.Raise(It.Is<ICardOperations>(c => c.CardId == cards.First().Id && c.CatalogId == catalog.Id
            && c.IsLinked == true)));
        }

        [Test]
        public void ShouldRaiseAnEventOnCardRemoval()
        {
            //Arrange
            var adminUserId = 2;

            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var cards = new List<Card> { new Card(1, 1) };
            var addedCards = new List<Card> { new Card(1, 1) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.GetCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id), catalog.Id)).Returns(cards);
            _catalogRepoMock.Setup(v => v.DeleteCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id))).Returns(true);


            //Act
            var deletedByAdmin = catalog.RemoveCard(cards, adminUserId);

            //Assert
            _cardEventHandlerMock.Verify(e => e.Raise(It.Is<ICardOperations>(c => c.CardId == cards.First().Id && c.CatalogId == catalog.Id
            && c.IsLinked == false)));

        }
        #endregion

        #region input validation

        [Test]
        public void ShouldFailAddOnInvalidCardInput()
        {
            //Arrange
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);

            //Act //Assert
            Assert.Throws<ArgumentNullException>(() => catalog.AddCards(null, adminUserId));

        }

        [Test]
        public void ShouldFailRemovalOnNullCardInput()
        {
            //Arrange
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);

            //Act //Assert
            Assert.Throws<ArgumentNullException>(() => catalog.RemoveCard(null, adminUserId));

        }

        [Test]
        public void ShouldFailRemovalIfCardDoesntExist()
        {
            //Arrange
            //Arrange
            var adminUserId = 2;
            var catalog = Catalog.GetExistingCatalog(_catalogId, _catalogRepoMock.Object, _cardEventHandlerMock.Object);
            var cards = new List<Card> { new Card(1, 1) };
            var cardToBeRemove = new List<Card> { new Card(2, 2) };

            _catalogRepoMock.Setup(v => v.GetAllAdminIds(catalog.Id)).Returns(new List<int> { adminUserId });
            _catalogRepoMock.Setup(v => v.GetCards(It.Is<IList<Card>>(c => c.First().Id == cards.First().Id), catalog.Id)).Returns(new List<Card>());


            //Act //Assert
            Assert.Throws<ArgumentException>(() => catalog.RemoveCard(cardToBeRemove, adminUserId));

        }

        #endregion
    }
}
