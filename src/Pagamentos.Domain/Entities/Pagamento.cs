using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Pagamentos.Domain.Exceptions;

namespace Pagamentos.Domain.Entities;

public class Pagamento : Entity, IAggregateRoot{
    private static readonly ImmutableDictionary<MetodoDePagamento, MetodoDePagamento[]> MetodosDePagamentosObrigatorios = new Dictionary<MetodoDePagamento, MetodoDePagamento[]>
        {
        { MetodoDePagamento.Cartao, [MetodoDePagamento.Master, MetodoDePagamento.Visa] },
        { MetodoDePagamento.Pix, [] }
    }
    .ToImmutableDictionary();

    public const int Parcelas = 1;

    protected Pagamento() { }

    [JsonConstructor]
    public Pagamento(Guid id, Guid pedidoId, MetodoDePagamento metodoDePagamento, decimal valorTotal, string? pagamentoExternoId)
    {
        ValidationDomain(id, pedidoId, metodoDePagamento, valorTotal);

        Id = id;
        PedidoId = pedidoId;
        MetodoDePagamento = metodoDePagamento;
        ValorTotal = valorTotal;
        PagamentoExternoId = pagamentoExternoId;
        Status = StatusPagamento.Pendente;
    }

    public Pagamento(Guid pedidoId, MetodoDePagamento metodoDePagamento, decimal valorTotal, string? pagamentoExternoId)
    : this(Guid.NewGuid(), pedidoId, metodoDePagamento, valorTotal, pagamentoExternoId) { }

    public Guid PedidoId { get; init; }
    public string? PagamentoExternoId { get; private set; }
    public StatusPagamento Status { get; private set; }
    public MetodoDePagamento MetodoDePagamento { get; init; }
    public decimal ValorTotal { get; init; }
    public string? UrlPagamento { get; private set; }

    public bool EstaAutorizado() => Status == StatusPagamento.Autorizado;
    public bool EstaPendente() => Status == StatusPagamento.Pendente;

    public void FinalizarPagamento(bool autorizado)
    {
        DomainExceptionValidation.When(Status != StatusPagamento.Pendente, $"Pagamento só pode ser confirmado quando o status atual é {StatusPagamento.Pendente}");
        Status = autorizado ? StatusPagamento.Autorizado : StatusPagamento.Rejeitado;
    }

    public void AssociarPagamentoExterno(string pagamentoExternoId, string urlPagamento)
    {
        PagamentoExternoId = pagamentoExternoId;
        UrlPagamento = urlPagamento;
    }

    private static void ValidationDomain(Guid id, Guid pedidoId, MetodoDePagamento metodoDePagamento, decimal valorTotal/*,string pagamentoExternoId*/)
    {
        DomainExceptionValidation.When(id == Guid.Empty, "Id inválido");
        DomainExceptionValidation.When(pedidoId == Guid.Empty, "Pedido inválido");
        DomainExceptionValidation.When(Enum.IsDefined(metodoDePagamento) is false, "Método de pagamento inválido");
        DomainExceptionValidation.When(valorTotal < 0, "Valor total inválido");
        DomainExceptionValidation.When(ValidationMetodoDePagamentoCartao(metodoDePagamento) is false, "Método de pagamento inválido");
    }

    private static bool ValidationMetodoDePagamentoCartao(MetodoDePagamento metodoDePagamento)
    {
        foreach (var metodo in MetodosDePagamentosObrigatorios)
        {
            if (metodoDePagamento.HasFlag(metodo.Key))
            {
                var tipoDeMetodoNecessario = metodo.Value;

                if (tipoDeMetodoNecessario == Array.Empty<MetodoDePagamento>())
                {
                    continue;
                }

                var temAlgumTipo = false;

                foreach (var tipo in tipoDeMetodoNecessario)
                {
                    temAlgumTipo = metodoDePagamento.HasFlag(tipo);

                    if (temAlgumTipo) break;
                }

                if (temAlgumTipo is false)
                {
                    return temAlgumTipo;
                }
            }
        }

        return true;
    }
}
