namespace Pagamento.Domain.Entities;

public enum StatusPagamento
{
    Pendente = 0,
    Autorizado = 1,
    Rejeitado = 2,
    Cancelado = 3,
}