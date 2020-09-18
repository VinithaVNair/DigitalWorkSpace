using CardManaging.Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardManaging.Core.Contracts
{
    public interface ICardContext
    {
        DbSet<Card> Card { get; set; }
        DbSet<LinkedCard> LinkedCard { get; set; }
        int SaveChanges();
    }
}
