namespace TaskFlow.Dominio.Classe.Estado;

public interface IEstadoTarefa
{
    EnumStatusTarefa Status { get; }
    public IEstadoTarefa Iniciar();
    public IEstadoTarefa Concluir();
    public IEstadoTarefa Cancelar();
    public IEstadoTarefa Reabrir();
}
