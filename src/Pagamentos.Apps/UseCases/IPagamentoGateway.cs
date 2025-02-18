
using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases;
public interface IPagamentoGateway
{
    Task<Pagamento> CreateAsync(Pagamento pagamento);
    Task<List<Pagamento>> FindPagamentoByPedidoIdAsync(Guid pedidoId);
    Task<Pagamento?> GetByIdAsync(Guid id);
    Task<Pagamento> UpdatePagamentoAsync(Pagamento pagamento);
    Task<Pagamento?> FindPagamentoByExternoIdAsync(string externoId);
}
