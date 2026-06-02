using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Convencoes;
using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Aplicacao.IAplic;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/tarefas")]
[ApiConventionType(typeof(TarefasControllerStatusCodes))]
public sealed class TarefasController : ControllerBase
{
    private readonly ITarefaAplic _tarefaAplic;

    public TarefasController(ITarefaAplic tarefaAplic)
    {
        _tarefaAplic = tarefaAplic;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TarefaDto>>> Listar(
        [FromQuery] EnumStatusTarefa? status,
        CancellationToken cancellationToken)
    {
        var tarefas = await _tarefaAplic.ListarAsync(status, cancellationToken);
        return Ok(tarefas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TarefaDto>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaAplic.ObterPorIdAsync(id, cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpPost]
    public async Task<ActionResult<TarefaDto>> Criar(
        [FromBody] TarefaCriarDto dto,
        CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaAplic.CriarAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TarefaDto>> Atualizar(
        int id,
        [FromBody] TarefaAtualizarDto dto,
        CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaAplic.AtualizarAsync(id, dto, cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var excluida = await _tarefaAplic.ExcluirAsync(id, cancellationToken);
        return excluida ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/concluir")]
    public async Task<ActionResult<TarefaDto>> Concluir(int id, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaAplic.MarcarComoConcluidaAsync(id, cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpPatch("{id:int}/reabrir")]
    public async Task<ActionResult<TarefaDto>> Reabrir(int id, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaAplic.MarcarComoPendenteAsync(id, cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }
}
