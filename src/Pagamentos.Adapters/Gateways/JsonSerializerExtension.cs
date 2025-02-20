using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Pagamentos.Apps.Types;

namespace Pagamentos.Adapters.Gateways;

public static class JsonSerializerExtension
{
    public static Result<T> TryDeserialize<T>([StringSyntax(StringSyntaxAttribute.Json)] this string jsonDocument, JsonSerializerOptions? jsonOptions = null)
    {
        try
        {
            var document = JsonSerializer.Deserialize<T>(jsonDocument, jsonOptions);

            if (document is not null)
            {
                return Result<T>.Succeed(document);
            }
        }
        catch (Exception) { }

        return Result<T>.Empty();
    }
}
