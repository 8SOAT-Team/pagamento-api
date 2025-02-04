using Pagamento.Apps.UseCases;

namespace Pagamento.Adapters.Gateways;

public class PagamentoGatewayCache(IPagamentoGateway nextExecution, ICacheContext cache) : CacheGateway(cache), IPagamentoGateway
{
    private readonly ICacheContext cache = cache;

    private static readonly Dictionary<string, (string cacheKey, bool InvalidateCacheOnChanges)> _cacheKeys = new()
    {
        [nameof(FindPagamentoByPedidoIdAsync)] = ($"{nameof(PagamentoGatewayCache)}:{nameof(FindPagamentoByPedidoIdAsync)}", false),
        [nameof(GetByIdAsync)] = ($"{nameof(PagamentoGatewayCache)}:{nameof(GetByIdAsync)}", false),
        [nameof(UpdatePagamentoAsync)] = ($"{nameof(PagamentoGatewayCache)}:{nameof(UpdatePagamentoAsync)}", false),
    };

    protected override Dictionary<string, (string cacheKey, bool InvalidateCacheOnChanges)> CacheKeys => _cacheKeys;

    public async Task<List<Domain.Entities.Pagamento>> FindPagamentoByPedidoIdAsync(Guid pedidoId)
    {
        var (cacheKey, _) = CacheKeys[nameof(FindPagamentoByPedidoIdAsync)];
        var key = $"{cacheKey}:{pedidoId}";

        var result = await cache.GetItemByKeyAsync<List<Domain.Entities.Pagamento>>(key);

        if (result.HasValue)
        {
            return result.Value!;
        }

        var item = await nextExecution.FindPagamentoByPedidoIdAsync(pedidoId);
        _ = await cache.SetNotNullStringByKeyAsync(key, item);

        return item;
    }

    public async Task<Domain.Entities.Pagamento?> GetByIdAsync(Guid id)
    {
        var (cacheKey, _) = CacheKeys[nameof(GetByIdAsync)];
        var key = $"{cacheKey}:{id}";

        var result = await cache.GetItemByKeyAsync<Domain.Entities.Pagamento>(key);

        if (result.HasValue)
        {
            return result.Value;
        }

        var item = await nextExecution.GetByIdAsync(id);
        _ = await cache.SetNotNullStringByKeyAsync(key, item);

        return item;
    }


    public async Task<Domain.Entities.Pagamento> UpdatePagamentoAsync(Domain.Entities.Pagamento pagamento)
    {
        var pagamentoAtualizado = await nextExecution.UpdatePagamentoAsync(pagamento);

        var (cacheKey, _) = CacheKeys[nameof(UpdatePagamentoAsync)];
        var key = $"{cacheKey}:{pagamento.Id}";
        await cache.InvalidateCacheAsync(key);
        await cache.SetNotNullStringByKeyAsync(key, pagamentoAtualizado);

        return pagamentoAtualizado;
    }
}
