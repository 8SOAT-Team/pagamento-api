using Pagamentos.Domain.Entities;

namespace Pagamentos.Apps.UseCases.Dtos;
[ExcludeFromCodeCoverage]
public record IniciarPagamentoDto
{
    public Guid PedidoId { get; init; }

    public MetodoDePagamento MetodoDePagamento { get; init; }

    public List<IniciarPagamentoItemDto> Itens { get; init; } = [];

    public IniciarPagamentoPagadorDto? Pagador { get; init; }
}
[ExcludeFromCodeCoverage]
public record IniciarPagamentoItemDto
{
    public Guid Id { get; init; }
    public string Titulo { get; init; }
    public string Descricao { get; init; }
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
}
[ExcludeFromCodeCoverage]
public record IniciarPagamentoPagadorDto
{
    public string Email { get; init; }
    public string Nome { get; init; }
    public string Cpf { get; init; }
}