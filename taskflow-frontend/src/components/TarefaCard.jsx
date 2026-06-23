import { useState, useRef, useEffect } from 'react';

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
  { nome: 'Amarelo', valor: '#fefec8' },
  { nome: 'Verde', valor: '#b4f5b4' },
  { nome: 'Azul', valor: '#b9e1f8' },
  { nome: 'Rosa', valor: '#ffaad7' },
  { nome: 'Lilas', valor: '#f3dcff' },
];

const STATUS_MAP = { 1: 'Pendente', 2: 'Pausada', 3: 'Concluida' };

export default function TarefaCard({
  tarefa,
  posicao,
  corSticknote,
  arrastando,
  onPointerDown,
  onCorChange,
  onPausar,
  onRetomar,
  onConcluir,
  onReabrir,
  onEditar,
  onExcluir,
}) {
  const [paletaAberta, setPaletaAberta] = useState(false);
  const paletaRef = useRef(null);

  useEffect(() => {
    if (!paletaAberta) return;
    const fechar = (e) => {
      if (paletaRef.current && !paletaRef.current.contains(e.target)) {
        setPaletaAberta(false);
      }
    };
    document.addEventListener('pointerdown', fechar);
    return () => document.removeEventListener('pointerdown', fechar);
  }, [paletaAberta]);

  const cor = PRIORIDADE_COR[tarefa.prioridade] || '#6c5ce7';
  const emoji = PRIORIDADE_EMOJI[tarefa.prioridade] || '⚪';
  const hoje = new Date();
  hoje.setHours(0, 0, 0, 0);
  const dataEntrega = new Date(tarefa.dataEntrega);
  dataEntrega.setHours(0, 0, 0, 0);
  const venceHoje = dataEntrega.getTime() === hoje.getTime();
  const dataPassou = dataEntrega < hoje;
  const status = STATUS_MAP[tarefa.status] ?? tarefa.status;
  const concluida = status === 'Concluida';
  const pausada = status === 'Pausada';
  const vencida = (status === 'Pendente' || status === 'Pausada') && dataPassou;

  return (
    <div
      className={`card ${concluida ? 'card-concluida' : ''} ${pausada ? 'card-pausada' : ''} ${vencida ? 'card-vencida' : ''} ${arrastando ? 'card-arrastando' : ''}`}
      data-tarefa-id={tarefa.id}
      style={{
        backgroundColor: corSticknote || undefined,
        borderColor: corSticknote || undefined,
        left: `${posicao?.x || 0}px`,
        top: `${posicao?.y || 0}px`,
      }}
      onPointerDown={(e) => onPointerDown(tarefa.id, e)}
    >

      <div className="card-header">
        <span className="card-disciplina">📚 {tarefa.disciplina}</span>
        <h3 className={`card-titulo ${concluida ? 'riscado' : ''}`}>{tarefa.titulo}</h3>
      </div>

      <span className="card-prioridade" style={{ color: cor }}>
        {emoji} {tarefa.prioridade === 'Media' ? 'Média' : tarefa.prioridade}
        {vencida && <span className="badge-vencida">Vencida</span>}
      </span>

      {tarefa.descricao && (
        <p className="card-descricao">{tarefa.descricao}</p>
      )}

      <div className="card-paleta" ref={paletaRef} aria-label="Cores do sticky note">
        <button
          type="button"
          className="btn-cor btn-cor-trigger"
          style={{ backgroundColor: corSticknote || '#e0e0e0' }}
          title="Mudar cor"
          aria-label="Abrir paleta de cores"
          onPointerDown={(e) => e.stopPropagation()}
          onClick={(e) => { e.stopPropagation(); setPaletaAberta(a => !a); }}
        />
        {paletaAberta && (
          <div className="card-paleta-opcoes">
            {CORES_STICKNOTE.map(corOpcao => (
              <button
                key={corOpcao.valor}
                type="button"
                className={`btn-cor ${corSticknote === corOpcao.valor ? 'btn-cor-ativa' : ''}`}
                style={{ backgroundColor: corOpcao.valor }}
                title={corOpcao.nome}
                aria-label={`Mudar cor para ${corOpcao.nome}`}
                onPointerDown={(e) => e.stopPropagation()}
                onClick={(e) => { e.stopPropagation(); onCorChange(tarefa.id, corOpcao.valor); setPaletaAberta(false); }}
              />
            ))}
          </div>
        )}
      </div>

      <div className="card-footer">
        <span
          className={`card-data ${(dataPassou || venceHoje) ? 'data-com-aviso' : ''} ${dataPassou ? 'data-vencida' : ''} ${venceHoje ? 'data-hoje' : ''}`}
          data-aviso={dataPassou ? 'A data desta tarefa ja passou.' : venceHoje ? 'Esta tarefa vence hoje.' : undefined}
        >
          {dataPassou ? '⚠️' : '📅'} {new Date(tarefa.dataEntrega).toLocaleDateString('pt-BR')}
        </span>

        <div className="card-acoes">
          {status === 'Pendente' && (
            <>
              <button className="btn-acao btn-pausar" onClick={() => onPausar(tarefa.id)} title="Pausar">
                ⏸ Pausar
              </button>
              <button className="btn-acao btn-concluir" onClick={() => onConcluir(tarefa.id)} title="Concluir">
                ✅ Concluir
              </button>
              <button className="btn-acao btn-editar" onClick={() => onEditar(tarefa)} title="Editar">
                ✏️
              </button>
              <button className="btn-acao btn-excluir" onClick={() => onExcluir(tarefa.id)} title="Excluir">
                🗑️
              </button>
            </>
          )}
          {status === 'Pausada' && (
            <>
              <button className="btn-acao btn-retomar" onClick={() => onRetomar(tarefa.id)} title="Retomar">
                ▶ Retomar
              </button>
              <button className="btn-acao btn-concluir" onClick={() => onConcluir(tarefa.id)} title="Concluir">
                ✅ Concluir
              </button>
              <button className="btn-acao btn-editar" onClick={() => onEditar(tarefa)} title="Editar">
                ✏️
              </button>
              <button className="btn-acao btn-excluir" onClick={() => onExcluir(tarefa.id)} title="Excluir">
                🗑️
              </button>
            </>
          )}
          {status === 'Concluida' && (
            <>
              <button className="btn-acao btn-reabrir" onClick={() => onReabrir(tarefa.id)} title="Reabrir">
                🔄 Reabrir
              </button>
              <button className="btn-acao btn-excluir" onClick={() => onExcluir(tarefa.id)} title="Excluir">
                🗑️
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
