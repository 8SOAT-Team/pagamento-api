using Pagamentos.Adapters.Types;

namespace Pagamentos.Adapters.Gateways;

public interface ICacheContext
{
    Task<Result<T>> GetItemByKeyAsync<T>(string key);
    Task<Result<T>> SetOnlyIfNotNullByKeyAsync<T>(string key, T? value, int expireInSec = 3600) where T : class;
    Task<Result<ICollection<T>>> SetOnlyIfContainsItemByKeyAsync<T>(string key, ICollection<T> value,
        int expireInSec = 3600) where T : class;
    Task<Result<string>> SetStringByKeyAsync(string key, string value, int expireInSec = 3600);
    Task<Result<string>> InvalidateCacheAsync(string key);
}