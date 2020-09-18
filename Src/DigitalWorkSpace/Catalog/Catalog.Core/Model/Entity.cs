using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CatalogManaging.Core.Model
{
    public abstract class Entity<T>
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        [Required]
        public T Id { get;  set; }

        protected Entity()
        {

        }
        protected Entity(T id)
        {
            Id = id;
        }
    }
}
