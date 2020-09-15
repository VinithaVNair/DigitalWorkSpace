using CatalogManaging.Core.Contracts;
using CatalogManaging.Core.Model;
using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatalogManaging.Infrastructure
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly ICatalogContext _catalogContext;
        private readonly ILogger<CatalogRepository> _logger;

        public CatalogRepository(ICatalogContext context, ILogger<CatalogRepository> logger)
        {
            _catalogContext = context;
            _logger = logger;
        }

        public Admin AddAdmin(Admin admin)
        {
            _catalogContext.Admin.Add(admin);
            _catalogContext.SaveChanges();
            _logger.LogInformation("Added user {Id} as admin to catalog {catalogId}", admin.Id, admin.CatalogId);
            return admin;
        }

        public IList<Card> AddCards(IList<Card> cards)
        {
            _catalogContext.Card.AddRange(cards);
            _catalogContext.SaveChanges();
            _logger.LogInformation("New cards {ids} added to  catalog {catalogId}", string.Join(",", cards.Select(a => a.Id)), cards.First().CatalogId);
            return cards;
        }

        public Catalog AddCatalog(Catalog catalog)
        {
            _catalogContext.Catalog.Add(catalog);
            _catalogContext.SaveChanges();
            _logger.LogInformation("New catalog Created {catalogId}", catalog.Id);
            return catalog;
        }

        public IList<PendingCard> AddPendingCard(IList<PendingCard> pendingCard)
        {
            _catalogContext.PendingCard.AddRange(pendingCard);
            _catalogContext.SaveChanges();
            _logger.LogInformation("Added cards {Id} for admin approval on catalog {catalogId}", string.Join(",", pendingCard.Select(a => a.Id)), pendingCard.First().CatalogId);
            return pendingCard;
        }

        public bool DeleteAdmin(Admin admin)
        {
            _catalogContext.Admin.Remove(admin);
            _catalogContext.SaveChanges();
            _logger.LogInformation("Removed user {Id} from admin group of catalog {catalogId}", admin.Id, admin.CatalogId);
            return true;
        }

        public bool DeleteCards(IList<Card> cards)
        {
            _catalogContext.Card.RemoveRange(cards);
            _catalogContext.SaveChanges();
            _logger.LogInformation("Deleted cards {Id} from catalog {catalogId}", string.Join(",", cards.Select(a => a.Id)), cards.First().CatalogId);
            return true;
        }

        public bool DeletePendingCard(IList<PendingCard> pendingCards)
        {
            _catalogContext.PendingCard.RemoveRange(pendingCards);
            _catalogContext.SaveChanges();
            _logger.LogInformation("Rejected edits on cards {Id} in catalog {catalogId}", string.Join(",", pendingCards.Select(a => a.Id)), pendingCards.First().CatalogId);
            return true;
        }

        public IList<PendingCard> GetPendingCards(IList<PendingCard> pendingCards, int catalogId)
        {
            return _catalogContext.PendingCard.Where(d => catalogId == d.CatalogId && pendingCards.Select(p => p.Version).Contains(d.Version) && pendingCards.Select(p => p.Id).Contains(d.Id)).ToList();
        }

        public IList<PendingCard> GetAllUnApprovedCards(int catalogId)
        {
            return _catalogContext.PendingCard.Where(d=>d.CatalogId==catalogId).ToList();
        }

        public IList<int> GetAllAdminIds(int catalogId)
        {
            return _catalogContext.Admin.Where(d => d.CatalogId==catalogId).Select(v=>v.Id).ToList();
        }

        public IList<Admin> GetAllAdmin(int catalogId)
        {
            return _catalogContext.Admin.Where(d => d.CatalogId == catalogId).ToList();
        } 

        public IList<Card> GetAllCards(int catalogId)
        {
            return _catalogContext.Card.Where(d => d.CatalogId==catalogId).ToList();
        }

        public IList<Card> GetOriginalCards(int catalogId, IList<PendingCard> cards)
        {
            return _catalogContext.Card.Where(d => d.CatalogId == catalogId && cards.Select(c => c.Id).Contains(d.Id)).ToList();

        }

        public Card GetCard(int catalogId, int cardId)
        {
            return _catalogContext.Card.Where(d => d.CatalogId == catalogId && d.Id == cardId).FirstOrDefault();
        }

        public Catalog GetCatalog(int catalogId)
        {
            return _catalogContext.Catalog.Where(d => d.Id == catalogId).FirstOrDefault();
        }

        public IList<Catalog> GetCatalogs()
        {
            return _catalogContext.Catalog.ToList();
        }

        public IList<int> GetCatalogLinkedToCards(int id, int version)
        {
            var catalogids = _catalogContext.Card.Where(d => d.Id == id && d.Version == version).Select(d=>d.CatalogId).ToList();
            return _catalogContext.Catalog.Where(d => catalogids.Contains(d.Id)).Select(d=>d.Id).ToList();
        }

        public Admin GetAdmin(int id, int catalogId)
        {
            return _catalogContext.Admin.Where(d => d.Id == id && d.CatalogId == catalogId).FirstOrDefault();
        }

        public IList<Card> GetCards(IList<Card> cards, int catalogId)
        {
            return _catalogContext.Card.Where(c => cards.Select(d => d.Id).Contains(c.Id) && c.CatalogId == catalogId).ToList();
        }
    }
}
