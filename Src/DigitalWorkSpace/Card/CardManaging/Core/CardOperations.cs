using CardManaging.Core.Contracts;
using CardManaging.Core.Event;
using CardManaging.Core.Events;
using CardManaging.Core.Model;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CardManaging.Core
{
    public class CardOperations : ICardOperations
    {
        private readonly ICardContext _cardContext;
        private readonly ICardEventHandler _cardEventHandler;
        private readonly ILogger<CardOperations> _logger;

        public CardOperations(ICardContext cardContext,ICardEventHandler cardEventHandler, ILogger<CardOperations> logger)
        {
            _cardContext = cardContext;
            _cardEventHandler = cardEventHandler;
            _logger = logger;
        }

        public Card CreateCard(Card card)
        {
            var cards = _cardContext.Card;
            int id = cards.Any() ? cards.Max(d => d.Id) + 1 : 1;

            var newCard = new Card(card)
            {
                Id = id
            };
            _cardContext.Card.Add(newCard);
            _cardContext.SaveChanges();

            _logger.LogInformation("Card created with {id} and {version}", newCard.Id, newCard.Version);
            return newCard;
        }

        public void LinkToCatalog(CardLinkedEvent cardLinkedEvent)
        {
            var card = _cardContext.Card.Where(d => d.Id == cardLinkedEvent.CardId && d.Version == cardLinkedEvent.Version).FirstOrDefault();
            if(card==null)
            {
                _logger.LogError("No card exists with {id} and {version}", cardLinkedEvent.CardId, cardLinkedEvent.Version);
                return;
            }

            var cardtoBeLinked = new LinkedCard(cardLinkedEvent.CardId, cardLinkedEvent.Version, cardLinkedEvent.CatalogId);
            _cardContext.LinkedCard.Add(cardtoBeLinked);
            
            if(!card.IsLinked)
            {
                card.IsLinked = true;
                _cardEventHandler.Raise(new UrlLinkedEvent(card.ShortUrl, true));
            }
            _cardContext.SaveChanges();
        }

        public void UnLinkCatalog(CardLinkedEvent cardLinkedEvent)
        {
            var cardtoBeUnLinked = _cardContext.LinkedCard.Where(d => d.Id == cardLinkedEvent.CardId && d.Version == cardLinkedEvent.Version &&d.CatalogId==cardLinkedEvent.CatalogId).FirstOrDefault();
            if(cardtoBeUnLinked == null)
            {
                _logger.LogError("No card exists with {id} and {version}", cardLinkedEvent.CardId, cardLinkedEvent.Version);
                return;
            }
            _cardContext.LinkedCard.Remove(cardtoBeUnLinked);
            _cardContext.SaveChanges();
            
            var linkedCard = _cardContext.LinkedCard.Where(d => d.Id == cardLinkedEvent.CardId);
            if (!linkedCard.Any())
            {
                var card = _cardContext.Card.Where(d => d.Id == cardLinkedEvent.CardId && d.Version == cardLinkedEvent.Version).FirstOrDefault();
                card.IsLinked = false;
                _cardContext.SaveChanges();
                
                _logger.LogInformation("No card is linked to any catalog with {shortUrl} any more hence raising an event of short url unlinking", card.ShortUrl);
                _cardEventHandler.Raise(new UrlLinkedEvent(card.ShortUrl, false));
            }

        }
        public Card EditCard(EditedCard edittedCard)
        {
            var parentCard = _cardContext.Card.Where(d => d.Id == edittedCard.Id && d.Version==edittedCard.Version).FirstOrDefault();

            if(parentCard==null)
            {
                throw new ArgumentException("Edit operation can not be performd as the card doesnt exist");
            }

            int version = _cardContext.Card.Where(d => d.Id == edittedCard.Id).Max(v => v.Version)+1;
            var clonedCard = new Card(parentCard, version)
            {
                Title = edittedCard.Title,
                Description = edittedCard.Description,
                ImageContent = edittedCard.ImageContent,
                Favicon = edittedCard.Favicon,
            };
            _cardContext.Card.Add(clonedCard);
            _cardContext.SaveChanges();

            _logger.LogInformation("Raising a cardEdited event");
            _cardEventHandler.Raise(new CardEditedEvent(clonedCard,parentCard.Version));
            return clonedCard;
        }

        public IList<Card> GetAllCards()
        {
            return _cardContext.Card.ToList();
        }

        public Card GetCard(int id)
        {
            return _cardContext.Card.Where(v => v.Id == id).FirstOrDefault();
        }

        public IList<Card> GetCards(int id, int version)
        {
            if(version==0)
            {
                return _cardContext.Card.Where(v => v.Id == id).ToList();
            }
            return _cardContext.Card.Where(v => v.Id == id && v.Version==version).ToList();
        }

        public void DeleteCard(string shortUrl)
        {
            var cards = _cardContext.Card.Where(d => d.ShortUrl == shortUrl).ToList();
            if(cards==null)
            {
                _logger.LogInformation("No card is linked to the short url {url}", shortUrl);
                return;
            }
            _cardContext.Card.RemoveRange(cards);
            _cardContext.SaveChanges();
        }

        public IList<Card> GetLinkedCards(Dictionary<int, int> cards)
        {
            return _cardContext.Card.Where(d => cards.Keys.Contains(d.Id) && cards.Values.Contains(d.Version)).ToList();
        }
    }
}
