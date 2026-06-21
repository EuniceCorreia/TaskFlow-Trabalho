using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public static class FiltroTarefaStrategyFactory
{
    public static IFiltroTarefaStrategy? Criar(EnumOrdenacaoTarefa? ordenacao) => ordenacao switch
    {
        EnumOrdenacaoTarefa.PorData => new OrdenarPorDataStrategy(),
        EnumOrdenacaoTarefa.PorPrioridade => new OrdenarPorPrioridadeStrategy(),
        EnumOrdenacaoTarefa.PorTitulo => new OrdenarPorTituloStrategy(),
        EnumOrdenacaoTarefa.ApenasAtrasadas => new FiltrarAtrasadasStrategy(),
        EnumOrdenacaoTarefa.ApenasConcluidas => new FiltrarConcluidasStrategy(),
        _ => null
    };
}
