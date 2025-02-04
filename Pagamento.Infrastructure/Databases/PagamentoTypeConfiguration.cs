using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pagamento.Infrastructure.Databases;

internal class PagamentoTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.Pagamento>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Pagamento> builder)
    {
        builder.ToTable("Pagamentos");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.PedidoId).IsRequired();
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.MetodoDePagamento).IsRequired();
        builder.Property(p => p.ValorTotal).HasPrecision(18, 2);
        builder.Property(p => p.PagamentoExternoId).IsRequired(false);

        builder.HasOne(p => p.Pedido).WithOne(p => p.Pagamento).HasForeignKey<Domain.Entities.Pagamento>(p => p.PedidoId);
    }
}
