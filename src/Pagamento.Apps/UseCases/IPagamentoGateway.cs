
namespace Pagamento.Apps.UseCases;
public interface IPagamentoGateway
{
    Task<List<Domain.Entities.Pagamento>> FindPagamentoByPedidoIdAsync(Guid pedidoId);
    Task<Domain.Entities.Pagamento?> GetByIdAsync(Guid id);
    Task<Domain.Entities.Pagamento> UpdatePagamentoAsync(Domain.Entities.Pagamento pagamento);
}
