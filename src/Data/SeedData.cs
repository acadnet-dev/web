using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data
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

            // create ProblemAuthor role
            var problemAuthorRole = roleManager.FindByNameAsync(UserRole.ProblemAuthor).Result;
            if (problemAuthorRole == null)
            {
                problemAuthorRole = new Role(UserRole.ProblemAuthor);
                roleManager.CreateAsync(problemAuthorRole).GetAwaiter().GetResult();
            }
        }
    }
}