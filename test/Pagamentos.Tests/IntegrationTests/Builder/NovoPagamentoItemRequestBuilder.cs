using Bogus;
using Pagamentos.Api.Pagamento;

namespace Pagamentos.Tests.IntegrationTests.Builder;

internal sealed class NovoPagamentoItemRequestBuilder : Faker<NovoPagamentoItemRequest>
{
    public static NovoPagamentoItemRequest Build() => new NovoPagamentoItemRequestBuilder().Generate();
}