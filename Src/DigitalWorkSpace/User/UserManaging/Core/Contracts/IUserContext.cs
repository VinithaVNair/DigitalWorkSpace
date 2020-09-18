using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using UserManaging.Core.Model;

namespace UserManaging.Core.Contracts
{
    public interface IUserContext
    {
        DbSet<User> User { get; set; }
        int SaveChanges();
    }
}
