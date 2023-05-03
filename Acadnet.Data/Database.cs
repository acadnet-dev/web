using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acadnet.Data.Identity;
using Acadnet.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Acadnet.Data
{
    public class Database : IdentityDbContext<User, Role, string>
    {
        public Database(DbContextOptions<Database> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("data");

            base.OnModelCreating(builder);
        }
    }
}