const BASE = '/api/tarefas';

export async function listar(status, ordenacao) {
  const params = new URLSearchParams();
  if (status) params.append('status', status);
  if (ordenacao) params.append('ordenacao', ordenacao);
  const qs = params.toString();
  const res = await fetch(qs ? `${BASE}?${qs}` : BASE);
  if (!res.ok) throw new Error('Erro ao buscar tarefas');
  return res.json();
}

export async function buscarNotificacoes() {
  try {
    const res = await fetch('/api/notificacoes');
    if (!res.ok) return [];
    return res.json();
  } catch {
    return [];
  }
}

export async function criar(dto) {
  const res = await fetch(BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error('Erro ao criar tarefa');
  return res.json();
}

export async function atualizar(id, dto) {
  const res = await fetch(`${BASE}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error('Erro ao atualizar tarefa');
  return res.json();
}

export async function excluir(id) {
  const res = await fetch(`${BASE}/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error('Erro ao excluir tarefa');
}

export async function concluir(id) {
  const res = await fetch(`${BASE}/${id}/concluir`, { method: 'PATCH' });
  if (!res.ok) throw new Error('Erro ao concluir tarefa');
  return res.json();
}

export async function reabrir(id) {
  const res = await fetch(`${BASE}/${id}/reabrir`, { method: 'PATCH' });
  if (!res.ok) throw new Error('Erro ao reabrir tarefa');
  return res.json();
}

export async function pausar(id) {
  const res = await fetch(`${BASE}/${id}/pausar`, { method: 'PATCH' });
  if (!res.ok) throw new Error('Erro ao pausar tarefa');
  return res.json();
}

export async function retomar(id) {
  const res = await fetch(`${BASE}/${id}/retomar`, { method: 'PATCH' });
  if (!res.ok) throw new Error('Erro ao retomar tarefa');
  return res.json();
}
