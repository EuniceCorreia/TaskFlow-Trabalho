using TaskFlow.Dominio.Excecoes;

namespace TaskFlow.API.Middlewares;

public sealed class TratamentoExcecoesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TratamentoExcecoesMiddleware> _logger;

    public TratamentoExcecoesMiddleware(RequestDelegate next, ILogger<TratamentoExcecoesMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RegraDeNegocioException exception)
        {
            await Results.Problem(
                title: "Regra de negocio violada",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest)
                .ExecuteAsync(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro nao tratado durante a requisicao.");

            await Results.Problem(
                title: "Erro interno",
                detail: "Nao foi possivel processar a requisicao.",
                statusCode: StatusCodes.Status500InternalServerError)
                .ExecuteAsync(context);
        }
    }
}
