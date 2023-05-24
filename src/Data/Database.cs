using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;
using Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class Database : IdentityDbContext<User, Role, string>
    {
        public Database(DbContextOptions<Database> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Problem> Problems { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("data");

            base.OnModelCreating(builder);

            builder.Entity<Course>().HasMany(c => c.Maintainers).WithMany(u => u.MaintainedCourses);
        }
    }
}