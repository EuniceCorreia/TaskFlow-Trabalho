namespace TaskFlow.Dominio.Observadores;

public enum EventoTarefa
{
    Criada = 1,
    Concluida = 2,
    Cancelada = 3,
    ProximaDoVencimento = 4,
    Atrasada = 5
}
