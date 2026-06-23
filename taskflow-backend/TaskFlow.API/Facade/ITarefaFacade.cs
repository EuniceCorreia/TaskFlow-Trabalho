using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.API.Facade;

public interface ITarefaFacade
{
    Task<TarefaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TarefaDto>> ListarAsync(EnumStatusTarefa? status, EnumOrdenacaoTarefa? ordenacao, CancellationToken cancellationToken = default);
    Task<TarefaDto> CriarAsync(TarefaCriarDto dto, CancellationToken cancellationToken = default);
    Task<TarefaDto?> AtualizarAsync(int id, TarefaAtualizarDto dto, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default);
    Task<TarefaDto?> TransicionarAsync(int id, string acao, CancellationToken cancellationToken = default);
}
