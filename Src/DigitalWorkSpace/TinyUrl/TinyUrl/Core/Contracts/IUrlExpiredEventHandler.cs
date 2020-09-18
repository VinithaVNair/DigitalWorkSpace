using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlManaging.Core.Event;

namespace UrlManaging.Core.Contracts
{
    public interface IUrlExpiredEventHandler
    {
        void Raise(IList<string> urlExpired);
        void Raise(string urlExpired);
    }
}
