using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acadnet.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace Acadnet.Data
{
    public class SeedData
    {
        public static void Initialize(Database context, RoleManager<Role> roleManager)
        {
            // create User role
            var userRole = roleManager.FindByNameAsync(UserRole.User).Result;
            if (userRole == null)
            {
                userRole = new Role(UserRole.User);
                roleManager.CreateAsync(userRole).GetAwaiter().GetResult();
            }

            // create Admin role
            var adminRole = roleManager.FindByNameAsync(UserRole.Admin).Result;
            if (adminRole == null)
            {
                adminRole = new Role(UserRole.Admin);
                roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
            }
        }
    }
}