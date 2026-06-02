namespace TaskFlow.Dominio.Excecoes;

public sealed class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string message)
        : base(message)
    {
    }
}
