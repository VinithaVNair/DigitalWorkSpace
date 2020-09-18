using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogManaging.Model
{
    /// <summary>
    /// Input for adding or deleting the admin
    /// </summary>
    public class AdminDto
    {
        /// <summary>
        /// Admin who is performing the action 
        /// </summary>
        [Required]
        public int AdminId { get; set; }

        /// <summary>
        /// User who needs to be added as admin
        /// </summary>
        [Required]
        public int UserId { get; set; }
    }
}
