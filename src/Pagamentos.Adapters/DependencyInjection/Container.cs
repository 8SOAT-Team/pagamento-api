using System.Diagnostics.CodeAnalysis;
using Pagamentos.Adapters.Controllers;
using Pagamentos.Apps.Handlers;

namespace Pagamentos.Adapters.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class Container
{
    public static IServiceCollection AddUseCaseControllers(this IServiceCollection services)
    {
        services.AddScoped<IPagamentoController, PagamentoController>();
        services.AddHandlers();
        
        return services;
    }

    private static void AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IPagamentoHandler, PagamentoHandler>();
    }
}