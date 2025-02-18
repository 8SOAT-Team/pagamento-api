namespace Pagamentos.Adapters.Gateways;

public abstract class CacheGateway(ICacheContext cache)
{
    protected abstract Dictionary<string, (string cacheKey, bool InvalidateCacheOnChanges)> CacheKeys { get; }
}
