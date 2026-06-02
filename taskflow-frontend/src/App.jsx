import { useState, useEffect, useCallback, useRef } from 'react';
import * as api from './api/tarefas';
import TarefaCard from './components/TarefaCard';
import TarefaForm from './components/TarefaForm';

const POSICOES_TAREFAS_KEY = 'taskflow-posicoes-tarefas';
const CORES_TAREFAS_KEY = 'taskflow-cores-tarefas';
const CARD_TAMANHO = 240;
const CARD_ESPACO = 18;

const FILTROS = [
  { label: '📋 Todas', value: '' },
  { label: '⏳ Pendentes', value: 'Pendente' },
  { label: '✅ Concluídas', value: 'Concluida' },
];

function lerPosicoesSalvas() {
  try {
    return JSON.parse(localStorage.getItem(POSICOES_TAREFAS_KEY)) || {};
  } catch {
    return {};
  }
}

function salvarPosicoesTarefas(posicoes) {
  localStorage.setItem(POSICOES_TAREFAS_KEY, JSON.stringify(posicoes));
}

function lerCoresSalvas() {
  try {
    return JSON.parse(localStorage.getItem(CORES_TAREFAS_KEY)) || {};
  } catch {
    return {};
  }
}

function salvarCoresTarefas(cores) {
  localStorage.setItem(CORES_TAREFAS_KEY, JSON.stringify(cores));
}

function calcularPosicaoInicial(index, larguraPainel) {
  const colunas = Math.max(1, Math.floor((larguraPainel + CARD_ESPACO) / (CARD_TAMANHO + CARD_ESPACO)));

  return {
    x: (index % colunas) * (CARD_TAMANHO + CARD_ESPACO),
    y: Math.floor(index / colunas) * (CARD_TAMANHO + CARD_ESPACO),
  };
}

function posicoesSobrepostas(a, b) {
  return (
    a.x < b.x + CARD_TAMANHO + CARD_ESPACO &&
    a.x + CARD_TAMANHO + CARD_ESPACO > b.x &&
    a.y < b.y + CARD_TAMANHO + CARD_ESPACO &&
    a.y + CARD_TAMANHO + CARD_ESPACO > b.y
  );
}

function calcularPosicaoLivre(posicoesAtuais, larguraPainel) {
  const colunas = Math.max(1, Math.floor((larguraPainel + CARD_ESPACO) / (CARD_TAMANHO + CARD_ESPACO)));
  const ocupadas = Object.values(posicoesAtuais).filter(Boolean);

  for (let tentativa = 0; tentativa < 500; tentativa += 1) {
    const candidata = calcularPosicaoInicial(tentativa, larguraPainel);
    const foraDoPainel = candidata.x + CARD_TAMANHO > larguraPainel && colunas > 1;

    if (!foraDoPainel && !ocupadas.some(posicao => posicoesSobrepostas(candidata, posicao))) {
      return candidata;
    }
  }

  return calcularPosicaoInicial(ocupadas.length, larguraPainel);
}

