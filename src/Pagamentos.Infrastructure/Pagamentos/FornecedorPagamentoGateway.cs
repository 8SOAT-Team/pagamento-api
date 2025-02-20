using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;
using Pagamentos.Infrastructure.Configurations;

namespace Pagamentos.Infrastructure.Pagamentos;

public class FornecedorPagamentoGateway(
    PaymentClient paymentClient,
    PreferenceClient preferenceClient) : IFornecedorPagamentoGateway
{
    public async Task<FornecedorCriarPagamentoResponseDto> IniciarPagamento(IniciarPagamentoDto request,
        CancellationToken cancellationToken = default)
    {
        var options = GetRequestOptions(EnvConfig.PagamentoFornecedorAccessToken);
        var preferenceRequest = new PreferenceRequest()
        {
            Items = request.Itens.Select(item => new PreferenceItemRequest()
            {
                Id = item.Id.ToString(),
                Title = item.Titulo,
                Description = item.Descricao,
                Quantity = item.Quantidade,
                UnitPrice = item.PrecoUnitario,
                CurrencyId = "BRL",
            }).ToList(),
            Payer = string.IsNullOrEmpty(request.Pagador?.Email)
                ? null
                : new PreferencePayerRequest
                {
                    Email = request.Pagador?.Email,
                },
            BackUrls = new PreferenceBackUrlsRequest()
            {
                Success = "https://fastorder.com.br/success",
                Failure = "https://fastorder.com.br/failure",
                Pending = "https://fastorder.com.br/pending",
            },
            NotificationUrl = EnvConfig.PagamentoWebhookUrl.AbsoluteUri,
            AutoReturn = "approved",
            ExternalReference = request.PedidoId.ToString(),
        };

        var response = await preferenceClient.CreateAsync(preferenceRequest, options, cancellationToken);
        return new FornecedorCriarPagamentoResponseDto(response.Id!, response.InitPoint);
    }

    public async Task<FornecedorGetPagamentoResponseDto> ObterPagamento(string idExterno,
        CancellationToken cancellationToken = default)
    {
        var options = GetRequestOptions(EnvConfig.PagamentoFornecedorAccessToken);
        var response = await paymentClient.GetAsync(long.Parse(idExterno), options, cancellationToken);

        return new FornecedorGetPagamentoResponseDto(response.ExternalReference, GetStatusPagamento(response.Status));
    }

    private static string GetPaymentMethodType(MetodoDePagamento metodoDePagamento)
        => metodoDePagamento switch
        {
            MetodoDePagamento.Pix => "pix",
            MetodoDePagamento.Cartao or MetodoDePagamento.Master or MetodoDePagamento.Visa => "credit_card",
            _ => "pix",
        };

    private static StatusPagamento GetStatusPagamento(string status)
        => status switch
        {
            "approved" => StatusPagamento.Autorizado,
            "rejected" => StatusPagamento.Rejeitado,
            "pending" => StatusPagamento.Pendente,
            "cancelled" => StatusPagamento.Cancelado,
            _ => StatusPagamento.Pendente,
        };

    private RequestOptions GetRequestOptions(string token)
    {
        var opt = new RequestOptions()
        {
            AccessToken = token
        };
        opt.CustomHeaders.Add("x-idempotencyid", Guid.NewGuid().ToString());
        return opt;
    }
}