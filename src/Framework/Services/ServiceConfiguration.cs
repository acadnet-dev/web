using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Identity;
using Data.Settings;
using Framework.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Services
{
    public static class ServiceConfiguration
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            // Custom services
            services.AddScoped<ISecurityContext, SecurityContext>();

            // Course services
            services.AddScoped<ICourseService, CourseService>();

            // Problem services
            services.AddScoped<IProblemService, ProblemService>();
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

        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // Settings
            services.Configure<MinioSettings>(configuration.GetSection("Minio"));
        }

        public static void AddApplicationCookie(this IServiceCollection services)
        {
            // Application cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
            });
        }
    }
}