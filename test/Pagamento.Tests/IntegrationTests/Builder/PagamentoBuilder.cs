using Bogus;
using Pagamento.Domain.Entities;
using Pagamento.Tests.IntegrationTests.Builder;

namespace IntegrationTests.Builder
{
    internal class PagamentoBuilder : Faker<Pagamento.Domain.Entities.Pagamento>
    {
        public PagamentoBuilder()
        {
            // Customizando a instância do Pagamento
            CustomInstantiator(f => new Pagamento.Domain.Entities.Pagamento(
                pedidoId: new PedidoBuilder().Build().Id, // Usando o PedidoBuilder para gerar o pedido
                metodoDePagamento: f.PickRandom<MetodoDePagamento>(), // Seleciona aleatoriamente um método de pagamento
                valorTotal: f.Finance.Amount(1, 1000), // Gera um valor aleatório entre 1 e 1000
                pagamentoExternoId: f.Random.Bool() ? f.Random.Guid().ToString() : null // Pode ou não ter um pagamento externo
            ));
        }

        // Método para gerar uma instância de Pagamento com valores definidos
        public PagamentoBuilder ComPedido(Pedido pedido)
        {
            CustomInstantiator(f => new Pagamento.Domain.Entities.Pagamento(
                pedidoId: pedido.Id,
                metodoDePagamento: MetodoDePagamento.Master,
                valorTotal: f.Finance.Amount(1, 1000),
                pagamentoExternoId: f.Random.Bool() ? f.Random.Guid().ToString() : null
            ));
            return this;
        }

        // Método para definir um método de pagamento específico
        public PagamentoBuilder ComMetodoDePagamento(MetodoDePagamento metodoDePagamento)
        {
            CustomInstantiator(f => new Pagamento.Domain.Entities.Pagamento(
                pedidoId: new PedidoBuilder().Build().Id,
                metodoDePagamento: metodoDePagamento,
                valorTotal: f.Finance.Amount(1, 1000),
                pagamentoExternoId: f.Random.Bool() ? f.Random.Guid().ToString() : null
            ));
            return this;
        }

        // Método para definir um valor total específico
        public PagamentoBuilder ComValorTotal(decimal valorTotal)
        {
            CustomInstantiator(f => new Pagamento.Domain.Entities.Pagamento(
                pedidoId: new PedidoBuilder().Build().Id,
                metodoDePagamento: f.PickRandom<MetodoDePagamento>(),
                valorTotal: valorTotal,
                pagamentoExternoId: f.Random.Bool() ? f.Random.Guid().ToString() : null
            ));
            return this;
        }

        // Método para definir o status do pagamento
        public PagamentoBuilder ComStatus(StatusPagamento status)
        {
            CustomInstantiator(f => new Pagamento.Domain.Entities.Pagamento(
                pedidoId: new PedidoBuilder().Build().Id,
                metodoDePagamento: f.PickRandom<MetodoDePagamento>(),
                valorTotal: f.Finance.Amount(1, 1000),
                pagamentoExternoId: f.Random.Bool() ? f.Random.Guid().ToString() : null
            ));
            return this;
        }

        // Método para gerar o pagamento
        public Pagamento.Domain.Entities.Pagamento Build() => Generate();
    }
}
