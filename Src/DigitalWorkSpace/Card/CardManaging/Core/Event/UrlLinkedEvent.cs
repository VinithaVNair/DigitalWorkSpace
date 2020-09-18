using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Core.Event
{
    public class UrlLinkedEvent
    {
        public string Url { get; set; }
        public bool IsLinked { get; set; }

        public UrlLinkedEvent(string url, bool isLinked)
        {
            Url = url;
            IsLinked = isLinked;
        }
    }
}
