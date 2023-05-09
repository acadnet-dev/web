using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acadnet.Data;
using Acadnet.Data.Identity;
using Acadnet.Framework.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acadnet.Framework.Services
{
    public static class ServiceConfiguration
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            // Custom services
            services.AddScoped<ISecurityContext, SecurityContext>();
        }

        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Identity
            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<Database>()
                .AddDefaultTokenProviders();
            services.AddAuthentication();
        }

        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Authentication
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"]!;
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;

                })
                // .AddGithub
                .AddCookie();
        }
    }
}