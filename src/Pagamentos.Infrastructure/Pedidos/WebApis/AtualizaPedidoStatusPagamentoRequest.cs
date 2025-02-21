using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pagamentos.Infrastructure.Pedidos.WebApis;

public record AtualizaPedidoStatusPagamentoRequest
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StatusPagamento Status { get; init; }
}