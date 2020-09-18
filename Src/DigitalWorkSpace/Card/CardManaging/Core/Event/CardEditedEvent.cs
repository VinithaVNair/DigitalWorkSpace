using CardManaging.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Core.Event
{
    public class CardEditedEvent
    {
        public int Id { get; private set; }
        public int OldVersion { get; private set; }
        public int Version { get; private set; }

        public CardEditedEvent(Card card, int oldVersion)
        {
            this.Id = card.Id;
            this.Version = card.Version;
            this.OldVersion = oldVersion;
        }
    }
}
