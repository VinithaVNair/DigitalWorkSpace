using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using UrlManaging.Core.Contracts;
using UrlManaging.Core.Model;

namespace UrlManaging.Infrastructure.DataAccess
{
    public class UrlContext : DbContext, IUrlContext
    {
        public DbSet<TinyUrl> TinyUrl { get ; set ; }

        public UrlContext(DbContextOptions option) : base(option)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TinyUrl>().HasKey(v => new { v.ShortUrl });
        }

        public void SaveUrl(TinyUrl tinyUrl)
        {
            TinyUrl.Add(tinyUrl);
            SaveChanges();
        }
    }
}
