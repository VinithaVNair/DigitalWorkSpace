using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Core.Model
{
    public class LinkedCard
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Catalog to which it is linked
        /// </summary>
        public int CatalogId { get; set; }

        public LinkedCard(int cardId,int version , int catatlogId)
        {
            Id = cardId;
            Version = version;
            CatalogId = catatlogId;
        }
        private LinkedCard()
        {

        }
    }
}
