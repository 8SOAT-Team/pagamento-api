﻿namespace Pagamento.Domain.Entities;

public class InvalidEmailArgumentException : InvalidArgumentException
{
    private const string _errorMessage = "Email não está em um formato válido.";

    public InvalidEmailArgumentException() : base(_errorMessage) { }
};
