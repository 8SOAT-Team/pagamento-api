using Microsoft.EntityFrameworkCore;
using Pagamento.Domain.Entities;

namespace Pagamento.Infrastructure.Databases;
public class PagamentoContext : DbContext
{
    public PagamentoContext()
    {

    }
    public PagamentoContext(DbContextOptions<PagamentoContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ItemDoPedido> ItensDoPedido { get; set; }
    public DbSet<Pagamento.Domain.Entities.Pagamento> Pagamentos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PagamentoContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
