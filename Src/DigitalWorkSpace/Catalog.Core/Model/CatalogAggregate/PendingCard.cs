using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CatalogManaging.Core.Model.CatalogAggregate
{
    /// <summary>
    /// Cards which are pending to be linked to the catalog after an edit
    /// </summary>
    public class PendingCard :Entity<int>
    {
        /// <summary>
        /// Catalog id to which it needs to be linked after approval
        /// </summary>
        [Required]
        public int CatalogId { get; private set; }

        /// <summary>
        /// New edited version of the card
        /// </summary>
        [Required]
        public int Version { get; private set; }
        private PendingCard ()
        {

        }
        public PendingCard(int id, int version) : base(id)
        {
            Version = version;
        }
        public PendingCard (int id ,int catalogId,int version):base(id)
        {
            Version = version;
            CatalogId = catalogId;
        }
    }
}
