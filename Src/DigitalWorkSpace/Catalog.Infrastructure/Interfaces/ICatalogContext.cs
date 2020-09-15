using CatalogManaging.Core.Model.CatalogAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogManaging.Infrastructure.Interfaces
{
    public interface ICatalogContext
    {
        DbSet<Card> Card { get; set; }
        DbSet<Admin> Admin { get; set; }
        DbSet<Catalog> Catalog { get; set; }
        DbSet<PendingCard> PendingCard { get; set; }
        int SaveChanges();
    }
}
