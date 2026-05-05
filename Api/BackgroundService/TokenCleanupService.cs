using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data;
namespace Api.Service;
public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

        public TokenCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var deadTokens = context.RefreshTokens
                        .Where(t => t.Expires <= DateTime.UtcNow || t.IsRevoked);

                    if (deadTokens.Any())
                    {
                        context.RefreshTokens.RemoveRange(deadTokens);
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
}