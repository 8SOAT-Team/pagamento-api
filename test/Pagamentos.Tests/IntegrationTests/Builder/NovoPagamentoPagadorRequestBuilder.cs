using Bogus;
using Pagamentos.Api.Pagamento;

namespace Pagamentos.Tests.IntegrationTests.Builder;

internal sealed class NovoPagamentoPagadorRequestBuilder : Faker<NovoPagamentoPagadorRequest>
{
    public static NovoPagamentoPagadorRequest Build() => new NovoPagamentoPagadorRequestBuilder().Generate();
}