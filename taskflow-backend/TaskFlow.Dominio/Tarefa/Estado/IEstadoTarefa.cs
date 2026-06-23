namespace TaskFlow.Dominio.Classe.Estado;

public interface IEstadoTarefa
{
    EnumStatusTarefa Status { get; }
    public IEstadoTarefa Pausar();
    public IEstadoTarefa Retomar();
    public IEstadoTarefa Concluir();
    public IEstadoTarefa Reabrir();
}
