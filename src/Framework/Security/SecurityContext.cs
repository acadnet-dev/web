using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Framework.Security
{
    public class SecurityContext : ISecurityContext
    {
        public readonly IHttpContextAccessor _httpContext;
        public readonly UserManager<User> _userManager;

        public SecurityContext(
            IHttpContextAccessor httpContext,
            UserManager<User> userManager
        )
        {
            _httpContext = httpContext;
            _userManager = userManager;
        }

        public bool IsAuthenticated => User != null;

        public User? User => _userManager.GetUserAsync(_httpContext.HttpContext!.User).Result;

        public bool UserHasRole(string role)
        {
            if (User == null)
                return false;

            return _userManager.IsInRoleAsync(User, role).Result;
        }
    }
}