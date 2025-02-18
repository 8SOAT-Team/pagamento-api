using System.Net;
using Microsoft.AspNetCore.Mvc;
using Pagamentos.Adapters.Controllers;
using Pagamentos.Adapters.Types;
using Pagamentos.Api.Pagamento;
using Pagamentos.Apps.UseCases.Dtos;
using Pagamentos.Domain.Entities;

namespace Pagamentos.Api.Endpoints;

public static class PagamentoExtensions
{
    private const string PagamentoTag = "Pagamentos";

    public static void AddEndpointPagamentos(this WebApplication app)
    {
        app.MapPost("/pagamento/pedido/{pedidoId:guid}", async (
                [FromServices] IPagamentoController pagamentoController,
                [FromRoute] Guid pedidoId,
                [FromBody] NovoPagamentoRequest request) =>
            {
                var useCaseResult = await pagamentoController.IniciarPagamento(new IniciarPagamentoDto()
                {
                    PedidoId = pedidoId,
                    MetodoDePagamento = (MetodoDePagamento)request.MetodoDePagamento,
                    Itens = request.Itens.Select(i => new IniciarPagamentoItemDto()
                    {
                        Id = i.Id,
                        Descricao = i.Descricao,
                        Titulo = i.Titulo,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    }).ToList(),
                    Pagador = new IniciarPagamentoPagadorDto()
                    {
                        Cpf = request.Pagador?.Cpf!,
                        Email = request.Pagador?.Email!,
                        Nome = request.Pagador?.Nome!,
                    }
                });
                return useCaseResult.GetResult();
            }).WithTags(PagamentoTag)
            .WithSummary("Inicialize um pagamento de um pedido.")
            .Produces<PagamentoResponseDto>((int)HttpStatusCode.Created)
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
            .Produces<PagamentoResponseDto>()
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
            .Produces<PagamentoResponseDto>((int)HttpStatusCode.OK)
            .Produces<AppBadRequestProblemDetails>((int)HttpStatusCode.BadRequest)
            .Produces((int)HttpStatusCode.NotFound)
            .WithOpenApi();
    }
}