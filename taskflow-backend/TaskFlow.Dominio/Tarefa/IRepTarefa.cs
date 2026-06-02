using TaskFlow.Dominio.Classe;

namespace TaskFlow.Dominio.IRep;

public interface ITarefaRep
{
    Task<Tarefa?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tarefa>> ListarAsync(EnumStatusTarefa? status, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken = default);
    void Atualizar(Tarefa tarefa);
    void Remover(Tarefa tarefa);
    Task<bool> ExisteTituloNaDisciplinaAsync(string titulo, string disciplina, int? ignorarId = null, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
