using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogManaging.Core.Contracts
{
    public interface ICardOperations
    {
        public int CatalogId { get; }
        public int CardId { get; }
        public bool IsLinked { get;}
        public int Version { get;  }
    }
}
