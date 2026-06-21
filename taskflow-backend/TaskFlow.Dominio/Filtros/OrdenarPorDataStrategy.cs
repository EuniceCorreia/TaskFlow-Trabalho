using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public sealed class OrdenarPorDataStrategy : IFiltroTarefaStrategy
{
    public IEnumerable<Tarefa> Aplicar(IEnumerable<Tarefa> tarefas) =>
        tarefas.OrderBy(t => t.DataEntrega);
}
