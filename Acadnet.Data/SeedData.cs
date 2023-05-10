using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acadnet.Data.Identity;
using Acadnet.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            // add test categories and exercises
            AddTestCategoriesAndExercises(context);
        }

        public static void AddTestCategoriesAndExercises(Database context)
        {
            var rootCategory = context.Categories.FirstOrDefault(c => c.Id == 1);
            if (rootCategory != null)
                return;

            // add categories
            var root = new Category
            {
                Id = 1,
                Name = "Olimpiada Acadnet - Interoperabilitate software",
                Description = "Olimpiada Acadnet - Interoperabilitate software",
            };

            var year = new Category
            {
                Id = 2,
                Name = "2023",
                Description = "2023",
                Parent = root
            };

            var stage = new Category
            {
                Id = 3,
                Name = "Etapa locala",
                Description = "Etapa locala",
                Parent = year
            };

            var schoolYear = new Category
            {
                Id = 4,
                Name = "9-10",
                Description = "9-10",
                Parent = stage
            };

            // seed categories
            context.Categories.Add(root);
            context.Categories.Add(year);
            context.Categories.Add(stage);
            context.Categories.Add(schoolYear);

            // add exercises
            var exercise1 = new Exercise
            {
                Id = 1,
                Name = "Binary",
                Category = schoolYear
            };

            var exercise2 = new Exercise
            {
                Id = 2,
                Name = "Buffer overflow",
                Category = schoolYear
            };

            // seed exercises
            context.Exercises.Add(exercise1);
            context.Exercises.Add(exercise2);

            context.SaveChanges();
        }
    }
}