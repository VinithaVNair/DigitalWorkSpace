using CatalogManaging.Core.Model.CatalogAggregate;
using CatalogManaging.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogManaging.Infrastructure.Data
{
    public class CatalogContext : DbContext, ICatalogContext
    {
        public DbSet<Card> Card { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Catalog> Catalog { get ; set ; }
        public DbSet<PendingCard> PendingCard { get; set ; }

        public CatalogContext(DbContextOptions option) : base(option)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>().HasKey(v => new { v.Id, v.CatalogId });
            modelBuilder.Entity<Admin>().HasKey(v => new { v.Id ,v.CatalogId});
            modelBuilder.Entity<Catalog>().HasKey(v => new { v.Id });
        }
    }
}
