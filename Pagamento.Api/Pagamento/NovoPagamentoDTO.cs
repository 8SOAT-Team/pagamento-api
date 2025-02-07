using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Pagamento.Adapters.Controllers;

namespace Pagamento.Api.Pagamento;

public record NovoPagamentoDTO
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MetodosDePagamento MetodoDePagamento { get; init; }
}
