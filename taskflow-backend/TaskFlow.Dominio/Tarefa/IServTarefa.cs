using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.IServ
{
    public interface ITarefaServ
    {
        Task<Tarefa> CriarAsync(string titulo, string descricao, string disciplina, EnumPrioridadeTarefa prioridade, DateTime dataEntrega, CancellationToken cancellationToken = default);

        Task AtualizarAsync(Tarefa tarefa, string titulo, string descricao, string disciplina, EnumPrioridadeTarefa prioridade, DateTime dataEntrega, CancellationToken cancellationToken = default);

        void MarcarComoConcluida(Tarefa tarefa);

        void MarcarComoPendente(Tarefa tarefa);
    }
}
