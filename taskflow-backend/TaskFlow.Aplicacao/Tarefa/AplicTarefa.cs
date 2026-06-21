using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Aplicacao.IAplic;
using TaskFlow.Aplicacao.Observadores;
using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.Filtros;
using TaskFlow.Dominio.IRep;
using TaskFlow.Dominio.IServ;
using TaskFlow.Dominio.Observadores;

namespace TaskFlow.Aplicacao.Aplic;

public sealed class TarefaAplic : ITarefaAplic
{
    private readonly ITarefaRep _tarefaRep;
    private readonly ITarefaServ _tarefaServ;
    private readonly IMapperTarefa _mapperTarefa;
    private readonly IGerenciadorNotificacoes _gerenciadorNotificacoes;

    public TarefaAplic(ITarefaRep tarefaRep, ITarefaServ tarefaServ, IMapperTarefa mapperTarefa, IGerenciadorNotificacoes gerenciadorNotificacoes)
    {
        _tarefaRep = tarefaRep;
        _tarefaServ = tarefaServ;
        _mapperTarefa = mapperTarefa;
        _gerenciadorNotificacoes = gerenciadorNotificacoes;
    }

    public async Task<TarefaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        return tarefa is null ? null : _mapperTarefa.Mapear(tarefa);
    }

    public async Task<IReadOnlyList<TarefaDto>> ListarAsync(EnumStatusTarefa? status, EnumOrdenacaoTarefa? ordenacao, CancellationToken cancellationToken = default)
    {
        var tarefas = await _tarefaRep.ListarAsync(status, cancellationToken);

        // Strategy: aplica ordenacao/filtro se solicitado
        var strategy = FiltroTarefaStrategyFactory.Criar(ordenacao);
        IEnumerable<Tarefa> resultado = strategy is not null
            ? strategy.Aplicar(tarefas)
            : tarefas;

        // Observer: verifica vencimento das tarefas ativas
        foreach (var tarefa in resultado.Where(t =>
            t.Status != EnumStatusTarefa.Concluida &&
            t.Status != EnumStatusTarefa.Cancelada))
        {
            VerificarVencimento(tarefa);
        }

        return resultado.Select(_mapperTarefa.Mapear).ToArray();
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

        _gerenciadorNotificacoes.Notificar(tarefa, EventoTarefa.Criada);
        VerificarVencimento(tarefa);

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

    public async Task<TarefaDto?> IniciarAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        if (tarefa is null)
        {
            return null;
        }

        _tarefaServ.Iniciar(tarefa);
        _tarefaRep.Atualizar(tarefa);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        VerificarVencimento(tarefa);

        return _mapperTarefa.Mapear(tarefa);
    }

    public async Task<TarefaDto?> ConcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        if (tarefa is null)
        {
            return null;
        }

        _tarefaServ.Concluir(tarefa);
        _tarefaRep.Atualizar(tarefa);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        _gerenciadorNotificacoes.Notificar(tarefa, EventoTarefa.Concluida);

        return _mapperTarefa.Mapear(tarefa);
    }

    public async Task<TarefaDto?> CancelarAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        if (tarefa is null)
        {
            return null;
        }

        _tarefaServ.Cancelar(tarefa);
        _tarefaRep.Atualizar(tarefa);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        _gerenciadorNotificacoes.Notificar(tarefa, EventoTarefa.Cancelada);

        return _mapperTarefa.Mapear(tarefa);
    }

    public async Task<TarefaDto?> ReabrirAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRep.ObterPorIdAsync(id, cancellationToken);
        if (tarefa is null)
        {
            return null;
        }

        _tarefaServ.Reabrir(tarefa);
        _tarefaRep.Atualizar(tarefa);
        await _tarefaRep.SalvarAlteracoesAsync(cancellationToken);

        return _mapperTarefa.Mapear(tarefa);
    }

    private void VerificarVencimento(Tarefa tarefa)
    {
        var diasRestantes = (tarefa.DataEntrega.Date - DateTime.UtcNow.Date).TotalDays;
        if (diasRestantes < 0)
            _gerenciadorNotificacoes.Notificar(tarefa, EventoTarefa.Atrasada);
        else if (diasRestantes <= 2)
            _gerenciadorNotificacoes.Notificar(tarefa, EventoTarefa.ProximaDoVencimento);
    }
}
