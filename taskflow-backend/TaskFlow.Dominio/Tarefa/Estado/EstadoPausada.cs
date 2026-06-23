using TaskFlow.Dominio.Excecoes;

namespace TaskFlow.Dominio.Classe.Estado;

public sealed class EstadoPausada : IEstadoTarefa
{
    public EnumStatusTarefa Status
    {
        get { return EnumStatusTarefa.Pausada; }
    }

    public IEstadoTarefa Pausar()
    {
        throw new RegraDeNegocioException("A tarefa ja esta pausada.");
    }

    public IEstadoTarefa Retomar()
    {
        return new EstadoPendente();
    }

    public IEstadoTarefa Concluir()
    {
        return new EstadoConcluida();
    }

    public IEstadoTarefa Reabrir()
    {
        throw new RegraDeNegocioException("A tarefa nao foi concluida.");
    }
}
