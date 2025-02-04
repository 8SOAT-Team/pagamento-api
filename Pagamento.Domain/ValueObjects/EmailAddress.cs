
using Pagamento.Domain.Expressions;

namespace Pagamento.Domain.Entities.ValueObjects;

public record EmailAddress
{
    public string Address { get; private init; } = null!;

    public EmailAddress(string address)
    {
        DomainExceptionValidation.When<InvalidEmailArgumentException>(IsValidEmail(address) is false);
        Address = address;
    }

    public override int GetHashCode() => Address.GetHashCode();

    public override string? ToString() => Address;

    private static bool IsValidEmail(string email) => string.IsNullOrEmpty(email) is false && Expression.ValidEmail().IsMatch(email);

    public static bool TryCreate(string email, out EmailAddress result)
    {
        result = null!;

        if (IsValidEmail(email))
        {
            result = new EmailAddress(email);
            return true;
        }

        return false;
    }

    public static implicit operator string(EmailAddress email) => email.Address;
}
