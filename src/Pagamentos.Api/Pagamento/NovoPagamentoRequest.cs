using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Pagamentos.Adapters.Controllers;

namespace Pagamentos.Api.Pagamento;

public record NovoPagamentoRequest
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MetodosDePagamento MetodoDePagamento { get; init; }

    [Required] public List<NovoPagamentoItemRequest> Itens { get; init; } = [];

    public NovoPagamentoPagadorRequest? Pagador { get; init; }
}