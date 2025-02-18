using CleanArch.UseCase.Faults;
using Pagamentos.Adapters.Presenters;

namespace Pagamentos.Tests.UnitTests.Adapters;
public class UseCasePresenterTests
{
    [Fact]
    public void AdaptUseCaseError_ShouldReturnAppProblemDetails()
    {
        // Arrange
        var useCaseError = new UseCaseError(UseCaseErrorType.BadRequest, "Erro ao executar caso de uso TesteUseCase");
        string title = "Erro ao executar caso de uso TesteUseCase";
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
        string title = "Erro ao executar caso de uso TesteUseCase";
        string useCaseName = "TesteUseCase";
        string entityId = "456";

        // Act
        var result = errors.AdaptUseCaseErrors(title, useCaseName, entityId).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.Equal(title, item.Title));
        

    }
}