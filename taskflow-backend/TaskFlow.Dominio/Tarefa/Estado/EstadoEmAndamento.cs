using TaskFlow.Dominio.Excecoes;

namespace TaskFlow.Dominio.Classe.Estado;

public sealed class EstadoEmAndamento : IEstadoTarefa
{
    public EnumStatusTarefa Status => EnumStatusTarefa.EmAndamento;

    public IEstadoTarefa Iniciar()
    { 
        throw new RegraDeNegocioException("A tarefa ja esta em andamento.");
    }

    public IEstadoTarefa Concluir()
    {
        return new EstadoConcluida();
    }

    public IEstadoTarefa Cancelar()
    {
        return new EstadoCancelada();
    }

    public IEstadoTarefa Reabrir()
    {
        return new EstadoPendente();
    }
}
