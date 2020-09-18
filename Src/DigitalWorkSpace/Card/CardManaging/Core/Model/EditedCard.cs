using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CardManaging.Core.Model
{
    public class EditedCard
    {
        /// <summary>
        /// Id of the original card which is being edited
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// current version of the card before edit
        /// </summary>
        [Required]
        public int Version { get; set; } = 0;

        /// <summary>
        /// favicon of the edited card if any
        /// </summary>
        public byte[] ImageContent { get; set; }

        /// <summary>
        /// Title of the card
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Description of the card
        /// </summary>
        [Required]
        public string Description { get; set; }
    }
}
