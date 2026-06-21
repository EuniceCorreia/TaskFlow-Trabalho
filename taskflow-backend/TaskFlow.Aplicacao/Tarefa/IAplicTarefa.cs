using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.Aplicacao.IAplic;

public interface ITarefaAplic
{
    Task<TarefaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TarefaDto>> ListarAsync(EnumStatusTarefa? status, EnumOrdenacaoTarefa? ordenacao, CancellationToken cancellationToken = default);
    Task<TarefaDto> CriarAsync(TarefaCriarDto dto, CancellationToken cancellationToken = default);
    Task<TarefaDto?> AtualizarAsync(int id, TarefaAtualizarDto dto, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default);
    Task<TarefaDto?> IniciarAsync(int id, CancellationToken cancellationToken = default);
    Task<TarefaDto?> ConcluirAsync(int id, CancellationToken cancellationToken = default);
    Task<TarefaDto?> CancelarAsync(int id, CancellationToken cancellationToken = default);
    Task<TarefaDto?> ReabrirAsync(int id, CancellationToken cancellationToken = default);
}
