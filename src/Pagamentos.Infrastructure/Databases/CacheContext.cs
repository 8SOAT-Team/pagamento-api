using System.Text.Json;
using Pagamentos.Adapters.Gateways;
using Pagamentos.Adapters.Types;
using StackExchange.Redis;

namespace Pagamentos.Infrastructure.Databases;

public class CacheContext(IConnectionMultiplexer connectionMultiplexer, JsonSerializerOptions jsonOptions)
    : ICacheContext
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task<Result<T>> GetItemByKeyAsync<T>(string key)
    {
        var item = await _database.StringGetAsync(key);

        if (item == RedisValue.Null)
        {
            return Result<T>.Empty();
        }

        var deserialized = item.ToString().TryDeserialize<T>(jsonOptions);

        return deserialized;
    }

    public async Task<Result<T>> SetOnlyIfNotNullByKeyAsync<T>(string key, T? value, int expireInSec = 3600)
        where T : class
    {
        if (value is null)
        {
            return Result<T>.Empty();
        }

        var result = await SetStringByKeyAsync(key, JsonSerializer.Serialize(value, jsonOptions), expireInSec);

        if (result.IsSucceed)
        {
            return Result<T>.Succeed(value);
        }

        return Result<T>.Failure(new AppProblemDetails("Não foi possível gravar em cache", "internal_server_error",
            "Verifique os logs", key));
    }

    public async Task<Result<ICollection<T>>> SetOnlyIfContainsItemByKeyAsync<T>(string key, ICollection<T> value,
        int expireInSec = 3600) where T : class
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value is null || value.Count == 0)
        {
            return Result<ICollection<T>>.Empty();
        }

        var result = await SetStringByKeyAsync(key, JsonSerializer.Serialize(value, jsonOptions), expireInSec);

        if (result.IsSucceed)
        {
            return Result<ICollection<T>>.Succeed(value);
        }

        return Result<ICollection<T>>.Failure(new AppProblemDetails("Não foi possível gravar em cache",
            "internal_server_error", "Verifique os logs", key));
    }


    public async Task<Result<string>> SetStringByKeyAsync(string key, string value, int expireInSec = 3600)
    {
        var response = await _database.StringSetAsync(key, value, TimeSpan.FromSeconds(expireInSec));

        if (response == RedisValue.Null)
        {
            return Result<string>.Empty();
        }

        return Result<string>.Succeed(value);
    }

    public async Task<Result<string>> InvalidateCacheAsync(string key)
    {
        var keyExists = await _database.KeyExistsAsync(key);
        if (keyExists) await _database.KeyDeleteAsync(key);
        return Result<string>.Empty();
    }
}