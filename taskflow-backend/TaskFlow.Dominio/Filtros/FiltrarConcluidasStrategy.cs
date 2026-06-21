using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public sealed class FiltrarConcluidasStrategy : IFiltroTarefaStrategy
{
    public IEnumerable<Tarefa> Aplicar(IEnumerable<Tarefa> tarefas) =>
        tarefas
            .Where(t => t.Status == EnumStatusTarefa.Concluida)
            .OrderByDescending(t => t.AtualizadaEm);
}
