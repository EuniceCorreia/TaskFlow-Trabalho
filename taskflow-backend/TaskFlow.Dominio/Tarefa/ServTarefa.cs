using TaskFlow.Dominio.Classe;
using TaskFlow.Dominio.Excecoes;
using TaskFlow.Dominio.IRep;
using TaskFlow.Dominio.IServ;

namespace TaskFlow.Dominio.Serv
{
    public class TarefaServ : ITarefaServ
    {
        private readonly ITarefaRep _repTarefa;

        public TarefaServ(ITarefaRep repTarefa)
        {
            _repTarefa = repTarefa;
        }

        public async Task<Tarefa> CriarAsync(string titulo, string descricao, string disciplina, EnumPrioridadeTarefa prioridade, DateTime dataEntrega, CancellationToken cancellationToken = default)
        {
            ValidarDados(titulo, descricao, disciplina, prioridade, dataEntrega);

            if (await _repTarefa.ExisteTituloNaDisciplinaAsync(titulo, disciplina, cancellationToken: cancellationToken))
            {
                throw new RegraDeNegocioException("Ja existe uma tarefa com este titulo para a disciplina informada.");
            }

            return new Tarefa(titulo, descricao, disciplina, prioridade, dataEntrega);
        }

        public async Task AtualizarAsync(Tarefa tarefa, string titulo, string descricao, string disciplina, EnumPrioridadeTarefa prioridade, DateTime dataEntrega, CancellationToken cancellationToken = default)
        {
            ValidarDados(titulo, descricao, disciplina, prioridade, dataEntrega);

            if (await _repTarefa.ExisteTituloNaDisciplinaAsync(titulo, disciplina, tarefa.Id, cancellationToken))
            {
                throw new RegraDeNegocioException("Ja existe outra tarefa com este titulo para a disciplina informada.");
            }

            tarefa.Atualizar(titulo, descricao, disciplina, prioridade, dataEntrega);
        }

        public void ValidarDados(string titulo, string descricao, string disciplina, EnumPrioridadeTarefa prioridade, DateTime dataEntrega)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                throw new RegraDeNegocioException("O titulo da tarefa e obrigatorio.");
            }

            if (titulo.Trim().Length > 120)
            {
                throw new RegraDeNegocioException("O titulo deve ter no maximo 120 caracteres.");
            }

            if (descricao.Trim().Length > 1000)
            {
                throw new RegraDeNegocioException("A descricao deve ter no maximo 1000 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(disciplina))
            {
                throw new RegraDeNegocioException("A disciplina da tarefa e obrigatoria.");
            }

            if (disciplina.Trim().Length > 100)
            {
                throw new RegraDeNegocioException("A disciplina deve ter no maximo 100 caracteres.");
            }

            if (!Enum.IsDefined(prioridade))
            {
                throw new RegraDeNegocioException("A prioridade informada e invalida.");
            }

            if (dataEntrega == default)
            {
                throw new RegraDeNegocioException("A data de entrega e obrigatoria.");
            }
        }

        public void Pausar(Tarefa tarefa) { tarefa.Pausar(); }

        public void Retomar(Tarefa tarefa) { tarefa.Retomar(); }

        public void Concluir(Tarefa tarefa) { tarefa.Concluir(); }

        public void Reabrir(Tarefa tarefa) { tarefa.Reabrir(); }
    }
}
