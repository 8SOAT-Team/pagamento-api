using Bogus;
using Bogus.Extensions.Brazil;
using Pagamento.Domain.Entities;


namespace Pagamento.Tests.IntegrationTests.Builder;
public class ClienteBuilder : Faker<Cliente>
{
    public ClienteBuilder()
    {
        CustomInstantiator(f => new Cliente(f.Person.Cpf(), f.Person.FullName, f.Person.Email));
    }
    public Cliente Build() => Generate();
}
