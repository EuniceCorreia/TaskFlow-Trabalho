using TaskFlow.Dominio.Excecoes;

namespace TaskFlow.Dominio.Classe.Estado;

public sealed class EstadoCancelada : IEstadoTarefa
{
    public EnumStatusTarefa Status
    {
        get { return EnumStatusTarefa.Cancelada; }
    }

    public IEstadoTarefa Iniciar()
    {
        throw new RegraDeNegocioException("Nao e possivel iniciar uma tarefa cancelada.");
    }

    public IEstadoTarefa Concluir()
    {
        throw new RegraDeNegocioException("Nao e possivel concluir uma tarefa cancelada.");
    }

    public IEstadoTarefa Cancelar()
    {
        throw new RegraDeNegocioException("A tarefa ja esta cancelada.");
    }

    public IEstadoTarefa Reabrir()
    {
        return new EstadoPendente();
    }
}
