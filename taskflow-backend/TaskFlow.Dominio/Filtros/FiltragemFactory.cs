using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.Filtros;

public sealed class FiltragemFactory : FiltroTarefaFactory
{
    private readonly EnumOrdenacaoTarefa _tipo;

    public FiltragemFactory(EnumOrdenacaoTarefa tipo)
    {
        _tipo = tipo;
    }

    public override IFiltroTarefaStrategy CriarStrategy()
    {
        return _tipo switch
        {
            EnumOrdenacaoTarefa.ApenasAtrasadas  => new FiltrarAtrasadasStrategy(),
            EnumOrdenacaoTarefa.ApenasConcluidas => new FiltrarConcluidasStrategy(),
            _ => throw new InvalidOperationException($"Tipo de filtragem nao suportado: {_tipo}")
        };
    }
}
