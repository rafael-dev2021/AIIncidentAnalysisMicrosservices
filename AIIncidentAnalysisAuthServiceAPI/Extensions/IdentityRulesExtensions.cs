using AIIncidentAnalysisAuthServiceAPI.Context;
using AIIncidentAnalysisAuthServiceAPI.Models;
using AIIncidentAnalysisAuthServiceAPI.Security;
using Microsoft.AspNetCore.Identity;

namespace AIIncidentAnalysisAuthServiceAPI.Extensions;

public static class IdentityRulesExtensions
{
      public static void AddIdentityRulesExtensions(this IServiceCollection service)
  {
      service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      service.AddDistributedMemoryCache();
      service.AddSession();
      service.AddMemoryCache();

      service.AddIdentity<PoliceOfficer, IdentityRole>()
          .AddEntityFrameworkStores<AppDbContext>()
          .AddDefaultTokenProviders();

      service.Configure<IdentityOptions>(options =>
      {
          options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(15);
          options.Lockout.MaxFailedAccessAttempts = 3;
          options.Lockout.AllowedForNewUsers = true;
      });

      service.Configure<PasswordOptions>(options =>
      {
          options.RequireDigit = true;
          options.RequireLowercase = true;
          options.RequireUppercase = true;
          options.RequireNonAlphanumeric = true;
          options.RequiredLength = 8;
          options.RequiredUniqueChars = 6;
      });

      service.AddAuthorizationBuilder()
          .AddPolicy("Admin", policy => { policy.RequireRole("Admin"); });

      service.AddHttpsRedirection(options => { options.HttpsPort = null; });

      service.AddHttpContextAccessor();
      service.AddMvc(options => { options.Filters.Add(new SecurityHeadersAttribute()); });

      service.Configure<DataProtectionTokenProviderOptions>(options =>
      {
          options.TokenLifespan = TimeSpan.FromMinutes(15);
      });

      service.AddAntiforgery(options => { options.HeaderName = "X-CSRF-TOKEN"; });

      service.ConfigureApplicationCookie(options =>
      {
          options.Cookie.HttpOnly = true;
          options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
          options.Cookie.SameSite = SameSiteMode.Strict;
          options.Cookie.IsEssential = true;

          options.LoginPath = "/api/v1/auth/Login";
          options.LogoutPath = "/api/v1/auth/Logout";
          options.AccessDeniedPath = "/api/v1/auth/AccessDenied";
          options.SlidingExpiration = true;
          options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
      });
  }
}