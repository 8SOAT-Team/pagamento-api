using Pagamentos.Adapters.Types;

namespace Pagamentos.Adapters.Gateways;

public interface ICacheContext
{
    Task<Result<T>> GetItemByKeyAsync<T>(string key);
    Task<Result<T>> SetNotNullStringByKeyAsync<T>(string key, T value, int expireInSec = 3600);
    Task<Result<string>> SetStringByKeyAsync(string key, string value, int expireInSec = 3600);
    Task<Result<string>> InvalidateCacheAsync(string key);
}
