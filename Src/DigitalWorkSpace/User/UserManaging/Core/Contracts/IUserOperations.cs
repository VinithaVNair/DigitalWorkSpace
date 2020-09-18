using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManaging.Core.Model;

namespace UserManaging.Core.Contracts
{
    public interface IUserOperations
    {
        User AddUser(string user);
        void Delete(string userName);
        IList<User> Users { get; }
        User GetUser(string userName);
    }
}
