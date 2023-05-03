using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acadnet.Data.Identity
{
    public class User
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}