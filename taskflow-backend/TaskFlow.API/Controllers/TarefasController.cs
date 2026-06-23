using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Convencoes;
using TaskFlow.API.Facade;
using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/tarefas")]
[ApiConventionType(typeof(TarefasControllerStatusCodes))]
public sealed class TarefasController : ControllerBase
{
    private readonly ITarefaFacade _facade;

    public TarefasController(ITarefaFacade facade)
    {
        _facade = facade;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TarefaDto>>> Listar(
        [FromQuery] EnumStatusTarefa? status,
        [FromQuery] EnumOrdenacaoTarefa? ordenacao,
        CancellationToken cancellationToken)
    {
        var tarefas = await _facade.ListarAsync(status, ordenacao, cancellationToken);
        return Ok(tarefas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TarefaDto>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var tarefa = await _facade.ObterPorIdAsync(id, cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpPost]
    public async Task<ActionResult<TarefaDto>> Criar(
        [FromBody] TarefaCriarDto dto,
        CancellationToken cancellationToken)
    {
        var tarefa = await _facade.CriarAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TarefaDto>> Atualizar(
        int id,
        [FromBody] TarefaAtualizarDto dto,
        CancellationToken cancellationToken)
    {
        var tarefa = await _facade.AtualizarAsync(id, dto, cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        var excluida = await _facade.ExcluirAsync(id, cancellationToken);
        return excluida ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/pausar")]
    public async Task<ActionResult<TarefaDto>> Pausar(int id, CancellationToken cancellationToken)
    {
        var tarefa = await _facade.TransicionarAsync(id, "pausar", cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpPatch("{id:int}/retomar")]
    public async Task<ActionResult<TarefaDto>> Retomar(int id, CancellationToken cancellationToken)
    {
        var tarefa = await _facade.TransicionarAsync(id, "retomar", cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpPatch("{id:int}/concluir")]
    public async Task<ActionResult<TarefaDto>> Concluir(int id, CancellationToken cancellationToken)
    {
        var tarefa = await _facade.TransicionarAsync(id, "concluir", cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpPatch("{id:int}/reabrir")]
    public async Task<ActionResult<TarefaDto>> Reabrir(int id, CancellationToken cancellationToken)
    {
        var tarefa = await _facade.TransicionarAsync(id, "reabrir", cancellationToken);
        return tarefa is null ? NotFound() : Ok(tarefa);
    }
}
