namespace TaskFlow.Dominio.Classe;

public enum EnumPrioridadeTarefa
{
    Baixa = 1,
    Media = 2,
    Alta = 3
}

public enum EnumStatusTarefa
{
    Pendente = 1,
    Pausada = 2,
    Concluida = 3
}

public enum EnumOrdenacaoTarefa
{
    PorData = 1,
    PorPrioridade = 2,
    PorTitulo = 3,
    ApenasAtrasadas = 4,
    ApenasConcluidas = 5
}
