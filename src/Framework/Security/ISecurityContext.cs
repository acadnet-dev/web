using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;

namespace Framework.Security
{
    public interface ISecurityContext
    {
        bool IsAuthenticated { get; }

        User? User { get; }

        /// <summary>
        /// Returns true if the user has the specified role.
        /// Also returns false if the user is not authenticated.
        /// </summary>
        bool UserHasRole(string role);
    }
}