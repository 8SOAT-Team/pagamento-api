using Microsoft.Extensions.DependencyInjection;
using Pagamentos.Adapters.Gateways;
using Pagamentos.Infrastructure.Configurations;
using Pagamentos.Infrastructure.Databases;
using StackExchange.Redis;

namespace Pagamentos.Infrastructure.DependencyInjection;

public static class CacheService
{
    public static IServiceCollection AddCacheService(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(EnvConfig.DistributedCacheUrl, o =>
            {
                o.AbortOnConnectFail = false;
                o.ConnectRetry = 2;
                o.Ssl = false;
                o.ConnectTimeout = 5;
            }));
        services.AddSingleton<ICacheContext, CacheContext>();

        return services;
    }
}