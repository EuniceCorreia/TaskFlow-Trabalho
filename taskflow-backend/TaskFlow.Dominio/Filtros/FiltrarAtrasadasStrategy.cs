using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public sealed class FiltrarAtrasadasStrategy : IFiltroTarefaStrategy
{
    public IEnumerable<Tarefa> Aplicar(IEnumerable<Tarefa> tarefas) =>
        tarefas
            .Where(t => t.Status != EnumStatusTarefa.Concluida
                     && t.Status != EnumStatusTarefa.Cancelada
                     && t.DataEntrega.Date < DateTime.UtcNow.Date)
            .OrderBy(t => t.DataEntrega);
}
