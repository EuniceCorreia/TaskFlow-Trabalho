using Microsoft.Extensions.Logging;
using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.Observadores;

namespace TaskFlow.Aplicacao.Observadores;

public sealed class NotificacaoTelaObservador : IObservadorTarefa
{
    private readonly ILogger<NotificacaoTelaObservador> _logger;

    public NotificacaoTelaObservador(ILogger<NotificacaoTelaObservador> logger)
    {
        _logger = logger;
    }

    public void Notificar(Tarefa tarefa, EventoTarefa evento)
    {
        var mensagem = evento switch
        {
            EventoTarefa.Criada => $"[TELA] Nova tarefa criada: '{tarefa.Titulo}' ({tarefa.Disciplina})",
            EventoTarefa.Concluida => $"[TELA] Tarefa concluida: '{tarefa.Titulo}'",
            EventoTarefa.Cancelada => $"[TELA] Tarefa cancelada: '{tarefa.Titulo}'",
            EventoTarefa.ProximaDoVencimento => $"[TELA] ATENCAO: Tarefa '{tarefa.Titulo}' vence em breve ({tarefa.DataEntrega:dd/MM/yyyy})",
            EventoTarefa.Atrasada => $"[TELA] ATRASADA: Tarefa '{tarefa.Titulo}' venceu em {tarefa.DataEntrega:dd/MM/yyyy}",
            _ => $"[TELA] Evento desconhecido para tarefa '{tarefa.Titulo}'"
        };

        _logger.LogInformation("{Mensagem}", mensagem);
    }
}
