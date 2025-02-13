using Microsoft.EntityFrameworkCore;
using Pagamento.Apps.UseCases;
using Pagamento.Infrastructure.Databases;

namespace Pagamento.Adapters.Gateways;

public class PagamentoGateway(PagamentoContext dbContext) : IPagamentoGateway
{

    private readonly PagamentoContext _dbContext = dbContext;

    public Task<List<Domain.Entities.Pagamento>> FindPagamentoByPedidoIdAsync(Guid pedidoId)
    {
        return _dbContext.Pagamentos.Where(p => p.PedidoId == pedidoId).ToListAsync();
    }

    public Task<Domain.Entities.Pagamento?> GetByIdAsync(Guid id)
    {
        return _dbContext.Pagamentos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Domain.Entities.Pagamento> UpdatePagamentoAsync(Domain.Entities.Pagamento pagamento)
    {
        _dbContext.Pagamentos.Update(pagamento);
        await _dbContext.SaveChangesAsync();
        return pagamento;
    }
}
