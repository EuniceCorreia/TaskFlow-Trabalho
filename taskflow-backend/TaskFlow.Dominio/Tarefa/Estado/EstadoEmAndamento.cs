using TaskFlow.Dominio.Excecoes;

namespace TaskFlow.Dominio.Classe.Estado;

public sealed class EstadoEmAndamento : IEstadoTarefa
{
    public EnumStatusTarefa Status => EnumStatusTarefa.EmAndamento;

    public IEstadoTarefa Iniciar() =>
        throw new RegraDeNegocioException("A tarefa ja esta em andamento.");

    public IEstadoTarefa Concluir() => new EstadoConcluida();

    public IEstadoTarefa Cancelar() => new EstadoCancelada();

    public IEstadoTarefa Reabrir() => new EstadoPendente();
}
