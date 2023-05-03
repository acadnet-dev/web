using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Acadnet.Data.Identity
{
    public class Role : IdentityRole
    {
        public Role()
        {
        }

        public Role(string roleName) : base(roleName)
        {
        }
    }

    public static class UserRole
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}