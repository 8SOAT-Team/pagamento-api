using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pagamento.Domain.Entities;

namespace Pagamento.Infrastructure.Databases;
internal class ProdutoTypeConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).IsRequired().HasMaxLength(100).IsRequired();
        builder.Property(p => p.Descricao).IsRequired().HasMaxLength(100);
        builder.Property(p => p.CategoriaId);
        builder.Property(p => p.Preco).HasPrecision(18, 2);
        builder.Property(p => p.Imagem).IsRequired().HasMaxLength(300);
        builder.HasOne(p => p.Categoria).WithMany(p => p.Produtos).HasForeignKey(p => p.CategoriaId);
    }
}
