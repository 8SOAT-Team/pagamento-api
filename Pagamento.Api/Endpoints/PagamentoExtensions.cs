using System.Net;
using Microsoft.AspNetCore.Mvc;
using Pagamento.Adapters.Controllers;
using Pagamento.Adapters.Types;
using Pagamento.Api.Pagamento;
using Pagamento.Configurations;

namespace Pagamento.Api.Endpoints;

public static class PagamentoExtensions
{
    private const string PagamentoTag = "Pagamentos";
    public static void AddEndpointPagamentos(this WebApplication app)
    {
        app.MapPost("/pagamento/pedido/{pedidoId:guid}/{valorTotal}/{emailPagador}", async (
                [FromServices] IPagamentoController pagamentoController,
                [FromRoute] Guid pedidoId,
                [FromRoute] decimal ValorTotal,
                [FromRoute] string EmailPagador,
                [FromBody] NovoPagamentoDTO request) =>
            {
                var useCaseResult = await pagamentoController.IniciarPagamento(pedidoId, request.MetodoDePagamento,
                    ValorTotal, EmailPagador, EnvConfig.PagamentoWebhookUrl.AbsoluteUri, EnvConfig.PagamentoFornecedorAccessToken);
                return useCaseResult.GetResult();
            }).WithTags(PagamentoTag)
            .WithSummary("Inicialize um pagamento de um pedido.")
            .Produces<PagamentoResponseDTO>((int)HttpStatusCode.Created)
            .Produces<AppBadRequestProblemDetails>((int)HttpStatusCode.BadRequest)
            .Produces((int)HttpStatusCode.NotFound)
            .WithOpenApi();

        app.MapPatch("/pagamento/{pagamentoId:guid}", async ([FromServices] IPagamentoController pagamentoController,
           [FromRoute] Guid pagamentoId,
           [FromBody] ConfirmarPagamentoDTO request,
           HttpContext httpContext) =>
        {
            var useCaseResult = await pagamentoController.ConfirmarPagamento(pagamentoId, request.Status);
            return useCaseResult.GetResult();
        }).WithTags(PagamentoTag)
        .WithSummary("Confirma o pagamento de um pedido pelo id do pagamento.")
        .Produces<PagamentoResponseDTO>((int)HttpStatusCode.OK)
        .Produces<AppBadRequestProblemDetails>((int)HttpStatusCode.BadRequest)
        .Produces((int)HttpStatusCode.NotFound)
        .WithOpenApi();

        app.MapGet("/pagamento/pedido/{pedidoId:guid}", async ([FromServices] IPagamentoController pagamentoController,
            [FromRoute] Guid pedidoId) =>
        {
            var useCaseResult = await pagamentoController.GetPagamentoByPedidoAsync(pedidoId);
            return useCaseResult.GetResult();
        }).WithTags(PagamentoTag)
        .WithSummary("Obtenha os dados de um pagamento pelo id do pedido.")
        .Produces<PagamentoResponseDTO>((int)HttpStatusCode.OK)
        .Produces<AppBadRequestProblemDetails>((int)HttpStatusCode.BadRequest)
        .Produces((int)HttpStatusCode.NotFound)
        .WithOpenApi();
    }
}
