using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Aplicacao.IAplic;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.API.Facade;

public sealed class TarefaFacade : ITarefaFacade
{
    private readonly ITarefaAplic _tarefaAplic;

    public TarefaFacade(ITarefaAplic tarefaAplic)
    {
        _tarefaAplic = tarefaAplic;
    }

    public Task<TarefaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _tarefaAplic.ObterPorIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<TarefaDto>> ListarAsync(EnumStatusTarefa? status, EnumOrdenacaoTarefa? ordenacao, CancellationToken cancellationToken = default)
    {
        return _tarefaAplic.ListarAsync(status, ordenacao, cancellationToken);
    }

    public Task<TarefaDto> CriarAsync(TarefaCriarDto dto, CancellationToken cancellationToken = default)
    {
        return _tarefaAplic.CriarAsync(dto, cancellationToken);
    }

    public Task<TarefaDto?> AtualizarAsync(int id, TarefaAtualizarDto dto, CancellationToken cancellationToken = default)
    {
        return _tarefaAplic.AtualizarAsync(id, dto, cancellationToken);
    }

    public Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        return _tarefaAplic.ExcluirAsync(id, cancellationToken);
    }

    public async Task<TarefaDto?> TransicionarAsync(int id, string acao, CancellationToken cancellationToken = default)
    {
        return acao switch
        {
            "pausar"   => await _tarefaAplic.PausarAsync(id, cancellationToken),
            "retomar"  => await _tarefaAplic.RetomarAsync(id, cancellationToken),
            "concluir" => await _tarefaAplic.ConcluirAsync(id, cancellationToken),
            "reabrir"  => await _tarefaAplic.ReabrirAsync(id, cancellationToken),
            _ => throw new ArgumentException($"Acao de transicao desconhecida: {acao}")
        };
    }
}
