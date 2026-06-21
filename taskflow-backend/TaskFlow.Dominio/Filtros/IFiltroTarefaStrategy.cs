using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public interface IFiltroTarefaStrategy
{
    IEnumerable<Tarefa> Aplicar(IEnumerable<Tarefa> tarefas);
}
