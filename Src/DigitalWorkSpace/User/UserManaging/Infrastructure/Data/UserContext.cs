using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManaging.Core.Contracts;
using UserManaging.Core.Model;

namespace UserManaging.Infrastructure.Data
{
    public class UserContext : DbContext,IUserContext
    {
        public DbSet<User> User { get; set; }
        public UserContext(DbContextOptions option) : base(option)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(v => new { v.UserId});
        }
    }
}
