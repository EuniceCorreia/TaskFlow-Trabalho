using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.Aplicacao.IAplic;

public interface ITarefaAplic
{
    Task<TarefaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TarefaDto>> ListarAsync(EnumStatusTarefa? status, CancellationToken cancellationToken = default);
    Task<TarefaDto> CriarAsync(TarefaCriarDto dto, CancellationToken cancellationToken = default);
    Task<TarefaDto?> AtualizarAsync(int id, TarefaAtualizarDto dto, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default);
    Task<TarefaDto?> MarcarComoConcluidaAsync(int id, CancellationToken cancellationToken = default);
    Task<TarefaDto?> MarcarComoPendenteAsync(int id, CancellationToken cancellationToken = default);
}
