using CardManaging.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Core.Events
{
    public interface ICardEventHandler
    {
        void Raise(CardEditedEvent CardEditedEvent);
        void Raise(UrlLinkedEvent cardEdited);

    }
}
