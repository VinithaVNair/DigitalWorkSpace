using CatalogManaging.Core.Model;
using CatalogManaging.Core.Model.CatalogAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatalogManaging.Core.Contracts
{
    public interface ICatalogRepository
    {
        Admin AddAdmin(Admin admin);
        bool DeleteAdmin(Admin admin);
        IList<int> GetAllAdminIds(int catalogId);
        IList<Admin> GetAllAdmin(int catalogId);
        IList<Card> GetOriginalCards(int catalogId, IList<PendingCard> cards);
        Card GetCard(int catalogId, int cardId);
        IList<Card> GetAllCards(int catalogId);
        IList<PendingCard> AddPendingCard(IList<PendingCard> pendingCard);
        bool DeletePendingCard(IList<PendingCard> pendingCard);
        IList<Card> AddCards(IList<Card> cards);
        bool DeleteCards(IList<Card> card);
        IList<Catalog> GetCatalogs();
        Catalog GetCatalog(int catalogId);
        IList<int> GetCatalogLinkedToCards(int id,int version);
        Catalog AddCatalog(Catalog catalog);
        IList<PendingCard> GetAllUnApprovedCards(int catalogId);
        IList<PendingCard> GetPendingCards(IList<PendingCard> pendingCards, int catalogId);
        Admin GetAdmin(int id, int catalogId);
        IList<Card> GetCards(IList<Card> cards, int catalogId);

    }
}
