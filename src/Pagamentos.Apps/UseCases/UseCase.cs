using System.Diagnostics.CodeAnalysis;
using CleanArch.UseCase;
using CleanArch.UseCase.Faults;
using CleanArch.UseCase.Options;
using Pagamentos.Domain.Exceptions;

namespace Pagamentos.Apps.UseCases;

[ExcludeFromCodeCoverage]
public static class UseCaseExtension
{
    public static Task<Any<TOut>> ResolveAsync<TLogContext, TCommand, TOut>(
        this UseCase<TLogContext, TCommand, TOut> usecase)
        where TOut : class
        where TCommand : Empty<object>, new()
    {
        var r = usecase.ResolveAsync(new TCommand());
        return r;
    }
}

[ExcludeFromCodeCoverage]
public abstract class UseCase<TLogContext, TCommand, TOut>(ILogger<TLogContext> logger)
    : UseCaseBase<TLogContext, TCommand, TOut>(logger) where TOut : class
{
    protected override bool ThrowExceptionOnFailure => true;

    public override async Task<Any<TOut>> ResolveAsync(TCommand command)
    {
        try
        {
            var resolveResult = await base.ResolveAsync(command);
            return resolveResult;
        }
        catch (DomainExceptionValidation dev)
        {
            AddError(new UseCaseError(UseCaseErrorType.BadRequest, dev.Message));
            Logger.LogError("Domain Exception {mensagem} {innerException}", dev.Message, dev.InnerException);
        }
        catch (Exception ex)
        {
            AddError(new UseCaseError(UseCaseErrorType.InternalError, ex.Message));
            Logger.LogError("Exception generica {mensagem} {innerException}", ex.Message, ex.InnerException);
        }

        return Any<TOut>.Empty;
    }
}