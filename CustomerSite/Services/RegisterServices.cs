using CustomerSite.Interfaces;

namespace CustomerSite.Services
{
    public static class RegisterService
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
    }
}