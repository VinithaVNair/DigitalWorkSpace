using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogManaging.Core.Contracts
{
    public interface ICardEventHandler
    {
        void Raise(ICardOperations cardOperations);
    }
}
