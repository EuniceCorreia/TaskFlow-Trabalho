using Microsoft.Extensions.Logging;
using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.Observadores;

namespace TaskFlow.Aplicacao.Observadores;

public sealed class LogSistemaObservador : IObservadorTarefa
{
    private readonly ILogger<LogSistemaObservador> _logger;

    public LogSistemaObservador(ILogger<LogSistemaObservador> logger)
    {
        _logger = logger;
    }

    public void Notificar(Tarefa tarefa, EventoTarefa evento)
    {
        _logger.LogInformation(
            "[LOG] Evento={Evento} | Id={Id} | Titulo={Titulo} | Status={Status} | DataEntrega={DataEntrega:dd/MM/yyyy} | Momento={Momento:yyyy-MM-dd HH:mm:ss}",
            evento, tarefa.Id, tarefa.Titulo, tarefa.Status, tarefa.DataEntrega, DateTime.UtcNow);
    }
}
