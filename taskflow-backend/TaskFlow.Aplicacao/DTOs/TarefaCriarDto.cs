using TaskFlow.Dominio.Classe;

namespace TaskFlow.Aplicacao.DTOs
{
    public class TarefaCriarDto
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Disciplina { get; set; }
        public EnumPrioridadeTarefa Prioridade { get; set; }
        public DateTime DataEntrega { get; set; }
    }

}
