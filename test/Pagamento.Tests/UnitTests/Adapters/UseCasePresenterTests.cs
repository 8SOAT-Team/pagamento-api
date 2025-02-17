using CleanArch.UseCase.Faults;
using Pagamento.Adapters.Presenters;

namespace Pagamento.Tests.UnitTests.Adapters;
public class UseCasePresenterTests
{
    [Fact]
    public void AdaptUseCaseError_ShouldReturnAppProblemDetails()
    {
        // Arrange
        var useCaseError = new UseCaseError(UseCaseErrorType.BadRequest, "Erro ao executar caso de uso TesteUseCase");
        string title = "Erro de Validação";
        string useCaseName = "TesteUseCase";
        string entityId = "123";

        // Act
        var result = useCaseError.AdaptUseCaseError(title, useCaseName, entityId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.Equal("Erro ao executar caso de uso TesteUseCase", result.Detail);
        Assert.Equal(useCaseError.Code.ToString(), result.Status);
        Assert.Equal(useCaseError.Description, result.Detail);
        Assert.Equal(entityId, result.Instance);
    }

    [Fact]
    public void AdaptUseCaseErrors_ShouldReturnListOfAppProblemDetails()
    {
        // Arrange
        var errors = new List<UseCaseError>
        {
            new UseCaseError(UseCaseErrorType.BadRequest, "Erro 1"),
            new UseCaseError(UseCaseErrorType.InternalError, "Erro 2")
        };
        string title = "Erro ao processar";
        string useCaseName = "TesteUseCase";
        string entityId = "456";

        // Act
        var result = errors.AdaptUseCaseErrors(title, useCaseName, entityId).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.Equal(title, item.Title));
        Assert.Equal("Erro ao executar caso de uso TesteUseCase", result[0].Detail);
        Assert.Equal("Erro ao executar caso de uso TesteUseCase", result[1].Detail);
        Assert.Equal(errors[0].Description, result[0].Detail);
        Assert.Equal(errors[1].Description, result[1].Detail);
        Assert.Equal(entityId, result[0].Instance);
        Assert.Equal(entityId, result[1].Instance);
    }
}