using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManaging.Core.Contracts;
using UserManaging.Core.Model;

namespace UserManaging.Core
{
    public class UserOperations : IUserOperations
    {
        private readonly IUserContext _userContext;
        public IList<User> Users { get { return _userContext.User.ToList(); } }
        private readonly ILogger<UserOperations> _logger;
        public UserOperations(IUserContext userContext, ILogger<UserOperations> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }
        public User AddUser(string newUser)
        {
            var user = new User { UserName = newUser };
            var existingUser = _userContext.User.Where(v => v.UserName == newUser).FirstOrDefault();
            if (existingUser==null)
            {
                _userContext.User.Add(user);
                _userContext.SaveChanges();
                return user;//with new id
            }
            else
            {
                _logger.LogInformation("Already exists an user with same user name {userName}", newUser);
                return existingUser;
            }
        }

        public void Delete(string user)
        {
            var userTobeDeleted = _userContext.User.Where(d => d.UserName == user).FirstOrDefault();
            if (userTobeDeleted != null)
            {
                _userContext.User.Remove(userTobeDeleted);
                _userContext.SaveChanges();
                _logger.LogInformation("User {userName} deleted", user);
            }
        }

        public User GetUser(string userName)
        {
            return _userContext.User.Where(d => d.UserName == userName).FirstOrDefault();
        }
    }
}
