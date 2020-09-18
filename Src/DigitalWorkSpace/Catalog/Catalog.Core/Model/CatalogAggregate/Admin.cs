using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CatalogManaging.Core.Model.CatalogAggregate
{
    /// <summary>
    /// Catalog Admin
    /// </summary>
    public class Admin : Entity<int>
    {
        /// <summary>
        /// Id of the Catalog to which the user is an admin 
        /// </summary>
        [Required]
        public int CatalogId { get; set; } 

        public Admin(int catalogId, int userId)
        {
            Id = userId;
            CatalogId=catalogId;
        }
        private Admin()//for EF
        {

        }
    }
}
