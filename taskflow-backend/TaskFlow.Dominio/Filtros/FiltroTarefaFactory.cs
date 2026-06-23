using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public abstract class FiltroTarefaFactory
{
    public abstract IFiltroTarefaStrategy CriarStrategy();

    public static FiltroTarefaFactory? ObterFactory(EnumOrdenacaoTarefa? ordenacao)
    {
        return ordenacao switch
        {
            EnumOrdenacaoTarefa.PorData          => new OrdenacaoFactory(ordenacao.Value),
            EnumOrdenacaoTarefa.PorPrioridade    => new OrdenacaoFactory(ordenacao.Value),
            EnumOrdenacaoTarefa.PorTitulo        => new OrdenacaoFactory(ordenacao.Value),
            EnumOrdenacaoTarefa.ApenasAtrasadas  => new FiltragemFactory(ordenacao.Value),
            EnumOrdenacaoTarefa.ApenasConcluidas => new FiltragemFactory(ordenacao.Value),
            _                                    => null
        };
    }
}
