using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogManaging.Model
{
    /// <summary>
    /// Input for adding or deleting card from catalog
    /// </summary>
    public class CardDto
    {
        /// <summary>
        /// User who is perfroming the action 
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Card id which needs to be added 
        /// </summary>
        [Required]
        public int CardId { get; set; }

        /// <summary>
        /// Version of the card
        /// </summary>
        [Required]
        public int CardVersion { get; set; }
    }
}
