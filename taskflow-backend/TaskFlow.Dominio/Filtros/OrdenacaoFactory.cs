using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public sealed class OrdenacaoFactory : FiltroTarefaFactory
{
    private readonly EnumOrdenacaoTarefa _tipo;

    public OrdenacaoFactory(EnumOrdenacaoTarefa tipo)
    {
        _tipo = tipo;
    }

    public override IFiltroTarefaStrategy CriarStrategy()
    {
        return _tipo switch
        {
            EnumOrdenacaoTarefa.PorData       => new OrdenarPorDataStrategy(),
            EnumOrdenacaoTarefa.PorPrioridade => new OrdenarPorPrioridadeStrategy(),
            EnumOrdenacaoTarefa.PorTitulo     => new OrdenarPorTituloStrategy(),
            _ => throw new InvalidOperationException($"Tipo de ordenacao nao suportado: {_tipo}")
        };
    }
}
