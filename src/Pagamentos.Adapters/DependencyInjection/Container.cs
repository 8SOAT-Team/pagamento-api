using System.Diagnostics.CodeAnalysis;
using Pagamentos.Adapters.Controllers;

namespace Pagamentos.Adapters.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class Container
{
    public static IServiceCollection AddUseCaseControllers(this IServiceCollection services)
    {
        services.AddScoped<IPagamentoController, PagamentoController>();

        return services;
    }
}