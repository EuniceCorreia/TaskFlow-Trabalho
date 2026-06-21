namespace TaskFlow.Dominio.Classe.Estado;

public interface IEstadoTarefa
{
    EnumStatusTarefa Status { get; }
    IEstadoTarefa Iniciar();
    IEstadoTarefa Concluir();
    IEstadoTarefa Cancelar();
    IEstadoTarefa Reabrir();
}
