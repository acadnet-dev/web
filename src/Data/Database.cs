using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;
using Data.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class Database : IdentityDbContext<User, Role, string>, IDataProtectionKeyContext
    {
        public Database(DbContextOptions<Database> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Problem> Problems { get; set; } = default!;
        public DbSet<Submission> Submissions { get; set; } = default!;

        // Data protection
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("data");

            base.OnModelCreating(builder);

            builder.Entity<Course>().HasMany(c => c.Maintainers).WithMany(u => u.MaintainedCourses);

            builder.Entity<Problem>().HasMany(p => p.Submissions).WithOne(s => s.Problem);
        }
    }
}