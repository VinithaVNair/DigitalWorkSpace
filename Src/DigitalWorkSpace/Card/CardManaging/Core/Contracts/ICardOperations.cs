using CardManaging.Core.Event;
using CardManaging.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Core.Contracts
{
    public interface ICardOperations
    {
        Card CreateCard(Card card);
        void LinkToCatalog(CardLinkedEvent cardLinkedEvent);
        void UnLinkCatalog(CardLinkedEvent cardLinkedEvent);
        Card EditCard(EditedCard card);
        IList<Card> GetAllCards();
        Card GetCard(int id);
        IList<Card> GetCards(int id,int version);
        IList<Card> GetLinkedCards(Dictionary<int,int> cards);
        void DeleteCard(string shortUrl);
    }
}
