using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UrlManaging.Core.Model;

namespace UrlManaging.Core.Contracts
{
    public interface IUrlContext
    {
        DbSet<TinyUrl> TinyUrl { get; set; }

        void SaveUrl(TinyUrl tinyUrl);
        int SaveChanges();

    }
}
