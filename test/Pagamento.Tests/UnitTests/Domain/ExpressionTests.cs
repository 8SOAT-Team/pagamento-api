using System.Text.RegularExpressions;

namespace Pagamento.Tests.UnitTests.Domain;
public class ExpressionTests
{
    [Theory]
    [InlineData("test@example.com", true)] // Testa um e-mail válido
    [InlineData("invalid-email", false)]  // Testa um e-mail inválido
    public void ValidEmail_ShouldValidateCorrectly(string email, bool expected)
    {
        // Arrange
        var regex = Expression.ValidEmail();

        // Act
        var result = regex.IsMatch(email);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("123.456.789-00", true)] // Testa CPF com ponto e hífen
    [InlineData("12345678900", true)]     // Testa CPF apenas com números
    [InlineData("12345", false)]          // Testa CPF com formato inválido
    public void HasCpfLength_ShouldValidateCorrectly(string cpf, bool expected)
    {
        // Arrange
        var regex = Expression.HasCpfLength();

        // Act
        var result = regex.IsMatch(cpf);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1234567890", false)]      // Testa uma string com apenas números (espera false)
    [InlineData("123-456-789", false)]     // Testa uma string com hífen (espera false)
    [InlineData("abcdef", true)]           // Testa uma string com apenas letras (espera true)
    [InlineData("abc123", true)]           // Testa uma string com letras e números (espera true)
    public void DigitsOnly_ShouldValidateCorrectly(string input, bool expected)
    {
        // Arrange
        var regex = Expression.DigitsOnly();

        // Act
        var result = regex.IsMatch(input);

        // Assert
        Assert.Equal(expected, result);
    }
}

public static class Expression
{
    public static Regex ValidEmail()
    {
        return new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public static Regex HasCpfLength()
    {
        return new Regex(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$");
    }

    public static Regex DigitsOnly()
    {
        return new Regex(@"^\D*$");
    }
}