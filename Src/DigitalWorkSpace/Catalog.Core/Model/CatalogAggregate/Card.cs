using CatalogManaging.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CatalogManaging.Core.Model.CatalogAggregate
{
    /// <summary>
    /// Un expirable card link to a catalog
    /// </summary>
    public class Card : Entity<int> //Should make it unexpirable card??
    {
        /// <summary>
        /// Id of the catalog to which it is linked
        /// </summary>
        [Required]
        public int CatalogId { get; private set; }

        /// <summary>
        /// Version of the card which is linked to this perticular catalog
        /// </summary>
        [Required]
        public int Version { get; private set; }
        public Card(int id, int version) : base(id)
        {
            Version = version;
        }
        private Card()//for EF
        {

        }
        public void LinkToCatalog(int catalogid)
        {
            this.CatalogId = catalogid;
        }
    }
}
