using TaskFlow.Dominio.Classe;

namespace TaskFlow.Aplicacao.DTOs
{
    public class TarefaDto
    {
        public TarefaDto(int id,
                         string titulo,
                         string descricao,
                         string disciplina,
                         EnumPrioridadeTarefa prioridade,
                         DateTime dataEntrega,
                         EnumStatusTarefa status,
                         DateTime criadaEm,
                         DateTime? atualizadaEm)
        {
            Id = id;
            Titulo = titulo;
            Descricao = descricao;
            Disciplina = disciplina;
            Prioridade = prioridade;
            DataEntrega = dataEntrega;
            Status = status;
            CriadaEm = criadaEm;
            AtualizadaEm = atualizadaEm;
        }

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Disciplina { get; set; }
        public EnumPrioridadeTarefa Prioridade { get; set; }
        public DateTime DataEntrega { get; set; }
        public EnumStatusTarefa Status { get; set; }
        public DateTime CriadaEm { get; set; }
        public DateTime? AtualizadaEm { get; set; }
    }
}
