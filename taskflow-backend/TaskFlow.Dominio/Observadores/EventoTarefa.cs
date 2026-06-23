namespace TaskFlow.Dominio.Observadores;

public enum EventoTarefa
{
    Criada = 1,
    Concluida = 2,
    Pausada = 3,
    ProximaDoVencimento = 4,
    Atrasada = 5
}
