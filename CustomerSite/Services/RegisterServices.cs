using CustomerSite.Interfaces;

namespace CustomerSite.Services
{
    public static class RegisterService
    {
        // We pass in IConfiguration so we can still read your appsettings.json!
        public static IServiceCollection AddBackendApiClients(this IServiceCollection services, IConfiguration config)
        {
            var backendUrl = config["ApiUrls:BackendApi"];

        Action<HttpClient> configureClient = client =>
        {
            client.BaseAddress = new Uri(backendUrl!);
        };

        services.AddHttpClient<IProductApiClient, ProductApiClient>(configureClient);
        services.AddHttpClient<IOrderApiClient,OrderApiClient>(configureClient);
            return services;
        }
    }
}