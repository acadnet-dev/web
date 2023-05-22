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

        bool UserHasRole(string role);
    }
}