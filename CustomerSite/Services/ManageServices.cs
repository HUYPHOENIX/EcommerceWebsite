using CustomerSite.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.Services
{
    public static class ManageService
    {
        public static IServiceCollection AddApiService(this IServiceCollection services, IConfiguration config)
        {
            var backendUrl = config["ApiUrls:BackendApi"];

            Action<HttpClient> configureClient = client =>
            {
                client.BaseAddress = new Uri(backendUrl!);
            };
            services.AddHttpClient<IProductApiService, ProductApiService>(configureClient);
            services.AddHttpClient<IOrderApiService, OrderApiService>(configureClient);
            services.AddHttpClient<IAuthService, AuthService>(configureClient);
            services.AddHttpClient<ICategoryApiService , CategoryService>(configureClient);
            return services;
        }

        public static IServiceCollection AddSessionService(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache()
                    .AddSession(options =>
                    {
                        options.IdleTimeout = TimeSpan.FromMinutes(10);
                        options.Cookie.HttpOnly = true;
                        options.Cookie.IsEssential = true;
                    })
                    .AddHttpContextAccessor()
                    .AddScoped<ICartService, CartService>();                   
            return services;
        }
        [HttpPost]
        public static IServiceCollection AddAuthenticationService(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.Cookie.HttpOnly = true;
                        options.LoginPath = "/Auth/Login";
                    });
            return services;
        }
    }
}