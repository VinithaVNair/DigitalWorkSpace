using CardManaging.Core.Contracts;
using CardManaging.Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Infrastructure.Data
{
    public class CardContext : DbContext, ICardContext
    {
        public DbSet<Card> Card { get ; set ; }
        public DbSet<LinkedCard> LinkedCard { get; set; }

        public CardContext(DbContextOptions option) : base(option)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>().HasKey(v => new { v.Id ,v.Version});
        }

    }
}
