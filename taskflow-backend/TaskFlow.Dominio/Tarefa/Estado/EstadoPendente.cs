using TaskFlow.Dominio.Excecoes;

namespace TaskFlow.Dominio.Classe.Estado;

public sealed class EstadoPendente : IEstadoTarefa
{
    public EnumStatusTarefa Status =>  EnumStatusTarefa.Pendente;

    public IEstadoTarefa Iniciar()
    {
        return new EstadoEmAndamento();
    }

    public IEstadoTarefa Concluir()
    {
        throw new RegraDeNegocioException("Inicie a tarefa antes de conclui-la.");
    }

    public IEstadoTarefa Cancelar()
    {
        return new EstadoCancelada();
    }

    public IEstadoTarefa Reabrir()
    {
        throw new RegraDeNegocioException("A tarefa ja esta pendente.");
    }
}