export default function App() {
  const [tarefas, setTarefas] = useState([]);
  const [filtro, setFiltro] = useState('');
  const [mostrarForm, setMostrarForm] = useState(false);
  const [tarefaEditando, setTarefaEditando] = useState(null);
  const [posicoes, setPosicoes] = useState(() => lerPosicoesSalvas());
  const [cores, setCores] = useState(() => lerCoresSalvas());
  const [tarefaArrastadaId, setTarefaArrastadaId] = useState(null);
  const [erro, setErro] = useState('');
  const [carregando, setCarregando] = useState(true);
  const listaRef = useRef(null);
  const tarefaArrastadaRef = useRef(null);
  const deslocamentoArrasteRef = useRef({ x: 0, y: 0 });

  const carregar = useCallback(async () => {
    try {
      setCarregando(true);
      const dados = await api.listar(filtro || undefined);
      setTarefas(dados);
      setErro('');
    } catch {
      setErro('Não foi possível conectar ao servidor. Verifique se a API está rodando.');
    } finally {
      setCarregando(false);
    }
  }, [filtro]);

  useEffect(() => { carregar(); }, [carregar]);

  useEffect(() => {
    if (!listaRef.current || tarefas.length === 0) return;

    const larguraPainel = listaRef.current.clientWidth || CARD_TAMANHO;

    setPosicoes(atuais => {
      const proximas = { ...atuais };
      let mudou = false;

      tarefas.forEach((tarefa) => {
        const id = String(tarefa.id);
        if (!proximas[id]) {
          proximas[id] = calcularPosicaoLivre(proximas, larguraPainel);
          mudou = true;
        }
      });

      if (mudou) salvarPosicoesTarefas(proximas);
      return proximas;
    });
  }, [tarefas]);

  useEffect(() => {
    if (!tarefaArrastadaId) return undefined;

    function handlePointerMove(e) {
      const idOrigem = tarefaArrastadaRef.current;
      const painel = listaRef.current;

      if (!idOrigem || !painel) return;

      const rect = painel.getBoundingClientRect();
      const deslocamento = deslocamentoArrasteRef.current;
      const x = Math.max(0, e.clientX - rect.left - deslocamento.x);
      const y = Math.max(0, e.clientY - rect.top - deslocamento.y);

      setPosicoes(atuais => ({
        ...atuais,
        [String(idOrigem)]: { x, y },
      }));
    }

    function handlePointerUp() {
      setPosicoes(atuais => {
        salvarPosicoesTarefas(atuais);
        return atuais;
      });
      tarefaArrastadaRef.current = null;
      setTarefaArrastadaId(null);
    }

    window.addEventListener('pointermove', handlePointerMove);
    window.addEventListener('pointerup', handlePointerUp, { once: true });

    return () => {
      window.removeEventListener('pointermove', handlePointerMove);
      window.removeEventListener('pointerup', handlePointerUp);
    };
  }, [tarefaArrastadaId]);

  async function handleSalvar(dto) {
    try {
      if (tarefaEditando) {
        await api.atualizar(tarefaEditando.id, dto);
      } else {
        await api.criar(dto);
      }
      fecharForm();
      carregar();
    } catch {
      setErro('Erro ao salvar tarefa.');
    }
  }

  async function handleConcluir(id) {
    try { await api.concluir(id); carregar(); } catch { setErro('Erro ao concluir.'); }
  }

  async function handleReabrir(id) {
    try { await api.reabrir(id); carregar(); } catch { setErro('Erro ao reabrir.'); }
  }

  async function handleExcluir(id) {
    if (!confirm('Tem certeza que deseja excluir esta tarefa?')) return;
    try { await api.excluir(id); carregar(); } catch { setErro('Erro ao excluir.'); }
  }

  function handleEditar(tarefa) {
    setTarefaEditando(tarefa);
    setMostrarForm(true);
  }

  function fecharForm() {
    setMostrarForm(false);
    setTarefaEditando(null);
  }

  function handlePointerDown(id, e) {
    if (e.target.closest('button') || e.target.closest('.card-paleta')) return;

    const rect = e.currentTarget.getBoundingClientRect();
    deslocamentoArrasteRef.current = {
      x: e.clientX - rect.left,
      y: e.clientY - rect.top,
    };
    tarefaArrastadaRef.current = id;
    setTarefaArrastadaId(id);
    e.preventDefault();
  }

  function handleCorChange(id, cor) {
    setCores(atuais => {
      const proximas = {
        ...atuais,
        [String(id)]: cor,
      };
      salvarCoresTarefas(proximas);
      return proximas;
    });
  }

  const pendentes = tarefas.filter(t => !t.concluida).length;
  const concluidas = tarefas.filter(t => t.concluida).length;
  const alturaPainel = Math.max(
    420,
    ...tarefas.map(tarefa => (posicoes[String(tarefa.id)]?.y || 0) + CARD_TAMANHO + CARD_ESPACO)
  );

  return (
    <div className="app">
      <header className="header">
        <div className="header-content">
          <div className="logo">
            <span className="logo-icon">🎓</span>
            <div>
              <h1>TaskFlow</h1>
              <p>Seu gerenciador de tarefas acadêmicas</p>
            </div>
          </div>
          <button className="btn-nova" onClick={() => setMostrarForm(true)}>
            + Nova Tarefa
          </button>
        </div>

        <div className="stats">
          <div className="stat">
            <span className="stat-numero">{tarefas.length}</span>
            <span className="stat-label">Total</span>
          </div>
          <div className="stat">
            <span className="stat-numero stat-numero-pendente">{pendentes}</span>
            <span className="stat-label">Pendentes</span>
          </div>
          <div className="stat">
            <span className="stat-numero stat-numero-concluida">{concluidas}</span>
            <span className="stat-label">Concluídas</span>
          </div>
        </div>
      </header>

      <main className="main">
        <div className="filtros">
          {FILTROS.map(f => (
            <button
              key={f.value}
              className={`btn-filtro ${filtro === f.value ? 'ativo' : ''}`}
              onClick={() => setFiltro(f.value)}
            >
              {f.label}
            </button>
          ))}
        </div>

        {erro && (
          <div className="erro">
            ⚠️ {erro}
            <button onClick={() => setErro('')}>✕</button>
          </div>
        )}

        {carregando ? (
          <div className="vazio">⏳ Carregando tarefas...</div>
        ) : tarefas.length === 0 ? (
          <div className="vazio">
            <span style={{ fontSize: '4rem' }}>📭</span>
            <p>Nenhuma tarefa encontrada.</p>
            <p style={{ color: '#aaa', fontSize: '0.9rem' }}>Que tal adicionar uma nova tarefa?</p>
          </div>
        ) : (
          <div className="lista" ref={listaRef} style={{ minHeight: `${alturaPainel}px` }}>
            {tarefas.map(t => (
              <TarefaCard
                key={t.id}
                tarefa={t}
                posicao={posicoes[String(t.id)]}
                corSticknote={cores[String(t.id)]}
                arrastando={String(tarefaArrastadaId) === String(t.id)}
                onPointerDown={handlePointerDown}
                onCorChange={handleCorChange}
                onConcluir={handleConcluir}
                onReabrir={handleReabrir}
                onEditar={handleEditar}
                onExcluir={handleExcluir}
              />
            ))}
          </div>
        )}
      </main>

      {mostrarForm && (
        <TarefaForm
          tarefaEditando={tarefaEditando}
          onSalvar={handleSalvar}
          onCancelar={fecharForm}
        />
      )}
    </div>
  );
}
