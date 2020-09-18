using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogManaging.Infrastructure.EventBus.Consumer
{
    public class CardEditEventDto
    {
        public int Id { get;  set; }
        public int OldVersion { get;  set; }
        public int Version { get;  set; }
    }
}
