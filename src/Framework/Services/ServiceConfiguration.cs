using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Identity;
using Data.Settings;
using Framework.Security;
using Framework.Services.FileServices;
using Framework.Services.ProblemServices;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
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
            services.AddScoped<ProblemServiceFactory>();
            services.AddScoped<SimpleAcadnetISProblemService>()
                    .AddScoped<IProblemService, SimpleAcadnetISProblemService>(provider => provider.GetService<SimpleAcadnetISProblemService>()!);

            // File services
            services.AddScoped<FileServiceFactory>();
            services.AddScoped<MinioFileService>()
                    .AddScoped<IFileService, MinioFileService>(provider => provider.GetService<MinioFileService>()!);
            services.AddScoped<S3FileService>()
                    .AddScoped<IFileService, S3FileService>(provider => provider.GetService<S3FileService>()!);

            // Checker services
            services.AddScoped<ICheckerService, CheckerService>();

            // Workspace services
            services.AddScoped<IWorkspaceService, WorkspaceService>();
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
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                });
        }

        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // Settings
            services.Configure<S3Settings>(configuration.GetSection("S3"));
            services.Configure<CheckerSettings>(configuration.GetSection("Checker"));
            services.Configure<WorkspacesSettings>(configuration.GetSection("Workspaces"));
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

        public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            // Data protection
            services.AddDataProtection()
                .PersistKeysToDbContext<Database>()
                .SetApplicationName(configuration["DataProtection:ApplicationName"]!);
        }
    }
}