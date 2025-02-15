using Microsoft.EntityFrameworkCore;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Infrastructure.Databases;
public class PagamentoContext : DbContext
{
    public PagamentoContext()
    {

    }
    public PagamentoContext(DbContextOptions<PagamentoContext> options) : base(options)
    {
    }

    public DbSet<Pagamento> Pagamentos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PagamentoContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
