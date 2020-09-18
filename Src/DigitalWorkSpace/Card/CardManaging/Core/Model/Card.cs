using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Core.Model
{
    public class Card
    {
        private Card()
        {
                
        }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; } = 0;

        /// <summary>
        /// ShortUrl to which it is linked
        /// </summary>
        [Required]
        public string ShortUrl { get; set; }

        /// <summary>
        /// favicon of the card
        /// </summary>    
        public byte[] ImageContent { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Default favicon url
        /// </summary>
        public string Favicon { get; set; }


        /// <summary>
        /// Linking information of the card
        /// if linked to any catalog then true
        /// </summary>
        public bool IsLinked { get; set; }    
        
        public Card(Card card,int version)
        {
            this.Id = card.Id;
            this.Version = version;
            this.ShortUrl = card.ShortUrl;
        }

        public Card(Card card)
        {
            this.ShortUrl = card.ShortUrl;
            this.ImageContent = card.ImageContent;
            this.Title = card.Title;
            this.Description = card.Description;
            this.Favicon = card.Favicon;
        }
    }
}
