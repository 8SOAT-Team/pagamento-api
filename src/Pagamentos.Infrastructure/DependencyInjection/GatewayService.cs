using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Pagamentos.Adapters.Gateways;
using Pagamentos.Apps.Gateways;
using Pagamentos.Apps.UseCases;
using Pagamentos.Infrastructure.Configurations;
using Pagamentos.Infrastructure.Pagamentos;
using Pagamentos.Infrastructure.Pedidos.Gateways;
using Pagamentos.Infrastructure.Pedidos.WebApis;
using Refit;

namespace Pagamentos.Infrastructure.DependencyInjection;

public static class GatewayService
{
    public static IServiceCollection AddGateways(this IServiceCollection services)
    {
        services.AddScoped<IPagamentoGateway, PagamentoGateway>()
            .DecorateIf<IPagamentoGateway, PagamentoGatewayCache>(() => !EnvConfig.IsTestEnv);

        services.AddScoped<IPedidoGateway, PedidoGateway>();
        services.AddPedidosApi();
        
        return services;
    }

    public static IServiceCollection AddGatewayDePagamento(this IServiceCollection services)
    {
        //Gateways
        services.AddSingleton<ISerializer, DefaultSerializer>();
        services.AddScoped<IFornecedorPagamentoGateway, FornecedorPagamentoGateway>();

        //client
        services.AddSingleton<PaymentClient>();
        services.AddSingleton<PreferenceClient>();

        return services;
    }

    private static void AddPedidosApi(this IServiceCollection services)
    {
        services.AddRefitClient<IPedidoApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(EnvConfig.PedidosApiUrl));
    }
}