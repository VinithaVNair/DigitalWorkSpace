using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace UserManaging.Core.Model
{
    public class User
    {
        /// <summary>
        /// User id of the user
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User name of the user
        /// </summary>
        public string UserName { get; set; }
    }
}
