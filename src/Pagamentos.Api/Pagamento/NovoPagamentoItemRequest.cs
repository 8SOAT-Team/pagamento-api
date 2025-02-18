using System.ComponentModel.DataAnnotations;

namespace Pagamentos.Api.Pagamento;

public record NovoPagamentoItemRequest
{
    [Required] public Guid Id { get; init; }
    public string Titulo { get; init; }
    public string Descricao { get; init; }
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
}