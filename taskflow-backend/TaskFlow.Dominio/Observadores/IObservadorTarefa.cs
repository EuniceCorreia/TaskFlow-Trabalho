using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Observadores;

public interface IObservadorTarefa
{
    void Notificar(Tarefa tarefa, EventoTarefa evento);
}
