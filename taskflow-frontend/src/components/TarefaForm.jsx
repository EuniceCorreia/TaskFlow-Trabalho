import { useState, useEffect } from 'react';

const PRIORIDADES = ['Baixa', 'Media', 'Alta'];

const vazio = {
  titulo: '',
  descricao: '',
  disciplina: '',
  prioridade: 'Media',
  dataEntrega: '',
};

export default function TarefaForm({ tarefaEditando, onSalvar, onCancelar }) {
  const [form, setForm] = useState(vazio);

  useEffect(() => {
    if (tarefaEditando) {
      setForm({
        titulo: tarefaEditando.titulo,
        descricao: tarefaEditando.descricao,
        disciplina: tarefaEditando.disciplina,
        prioridade: tarefaEditando.prioridade,
        dataEntrega: tarefaEditando.dataEntrega.slice(0, 10),
      });
    } else {
      setForm(vazio);
    }
  }, [tarefaEditando]);

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  function handleSubmit(e) {
    e.preventDefault();
    onSalvar({
      ...form,
      dataEntrega: new Date(form.dataEntrega + 'T00:00:00').toISOString(),
    });
  }

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h2 className="modal-titulo">
          {tarefaEditando ? '✏️ Editar Tarefa' : '📝 Nova Tarefa'}
        </h2>
        <form onSubmit={handleSubmit} className="form">
          <label>Título *</label>
          <input
            name="titulo"
            value={form.titulo}
            onChange={handleChange}
            placeholder="Ex: Estudar para a prova de Cálculo"
            required
          />

          <label>Disciplina *</label>
          <input
            name="disciplina"
            value={form.disciplina}
            onChange={handleChange}
            placeholder="Ex: Engenharia de Software"
            required
          />

          <label>Descrição</label>
          <textarea
            name="descricao"
            value={form.descricao}
            onChange={handleChange}
            placeholder="Detalhes da tarefa..."
            rows={3}
          />

          <label>Prioridade</label>
          <select name="prioridade" value={form.prioridade} onChange={handleChange}>
            {PRIORIDADES.map(p => (
              <option key={p} value={p}>{p === 'Media' ? 'Média' : p}</option>
            ))}
          </select>

          <label>Data de Entrega *</label>
          <input
            type="date"
            name="dataEntrega"
            value={form.dataEntrega}
            onChange={handleChange}
            required
          />

          <div className="form-botoes">
            <button type="button" className="btn-cancelar" onClick={onCancelar}>
              Cancelar
            </button>
            <button type="submit" className="btn-salvar">
              {tarefaEditando ? 'Salvar Alterações' : 'Criar Tarefa'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
