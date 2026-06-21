using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.Aplicacao;

public sealed class MapperTarefa : IMapperTarefa
{
    public TarefaDto Mapear(Tarefa tarefa)
    {
        return new TarefaDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Disciplina,
            tarefa.Prioridade,
            tarefa.DataEntrega,
            tarefa.Status,
            tarefa.CriadaEm,
            tarefa.AtualizadaEm);
    }
}
