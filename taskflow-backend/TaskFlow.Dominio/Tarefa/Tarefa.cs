using TaskFlow.Dominio.Classe.Estado;

namespace TaskFlow.Dominio.Classe;

public class Tarefa
{
    public int Id { get; private set; }
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public string Disciplina { get; private set; }
    public EnumPrioridadeTarefa Prioridade { get; private set; }
    public DateTime DataEntrega { get; private set; }
    public EnumStatusTarefa Status { get; private set; }
    public DateTime CriadaEm { get; private set; }
    public DateTime? AtualizadaEm { get; private set; }

    private Tarefa()
    {
        Titulo = string.Empty;
        Descricao = string.Empty;
        Disciplina = string.Empty;
    }

    public Tarefa(string titulo, string descricao, string disciplina, EnumPrioridadeTarefa prioridade, DateTime dataEntrega)
    {
        Titulo = titulo.Trim();
        Descricao = descricao.Trim();
        Disciplina = disciplina.Trim();
        Prioridade = prioridade;
        DataEntrega = dataEntrega;
        Status = EnumStatusTarefa.Pendente;
        CriadaEm = DateTime.UtcNow;
    }

    public void Atualizar(string titulo, string descricao, string disciplina, EnumPrioridadeTarefa prioridade, DateTime dataEntrega)
    {
        Titulo = titulo.Trim();
        Descricao = descricao.Trim();
        Disciplina = disciplina.Trim();
        Prioridade = prioridade;
        DataEntrega = dataEntrega;
        AtualizadaEm = DateTime.UtcNow;
    }

    private IEstadoTarefa ObterEstadoAtual() => Status switch
    {
        EnumStatusTarefa.Pendente => new EstadoPendente(),
        EnumStatusTarefa.EmAndamento => new EstadoEmAndamento(),
        EnumStatusTarefa.Concluida => new EstadoConcluida(),
        EnumStatusTarefa.Cancelada => new EstadoCancelada(),
        _ => throw new InvalidOperationException($"Status desconhecido: {Status}")
    };

    public void Iniciar()
    {
        Status = ObterEstadoAtual().Iniciar().Status;
        AtualizadaEm = DateTime.UtcNow;
    }

    public void Concluir()
    {
        Status = ObterEstadoAtual().Concluir().Status;
        AtualizadaEm = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        Status = ObterEstadoAtual().Cancelar().Status;
        AtualizadaEm = DateTime.UtcNow;
    }

    public void Reabrir()
    {
        Status = ObterEstadoAtual().Reabrir().Status;
        AtualizadaEm = DateTime.UtcNow;
    }
}
