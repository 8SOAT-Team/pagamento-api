using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pagamento.Domain.Entities;
using Pagamento.Domain.ValueObjects;

namespace Pagamento.Infrastructure.Databases;
internal class ClienteTypeConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Cpf).IsRequired().HasMaxLength(11)
            .HasConversion(new ValueConverter<Cpf, string>(v => v.GetSanitized(), v => new Cpf(v)));

        builder.HasIndex(c => c.Cpf);

        builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(100)
            .HasConversion(new ValueConverter<EmailAddress, string>(v => v.Address, v => new EmailAddress(v)));
    }
}
