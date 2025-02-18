using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pagamentos.Infrastructure.Configurations;
using Pagamentos.Infrastructure.Databases;

namespace Pagamentos.Infrastructure.DependencyInjection;

public static class DatabaseContext
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services)
    {
        services.AddDbContext<PagamentoContext>(options =>
            options.UseLazyLoadingProxies().UseSqlServer(EnvConfig.DatabaseConnection));

        return services;
    }
}