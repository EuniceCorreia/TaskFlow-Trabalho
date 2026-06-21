using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public sealed class OrdenarPorTituloStrategy : IFiltroTarefaStrategy
{
    public IEnumerable<Tarefa> Aplicar(IEnumerable<Tarefa> tarefas) =>
        tarefas.OrderBy(t => t.Titulo, StringComparer.OrdinalIgnoreCase);
}
