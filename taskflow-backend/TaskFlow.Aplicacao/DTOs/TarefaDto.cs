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
                         bool concluida,
                         DateTime criadaEm,
                         DateTime? atualizadaEm)
        {
            Id = id;
            Titulo = titulo;
            Descricao = descricao;
            Disciplina = disciplina;
            Prioridade = prioridade;
            DataEntrega = dataEntrega;
            Concluida = concluida;
            CriadaEm = criadaEm;
            AtualizadaEm = atualizadaEm;
        }

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Disciplina { get; set; }
        public EnumPrioridadeTarefa Prioridade { get; set; }
        public DateTime DataEntrega { get; set; }
        public bool Concluida { get; set; }
        public DateTime CriadaEm { get; set; }
        public DateTime? AtualizadaEm { get; set; }
    }
}
