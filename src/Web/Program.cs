using Data;
using Data.Identity;
using Framework.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Port config, if not ran by IISExpress
builder.WebHost.UseUrls(builder.Configuration["Urls"]!);

// Settings
builder.Services.AddSettings(builder.Configuration);

// Database
builder.Services.AddDbContext<Database>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Identity
builder.Services.AddIdentityServices(builder.Configuration);

// Authentication
builder.Services.AddAuthenticationServices(builder.Configuration);

// Custom services
builder.Services.AddCustomServices();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperConfigProfile));

// Application cookie
builder.Services.AddApplicationCookie();

// Data protection
builder.Services.AddDataProtection(builder.Configuration);

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<Database>();
    database.Database.Migrate();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    SeedData.Initialize(database, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
