const BASE = '/api/tarefas';

export async function listar(status) {
  const url = status ? `${BASE}?status=${status}` : BASE;
  const res = await fetch(url);
  if (!res.ok) throw new Error('Erro ao buscar tarefas');
  return res.json();
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
