using Pagamentos.Apps.UseCases;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Adapters.Gateways;

public class PagamentoGatewayCache(IPagamentoGateway nextExecution, ICacheContext cache) : CacheGateway(cache), IPagamentoGateway
{
    private readonly ICacheContext _cache = cache;

    private static readonly Dictionary<string, (string cacheKey, bool InvalidateCacheOnChanges)> _cacheKeys = new()
    {
        [nameof(FindPagamentoByPedidoIdAsync)] = ($"{nameof(PagamentoGatewayCache)}:{nameof(FindPagamentoByPedidoIdAsync)}", false),
        [nameof(GetByIdAsync)] = ($"{nameof(PagamentoGatewayCache)}:{nameof(GetByIdAsync)}", false),
        [nameof(UpdatePagamentoAsync)] = ($"{nameof(PagamentoGatewayCache)}:{nameof(UpdatePagamentoAsync)}", false),
        [nameof(FindPagamentoByExternoIdAsync)] = ($"{nameof(PagamentoGatewayCache)}:{nameof(FindPagamentoByExternoIdAsync)}", false),
    };

    protected override Dictionary<string, (string cacheKey, bool InvalidateCacheOnChanges)> CacheKeys => _cacheKeys;

    public Task<Pagamento> CreateAsync(Pagamento pagamento)
    {
        return nextExecution.CreateAsync(pagamento);
    }

    public async Task<List<Pagamento>> FindPagamentoByPedidoIdAsync(Guid pedidoId)
    {
        var (cacheKey, _) = CacheKeys[nameof(FindPagamentoByPedidoIdAsync)];
        var key = $"{cacheKey}:{pedidoId}";

        var result = await _cache.GetItemByKeyAsync<List<Pagamento>>(key);

        if (result.HasValue)
        {
            return result.Value!;
        }

        var item = await nextExecution.FindPagamentoByPedidoIdAsync(pedidoId);
        _ = await _cache.SetOnlyIfContainsItemByKeyAsync(key, item);

        return item;
    }

    public async Task<Pagamento?> GetByIdAsync(Guid id)
    {
        var (cacheKey, _) = CacheKeys[nameof(GetByIdAsync)];
        var key = $"{cacheKey}:{id}";

        var result = await _cache.GetItemByKeyAsync<Pagamento>(key);

        if (result.HasValue)
        {
            return result.Value;
        }

        var item = await nextExecution.GetByIdAsync(id);
        _ = await _cache.SetOnlyIfNotNullByKeyAsync(key, item);

        return item;
    }


    public async Task<Pagamento> UpdatePagamentoAsync(Pagamento pagamento)
    {
        var pagamentoAtualizado = await nextExecution.UpdatePagamentoAsync(pagamento);

        var (cacheKey, _) = CacheKeys[nameof(UpdatePagamentoAsync)];
        var key = $"{cacheKey}:{pagamento.Id}";
        await _cache.InvalidateCacheAsync(key);
        await _cache.SetOnlyIfNotNullByKeyAsync(key, pagamentoAtualizado);

        return pagamentoAtualizado;
    }

    public async Task<Pagamento?> FindPagamentoByExternoIdAsync(string externoId)
    {
        var (cacheKey, _) = CacheKeys[nameof(FindPagamentoByExternoIdAsync)];
        var key = $"{cacheKey}:{externoId}";

        var result = await _cache.GetItemByKeyAsync<Pagamento>(key);

        if (result.HasValue)
        {
            return result.Value;
        }

        var item = await nextExecution.FindPagamentoByExternoIdAsync(externoId);
        _ = await _cache.SetOnlyIfNotNullByKeyAsync(key, item);

        return item;
    }
}
