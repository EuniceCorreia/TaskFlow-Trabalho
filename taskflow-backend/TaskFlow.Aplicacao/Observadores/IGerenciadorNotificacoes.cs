using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.Observadores;

namespace TaskFlow.Aplicacao.Observadores;

public interface IGerenciadorNotificacoes
{
    void Registrar(IObservadorTarefa observador);
    void Remover(IObservadorTarefa observador);
    void Notificar(Tarefa tarefa, EventoTarefa evento);
}
