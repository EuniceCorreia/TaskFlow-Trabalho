using TaskFlow.Dominio.Excecoes;

namespace TaskFlow.Dominio.Classe.Estado;

public sealed class EstadoConcluida : IEstadoTarefa
{
    public EnumStatusTarefa Status
    {
        get { return EnumStatusTarefa.Concluida; }
    }

    public IEstadoTarefa Iniciar()
    {
        throw new RegraDeNegocioException("A tarefa ja foi concluida.");
    }

    public IEstadoTarefa Concluir()
    {
        throw new RegraDeNegocioException("A tarefa ja foi concluida.");
    }

    public IEstadoTarefa Cancelar()
    {
        throw new RegraDeNegocioException("Nao e possivel cancelar uma tarefa ja concluida.");
    }

    public IEstadoTarefa Reabrir()
    {
        return new EstadoPendente();
    }
}
