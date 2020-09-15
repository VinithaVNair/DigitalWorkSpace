using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogManaging.Model
{
    /// <summary>
    /// Input for creating a Catalog
    /// </summary>
    public class CatalogCreationDto
    {
        /// <summary>
        /// User performing the action 
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Name of the catalog
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CatalogName { get; set; }
    }
}
