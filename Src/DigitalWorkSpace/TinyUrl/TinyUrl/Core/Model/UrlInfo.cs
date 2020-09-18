using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UrlManaging.Core.Model
{
    /// <summary>
    /// Input for url creation
    /// </summary>
    public class UrlInfo
    {
        /// <summary>
        /// Original url for which tiny url needs to be created
        /// </summary>
        [Required]
        public string OriginalUrl { get; set; }

        /// <summary>
        /// Expiry date of the new url
        /// </summary>
        [Required]
        public DateTime Expiry { get; set; }
    }
}
