using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Aplicacao.IAplic;
using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.IRep;
using TaskFlow.Dominio.IServ;

namespace TaskFlow.Aplicacao.Aplic;

public sealed class TarefaAplic : ITarefaAplic
{
    private readonly ITarefaRep _tarefaRep;
    private readonly ITarefaServ _tarefaServ;
    private readonly IMapperTarefa _mapperTarefa;

    public TarefaAplic(ITarefaRep tarefaRep, ITarefaServ tarefaServ, IMapperTarefa mapperTarefa)
    {
        _tarefaRep = tarefaRep;
        _tarefaServ = tarefaServ;
        _mapperTarefa = mapperTarefa;
    }

    public async Task<TarefaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        return tarefa is null ? null : _mapperTarefa.Mapear(tarefa);
    }

    public async Task<IReadOnlyList<TarefaDto>> ListarAsync( EnumStatusTarefa? status, CancellationToken cancellationToken = default)
    {
        var tarefas = await _tarefaRep.ListarAsync(status, cancellationToken);
        return tarefas.Select(_mapperTarefa.Mapear).ToArray();
    }

    public async Task<TarefaDto> CriarAsync(TarefaCriarDto dto, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaServ.CriarAsync(
            dto.Titulo,
            dto.Descricao,
            dto.Disciplina,
            dto.Prioridade,
            dto.DataEntrega,
            cancellationToken);

        await _tarefaRep.AdicionarAsync(tarefa, cancellationToken);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        return _mapperTarefa.Mapear(tarefa);
    }

    public async Task<TarefaDto?> AtualizarAsync(int id, TarefaAtualizarDto dto, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        if (tarefa is null)
        {
            return null;
        }

        await _tarefaServ.AtualizarAsync(
            tarefa,
            dto.Titulo,
            dto.Descricao,
            dto.Disciplina,
            dto.Prioridade,
            dto.DataEntrega,
            cancellationToken);

        _tarefaRep.Atualizar(tarefa);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        return _mapperTarefa.Mapear(tarefa);
    }

    public async Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        if (tarefa is null)
        {
            return false;
        }

        _tarefaRep.Remover(tarefa);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        return true;
    }

    public async Task<TarefaDto?> MarcarComoConcluidaAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        if (tarefa is null)
        {
            return null;
        }

        _tarefaServ.MarcarComoConcluida(tarefa);
        _tarefaRep.Atualizar(tarefa);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        return _mapperTarefa.Mapear(tarefa);
    }

    public async Task<TarefaDto?> MarcarComoPendenteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        if (tarefa is null)
        {
            return null;
        }

        _tarefaServ.MarcarComoPendente(tarefa);
        _tarefaRep.Atualizar(tarefa);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        return _mapperTarefa.Mapear(tarefa);
    }
   
}
