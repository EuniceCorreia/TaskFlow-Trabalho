using TaskFlow.Dominio.Excecoes;

namespace TaskFlow.Dominio.Classe.Estado;

public sealed class EstadoConcluida : IEstadoTarefa
{
    public EnumStatusTarefa Status
    {
        get { return EnumStatusTarefa.Concluida; }
    }

    public IEstadoTarefa Pausar()
    {
        throw new RegraDeNegocioException("Nao e possivel pausar uma tarefa ja concluida.");
    }

    public IEstadoTarefa Retomar()
    {
        throw new RegraDeNegocioException("Nao e possivel retomar uma tarefa ja concluida.");
    }

    public IEstadoTarefa Concluir()
    {
        throw new RegraDeNegocioException("A tarefa ja foi concluida.");
    }

    public IEstadoTarefa Reabrir()
    {
        return new EstadoPendente();
    }
}
