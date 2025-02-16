namespace Pagamentos.Api.Pagamento;

public record NovoPagamentoPagadorRequest
{
    public string Email { get; init; }
    public string Nome { get; init; }
    public string Cpf { get; init; }
}