﻿using Bogus;
using Pagamento.Domain.Entities;

namespace Pagamento.Tests.UnitTests.Domain.Stubs.Pedidos;

internal sealed class CategoriaStubBuilder : Faker<Categoria>
{
    private CategoriaStubBuilder()
    {
        CustomInstantiator(f => new Categoria(f.Commerce.Categories(1).First(), f.Commerce.Department()));
    }

    public static Categoria Create() => new CategoriaStubBuilder().Generate();
    public static List<Categoria> CreateMany(int qty) => new CategoriaStubBuilder().Generate(qty);
}