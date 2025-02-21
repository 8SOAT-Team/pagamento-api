using Pagamentos.Infrastructure.Pedidos.WebApis;
using DomainStatusPagamento = Pagamentos.Domain.Entities.StatusPagamento;

namespace Pagamentos.Infrastructure.Pedidos.Mappers;

public static class StatusPagamentoMapper
{
    public static StatusPagamento ToStatusPagamento(this DomainStatusPagamento status) => status switch
    {
        DomainStatusPagamento.Pendente => StatusPagamento.Pendente,
        DomainStatusPagamento.Autorizado => StatusPagamento.Aprovado,
        DomainStatusPagamento.Rejeitado or DomainStatusPagamento.Cancelado => StatusPagamento.Recusado,
        _ => throw new Exception("Status de pagamento não suportado")
    };
}