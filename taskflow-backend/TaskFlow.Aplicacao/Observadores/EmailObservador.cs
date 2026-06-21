using Microsoft.Extensions.Logging;
using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.Observadores;

namespace TaskFlow.Aplicacao.Observadores;

public sealed class EmailObservador : IObservadorTarefa
{
    private readonly ILogger<EmailObservador> _logger;

    public EmailObservador(ILogger<EmailObservador> logger)
    {
        _logger = logger;
    }

    public void Notificar(Tarefa tarefa, EventoTarefa evento)
    {
        if (evento is not (EventoTarefa.ProximaDoVencimento or EventoTarefa.Atrasada))
            return;

        var assunto = evento switch
        {
            EventoTarefa.ProximaDoVencimento => $"Tarefa '{tarefa.Titulo}' vence em breve ({tarefa.DataEntrega:dd/MM/yyyy})",
            EventoTarefa.Atrasada => $"Tarefa '{tarefa.Titulo}' esta atrasada desde {tarefa.DataEntrega:dd/MM/yyyy}",
            _ => string.Empty
        };

        _logger.LogInformation("[E-MAIL SIMULADO] Para: usuario@taskflow.com | Assunto: {Assunto}", assunto);
    }
}
