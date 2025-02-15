using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Pagamentos.Adapters.Controllers;

namespace Pagamento.Pagamento;

public record NovoPagamentoDTO
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MetodosDePagamento MetodoDePagamento { get; init; }

    public decimal ValorTotal { get; init; }
    public string EmailPagador { get; init; }
}
