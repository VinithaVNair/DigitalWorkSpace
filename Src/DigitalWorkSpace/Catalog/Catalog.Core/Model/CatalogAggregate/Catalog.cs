using CatalogManaging.Core.Contracts;
using System;
using System.Collections.Generic;
using CatalogManaging.Core.Events;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace CatalogManaging.Core.Model.CatalogAggregate
{
    /// <summary>
    /// Catalog which contains cards of tinyUrl
    /// </summary>
    public class Catalog : Entity<int>//AggregateRoot
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly ICardEventHandler _cardEventHandler;

        /// <summary>
        /// Name of the catalog
        /// </summary>
        [Required]
        public string Name { get; set; }
        private Catalog()//for EF
        {

        }

        public Catalog(ICatalogRepository CatalogRepository,ICardEventHandler cardEventHandler)
        {
            _catalogRepository = CatalogRepository;
            _cardEventHandler = cardEventHandler;
        }

        public Catalog(string name, int userId, ICatalogRepository CatalogRepository, ICardEventHandler cardEventHandler)
        {
            _catalogRepository = CatalogRepository;
            _cardEventHandler = cardEventHandler;
            Name = name;

            var catalog= _catalogRepository.AddCatalog(this);
            Id = catalog.Id;            
           
            var admin = new Admin(Id, userId);
            _catalogRepository.AddAdmin(admin);
        }

        public static Catalog GetExistingCatalog(int CatalogId,ICatalogRepository catalogRepository, ICardEventHandler cardEventHandler)
        {
            var catalogInfo = catalogRepository.GetCatalog(CatalogId);
            if(catalogInfo==null)
            {
                return null;
            }
            var catalog = new Catalog(catalogRepository, cardEventHandler)
            {
                Id = catalogInfo.Id,
                Name = catalogInfo.Name
            };
            return catalog;
        }

        public IList<Card> EditCard(IList<PendingCard> pendingCards, int userId)
        {
            if (pendingCards == null || !pendingCards.Any())
            {
                throw new ArgumentNullException("Invalid cards");
            }

            if (IsAnAdmin(userId))
            {
                var oldCardDeleted=RemoveCorrespondingOldCard(pendingCards);

                if (oldCardDeleted)
                {
                    var cardsEdited = AddNewCardsToCatalog(pendingCards);

                    _catalogRepository.DeletePendingCard(pendingCards);
                    return cardsEdited;
                }
            }
            return null;
        }
        
        public bool RejectEditOnCard(IList<PendingCard> pendingCards, int userId)
        {
            if (pendingCards == null || !pendingCards.Any())
            {
                throw new ArgumentNullException("Invalid cards");
            }

            if (IsAnAdmin(userId))
            {
                var cardsToBeDeleted = _catalogRepository.GetPendingCards(pendingCards, Id);
                if(cardsToBeDeleted == null || !cardsToBeDeleted.Any())
                {
                    throw new ArgumentException("Invalid cards");
                }
                return _catalogRepository.DeletePendingCard(cardsToBeDeleted);
            }
            return false;
        }

        public Card GetCard(int cardId)
        {
            return _catalogRepository.GetCard(Id, cardId);
        }

        public IList<Card> GetCards()
        {
            return _catalogRepository.GetAllCards(Id);
        } 

        public bool RemoveCard(IList<Card> cards, int userId)
        {
            if(cards==null || !cards.Any())
            {
                throw new ArgumentNullException("Invalid cards");
            }

            if (IsAnAdmin(userId))
            {
                var cardsToBeDeleted=_catalogRepository.GetCards(cards,this.Id);
                if(cardsToBeDeleted ==null || !cardsToBeDeleted.Any())
                {
                    throw new ArgumentException(string.Format("Invalid cards provided"));
                }
                var deletionSuccess = _catalogRepository.DeleteCards(cardsToBeDeleted);
                if (deletionSuccess)
                {
                    cardsToBeDeleted.ToList().ForEach(d => _cardEventHandler.Raise(new CardUnlinkedEvent(Id, d.Id, d.Version)));
                    return deletionSuccess;
                }
            }
            return false;
        }

        public Admin AddAdmin(int adminId, int userId)
        {
            if (IsAnAdmin(adminId))
            {
                var newAdmin = new Admin(Id, userId);
                _catalogRepository.AddAdmin(newAdmin);
                return newAdmin;
            }
            return null;
        }

        public IList<Admin> GetAllAdmin()
        {
            return _catalogRepository.GetAllAdmin(Id);
        }

        public bool RemoveAdmin(int adminId, int  toBeDeltedAdminId)
        {
            if (IsAnAdmin(adminId))
            {
                var adminforCatalog = _catalogRepository.GetAllAdminIds(Id);
                if (adminforCatalog.Count > 1)
                {
                    var adminToBeDelete = _catalogRepository.GetAdmin(toBeDeltedAdminId, Id);
                    if(adminToBeDelete==null)
                    {
                        throw new ArgumentException(string.Format("No such admin exists {0}", adminId));
                    }
                    return _catalogRepository.DeleteAdmin(adminToBeDelete);
                }
            }
            return false;
        }

        public IList<PendingCard> GetAllUnApprovedCards()
        {
            return _catalogRepository.GetAllUnApprovedCards(Id);
        }

        public IList<Card> AddCards(IList<Card> cards, int userId)
        {
            if (cards == null || !cards.Any())
            {
                throw new ArgumentNullException("Invalid cards");
            }

            if (IsAnAdmin(userId))
            {
                if (CardAlreadyExist(cards))
                {
                    throw new InvalidOperationException("Duplicate cards are not allowed to be added");
                }
                
                foreach (var card in cards)
                {
                    card.LinkToCatalog(Id);
                }
               var addedCards= _catalogRepository.AddCards(cards);
                addedCards.ToList().ForEach(d => _cardEventHandler.Raise(new CardLinkedEvent(Id, d.Id, d.Version)));
                return cards;
            }
            return null;
        }

        #region private methods
        private bool IsAnAdmin(int userId)
        {
            var adminforCatalog = _catalogRepository.GetAllAdminIds(Id);
            return adminforCatalog.Contains(userId);
        }

        private bool RemoveCorrespondingOldCard(IList<PendingCard> pendingCards)
        {
            var oldCard = _catalogRepository.GetOriginalCards(Id, pendingCards);
            if(oldCard != null && oldCard.Any())
            {
                var deleted = _catalogRepository.DeleteCards(oldCard);
                if (deleted)
                {
                    oldCard.ToList().ForEach(d => _cardEventHandler.Raise(new CardUnlinkedEvent(Id, d.Id, d.Version)));
                    return deleted;
                }
            }
            return false;
        }

        private bool CardAlreadyExist(IList<Card> cards)
        {
            var existingCard = _catalogRepository.GetCards(cards, Id);
            return (existingCard!=null && existingCard.Any());
        }

        private IList<Card> AddNewCardsToCatalog(IList<PendingCard> pendingCards)
        {
            var newCards = new List<Card>();
            foreach (var pendingCard in pendingCards)
            {
                var card = new Card(pendingCard.Id, pendingCard.Version);
                card.LinkToCatalog(Id);
                newCards.Add(card);
            }
            var savedCards=_catalogRepository.AddCards(newCards);
            if(savedCards!=null)
            {
                savedCards.ToList().ForEach(d => _cardEventHandler.Raise(new CardLinkedEvent(Id, d.Id, d.Version)));
                return savedCards;
            }
            return null;
        }

        public static implicit operator Lazy<object>(Catalog v)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
