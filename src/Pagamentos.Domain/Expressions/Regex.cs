using System.Text.RegularExpressions;

namespace Pagamentos.Domain.Expressions;

public partial class Expression
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public static partial Regex ValidEmail();
}
