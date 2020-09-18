using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using UserManaging.Core.Contracts;
using UserManaging.Core.Model;

namespace UserManaging.Controllers
{
    [Route("users")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class UserManagingController : ControllerBase
    {
        private readonly IUserOperations _userOperations;
        private readonly ILogger<UserManagingController> _logger;
        public UserManagingController(IUserOperations userOperations, ILogger<UserManagingController> logger)
        {
            _userOperations = userOperations;
            _logger = logger;
        }

        /// <summary>
        /// Gets user details by his/her user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("{userName}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<User> GetUser(string userName)
        {
            var user = _userOperations.GetUser(userName);
            return Ok(user);
        }

        /// <summary>
        /// Creates a user id for the user name
        /// </summary>
        /// <param name="userName">user name</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<User> AddUser([Required][FromBody]string userName)
        {
            _logger.LogInformation("user creation initated for {userName}", userName);
            var user = _userOperations.AddUser(userName);
            return Ok(user);
        }

        /// <summary>
        /// Gets user details by his/her user name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = _userOperations.Users;
            return Ok(users);
        }
    }
}
