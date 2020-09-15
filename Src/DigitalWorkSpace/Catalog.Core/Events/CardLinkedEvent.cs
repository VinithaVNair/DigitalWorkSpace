using CatalogManaging.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogManaging.Core.Events
{
    public class CardLinkedEvent : ICardOperations
    {
        public int CatalogId { get ; private set; }
        public int CardId { get ; private set ; }
        public int Version { get; set; }
        public bool IsLinked { get ; private set; }
        public CardLinkedEvent(int catalogId,int cardId,int version)
        {
            CatalogId = catalogId;
            Version = version;
            CardId = cardId;
            IsLinked = true;
        }
    }
}
