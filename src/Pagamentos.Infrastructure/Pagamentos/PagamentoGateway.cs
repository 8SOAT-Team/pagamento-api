using Microsoft.EntityFrameworkCore;
using Pagamentos.Apps.UseCases;
using Pagamentos.Domain.Entities;
using Pagamentos.Infrastructure.Databases;

namespace Pagamentos.Infrastructure.Pagamentos;

public class PagamentoGateway(PagamentoContext dbContext) : IPagamentoGateway
{
    public async Task<Pagamento> CreateAsync(Pagamento pagamento)
    {
        await dbContext.Set<Pagamento>().AddAsync(pagamento);
        await dbContext.SaveChangesAsync();
        return pagamento;
    }

    public async Task<List<Pagamento>> FindPagamentoByPedidoIdAsync(Guid pedidoId)
    {
        return await dbContext.Pagamentos.Where(p => p.PedidoId == pedidoId).ToListAsync();
    }

    public Task<Pagamento?> GetByIdAsync(Guid id)
    {
        return dbContext.Pagamentos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Pagamento> UpdatePagamentoAsync(Pagamento pagamento)
    {
        dbContext.Pagamentos.Update(pagamento);
        await dbContext.SaveChangesAsync();
        return pagamento;
    }

    public Task<Pagamento?> FindPagamentoByExternoIdAsync(string externoId)
    {
        return dbContext.Pagamentos.FirstOrDefaultAsync(p => p.PagamentoExternoId == externoId);
    }
}
