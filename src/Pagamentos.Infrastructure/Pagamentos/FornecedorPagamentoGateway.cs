using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using Pagamentos.Apps.UseCases;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;
using Pagamentos.Infrastructure.Configurations;

namespace Pagamentos.Infrastructure.Pagamentos
{
    public class FornecedorPagamentoGateway(PaymentClient paymentClient,
        PreferenceClient preferenceClient,
        IPagamentoGateway pagamentoGateway) : IFornecedorPagamentoGateway
    {
        public async Task<FornecedorCriarPagamentoResponseDto> IniciarPagamento(MetodoDePagamento metodoDePagamento,
            string emailPagador, decimal valorTotal, string referenciaExternaId, Guid pedidoId, CancellationToken cancellationToken = default)
        {
            var pedido = await pagamentoGateway.FindPagamentoByPedidoIdAsync(pedidoId);

            var options = GetRequestOptions(EnvConfig.PagamentoFornecedorAccessToken);
            var preferenceRequest = new PreferenceRequest()
            {
                Payer = string.IsNullOrEmpty(emailPagador) ? null : new PreferencePayerRequest
                {
                    Email = emailPagador,
                },
                BackUrls = new PreferenceBackUrlsRequest()
                {
                    Success = "https://fastorder.com.br/success",
                    Failure = "https://fastorder.com.br/failure",
                    Pending = "https://fastorder.com.br/pending",
                },
                NotificationUrl = EnvConfig.PagamentoWebhookUrl.AbsoluteUri,
                AutoReturn = "approved",
                ExternalReference = referenciaExternaId,
            };

            var response = await preferenceClient.CreateAsync(preferenceRequest, options, cancellationToken);
            return new FornecedorCriarPagamentoResponseDto(response.Id?.ToString()!, response.InitPoint);
        }

        public async Task<FornecedorGetPagamentoResponseDto?> ObterPagamento(string idExterno, CancellationToken cancellationToken = default)
        {
            var options = GetRequestOptions(EnvConfig.PagamentoFornecedorAccessToken);
            var response = await paymentClient.GetAsync(long.Parse(idExterno), options, cancellationToken);
            var pagamentoId = response.ExternalReference;

            return new FornecedorGetPagamentoResponseDto(response.Id.ToString()!, Guid.Parse(pagamentoId), GetStatusPagamento(response.Status));
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
}
