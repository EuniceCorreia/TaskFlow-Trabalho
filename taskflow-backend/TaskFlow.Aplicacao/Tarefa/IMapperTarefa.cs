using TaskFlow.Aplicacao.DTOs;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.Aplicacao;

public interface IMapperTarefa
{
    TarefaDto Mapear(Tarefa tarefa);
}
