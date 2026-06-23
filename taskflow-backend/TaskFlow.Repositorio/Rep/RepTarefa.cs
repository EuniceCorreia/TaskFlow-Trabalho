using Microsoft.EntityFrameworkCore;
using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.IRep;
using TaskFlow.Repositorio.EF;

namespace TaskFlow.Repositorio.Rep;

public sealed class TarefaRep : ITarefaRep
{
    private readonly TaskFlowDbContext _context;

    public TarefaRep(TaskFlowDbContext context)
    {
        _context = context;
    }

    public Task<Tarefa?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Tarefas.FirstOrDefaultAsync( t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Tarefa>> ListarAsync(EnumStatusTarefa? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Tarefas.AsNoTracking();

        query = status switch
        {
            EnumStatusTarefa.Pendente => query.Where( t => t.Status == EnumStatusTarefa.Pendente),
            EnumStatusTarefa.Pausada => query.Where( t => t.Status == EnumStatusTarefa.Pausada),
            EnumStatusTarefa.Concluida => query.Where( t => t.Status == EnumStatusTarefa.Concluida),
            _ => query
        };

        return await query
            .OrderBy( t => t.Status == EnumStatusTarefa.Concluida)
            .ThenBy( t => t.DataEntrega)
            .ThenByDescending( t => t.Prioridade)
            .ThenBy( t => t.Disciplina)
            .ToListAsync(cancellationToken);
    }

    public Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken = default)
    {
        return _context.Tarefas.AddAsync(tarefa, cancellationToken).AsTask();
    }

    public void Atualizar(Tarefa tarefa)
    {
        _context.Tarefas.Update(tarefa);
    }

    public void Remover(Tarefa tarefa)
    {
        _context.Tarefas.Remove(tarefa);
    }

    public Task<bool> ExisteTituloNaDisciplinaAsync(string titulo, string disciplina, int? ignorarId = null, CancellationToken cancellationToken = default)
    {
        var tituloNormalizado = titulo.Trim().ToUpper();
        var disciplinaNormalizada = disciplina.Trim().ToUpper();

        return _context.Tarefas
            .AsNoTracking()
            .AnyAsync( t => t.Titulo.ToUpper() == tituloNormalizado &&
                            t.Disciplina.ToUpper() == disciplinaNormalizada &&
                            (!ignorarId.HasValue || t.Id != ignorarId.Value),
                            cancellationToken);
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
