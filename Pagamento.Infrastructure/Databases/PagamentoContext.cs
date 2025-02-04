using Microsoft.EntityFrameworkCore;

namespace Pagamento.Infrastructure.Databases;
public class PagamentoContext : DbContext
{
    public PagamentoContext()
    {

    }
    public PagamentoContext(DbContextOptions<PagamentoContext> options) : base(options)
    {
    }
    
    public DbSet<Domain.Entities.Pagamento> Pagamentos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PagamentoContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
