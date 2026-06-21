using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.Observadores;

namespace TaskFlow.Aplicacao.Observadores;

public sealed class GerenciadorNotificacoes : IGerenciadorNotificacoes
{
    private readonly List<IObservadorTarefa> _observadores;

    public GerenciadorNotificacoes(IEnumerable<IObservadorTarefa> observadores)
    {
        _observadores = observadores.ToList();
    }

    public void Registrar(IObservadorTarefa observador) => _observadores.Add(observador);

    public void Remover(IObservadorTarefa observador) => _observadores.Remove(observador);

    public void Notificar(Tarefa tarefa, EventoTarefa evento)
    {
        foreach (var observador in _observadores)
            observador.Notificar(tarefa, evento);
    }
}
