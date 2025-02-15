using Bogus;
using Bogus.Extensions.Brazil;
using Pagamentos.Domain.Entities;


namespace Pagamentos.Tests.IntegrationTests.Builder;
public class ClienteBuilder : Faker<Cliente>
{
    public ClienteBuilder()
    {
        CustomInstantiator(f => new Cliente(f.Person.Cpf(), f.Person.FullName, f.Person.Email));
    }
    public Cliente Build() => Generate();
}
