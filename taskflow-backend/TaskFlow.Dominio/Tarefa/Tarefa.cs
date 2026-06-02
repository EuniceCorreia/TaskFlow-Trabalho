namespace TaskFlow.Dominio.Classe;

public class Tarefa
{
    public int Id { get; private set; }
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public string Disciplina { get; private set; }
    public EnumPrioridadeTarefa Prioridade { get; private set; }
    public DateTime DataEntrega { get; private set; }
    public bool Concluida { get; private set; }
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
        Concluida = false;
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

    public void MarcarComoConcluida()
    {
        if (Concluida)
        {
            return;
        }

        Concluida = true;
        AtualizadaEm = DateTime.UtcNow;
    }

    public void MarcarComoPendente()
    {
        if (!Concluida)
        {
            return;
        }

        Concluida = false;
        AtualizadaEm = DateTime.UtcNow;
    }
}
