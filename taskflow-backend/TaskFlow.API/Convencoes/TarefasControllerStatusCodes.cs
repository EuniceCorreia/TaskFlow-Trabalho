using Microsoft.AspNetCore.Mvc;
using TaskFlow.Aplicacao.DTOs;

namespace TaskFlow.API.Convencoes;

public static class TarefasControllerStatusCodes
{
    [ProducesResponseType(typeof(IReadOnlyList<TarefaDto>), StatusCodes.Status200OK)]
    public static void Listar()
    {
    }

    [ProducesResponseType(typeof(TarefaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static void ObterPorId()
    {
    }

    [ProducesResponseType(typeof(TarefaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public static void Criar()
    {
    }

    [ProducesResponseType(typeof(TarefaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static void Atualizar()
    {
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static void Excluir()
    {
    }

    [ProducesResponseType(typeof(TarefaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static void Concluir()
    {
    }

    [ProducesResponseType(typeof(TarefaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static void Reabrir()
    {
    }
}
