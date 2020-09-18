using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Core.Event
{
    public class CardLinkedEvent
    {
        public int CatalogId { get; set; }
        public int CardId { get; set; }
        public int Version { get; set; }
        public bool IsLinked { get; set; }
    }
}
