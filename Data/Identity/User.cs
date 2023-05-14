using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Data.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;
    }
}