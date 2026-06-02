const PRIORIDADE_COR = {
  Baixa: '#00b894',
  Media: '#f39c12',
  Alta: '#e74c3c',
};

const PRIORIDADE_EMOJI = {
  Baixa: '🟢',
  Media: '🟡',
  Alta: '🔴',
};

const CORES_STICKNOTE = [
  { nome: 'Amarelo', valor: '#fff7a8' },
  { nome: 'Verde', valor: '#d9f8dc' },
  { nome: 'Azul', valor: '#dff7ff' },
  { nome: 'Rosa', valor: '#ffd8d8' },
  { nome: 'Lilas', valor: '#eadcff' },
];

export default function TarefaCard({
  tarefa,
  posicao,
  corSticknote,
  arrastando,
  onPointerDown,
  onCorChange,
  onConcluir,
  onReabrir,
  onEditar,
  onExcluir,
}) {
  const cor = PRIORIDADE_COR[tarefa.prioridade] || '#6c5ce7';
  const emoji = PRIORIDADE_EMOJI[tarefa.prioridade] || '⚪';
  const hoje = new Date();
  hoje.setHours(0, 0, 0, 0);
  const dataEntrega = new Date(tarefa.dataEntrega);
  dataEntrega.setHours(0, 0, 0, 0);
  const venceHoje = dataEntrega.getTime() === hoje.getTime();
  const dataPassou = dataEntrega < hoje;
  const vencida = !tarefa.concluida && dataPassou;

  return (
    <div
      className={`card ${tarefa.concluida ? 'card-concluida' : ''} ${vencida ? 'card-vencida' : ''} ${arrastando ? 'card-arrastando' : ''}`}
      data-tarefa-id={tarefa.id}
      style={{
        backgroundColor: corSticknote || undefined,
        borderColor: corSticknote || undefined,
        borderLeft: `5px solid ${cor}`,
        left: `${posicao?.x || 0}px`,
        top: `${posicao?.y || 0}px`,
      }}
      onPointerDown={(e) => onPointerDown(tarefa.id, e)}
    >

      <div className="card-header">
        <div>
          <span className="card-disciplina">📚 {tarefa.disciplina}</span>
          <h3 className={`card-titulo ${tarefa.concluida ? 'riscado' : ''}`}>{tarefa.titulo}</h3>
        </div>
        <span className="card-prioridade" style={{ color: cor }}>
          {emoji} {tarefa.prioridade === 'Media' ? 'Média' : tarefa.prioridade}
        </span>
      </div>

      {tarefa.descricao && (
        <p className="card-descricao">{tarefa.descricao}</p>
      )}

      <div className="card-paleta" aria-label="Cores do sticky note">
        {CORES_STICKNOTE.map(corOpcao => (
          <button
            key={corOpcao.valor}
            type="button"
            className={`btn-cor ${corSticknote === corOpcao.valor ? 'btn-cor-ativa' : ''}`}
            style={{ backgroundColor: corOpcao.valor }}
            title={corOpcao.nome}
            aria-label={`Mudar cor para ${corOpcao.nome}`}
            onClick={() => onCorChange(tarefa.id, corOpcao.valor)}
          />
        ))}
      </div>

      <div
        className="card-footer"
        style={{
          position: 'absolute',
          left: 0,
          right: 0,
          bottom: '0.95rem',
          width: '100%',
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          gap: '0.5rem',
          padding: '0 0.75rem',
          textAlign: 'center',
          transform: 'none',
        }}
      >
        <span
          className={`card-data ${(dataPassou || venceHoje) ? 'data-com-aviso' : ''} ${dataPassou ? 'data-vencida' : ''} ${venceHoje ? 'data-hoje' : ''}`}
          data-aviso={dataPassou ? 'A data desta tarefa ja passou.' : venceHoje ? 'Esta tarefa vence hoje.' : undefined}
        >
          {dataPassou ? '⚠️' : '📅'} {new Date(tarefa.dataEntrega).toLocaleDateString('pt-BR')}
        </span>

        <div
          className="card-acoes"
          style={{
            display: 'flex',
            flexWrap: 'wrap',
            justifyContent: 'center',
            alignItems: 'center',
            gap: '0.4rem',
            width: '100%',
            margin: '0 auto',
          }}
        >
          {tarefa.concluida ? (
            <button className="btn-acao btn-reabrir" onClick={() => onReabrir(tarefa.id)} title="Reabrir">
              🔄 Reabrir
            </button>
          ) : (
            <button className="btn-acao btn-concluir" onClick={() => onConcluir(tarefa.id)} title="Concluir">
              ✅ Concluir
            </button>
          )}
          <button className="btn-acao btn-editar" onClick={() => onEditar(tarefa)} title="Editar">
            ✏️
          </button>
          <button className="btn-acao btn-excluir" onClick={() => onExcluir(tarefa.id)} title="Excluir">
            🗑️
          </button>
        </div>
      </div>
    </div>
  );
}
