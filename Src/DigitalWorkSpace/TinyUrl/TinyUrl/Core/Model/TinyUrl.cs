using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlManaging.Core.Model
{
    /// <summary>
    /// Tiny Url information
    /// </summary>
    public class TinyUrl
    {
        /// <summary>
        /// Tiny url representing original url
        /// </summary>
        public string ShortUrl { get; set; }
        /// <summary>
        /// original url
        /// </summary>
        public string OriginalUrl { get; set; }
        /// <summary>
        /// Expiry date of the url
        /// </summary>
        public DateTime Expiry { get; set; }
        /// <summary>
        /// Linking information of url
        /// </summary>
        public bool IsLinked { get; set; }
    }
}
