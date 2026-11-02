import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import * as api from './tarefas.js'

function resposta(ok, dados) {
  return { ok, json: vi.fn().mockResolvedValue(dados) }
}

let fetchMock

beforeEach(() => {
  fetchMock = vi.fn()
  vi.stubGlobal('fetch', fetchMock)
})

afterEach(() => {
  vi.unstubAllGlobals()
})

describe('listar', () => {
  it('sem filtros chama /api/tarefas', async () => {
    fetchMock.mockResolvedValue(resposta(true, [{ id: 1 }]))
    await expect(api.listar()).resolves.toEqual([{ id: 1 }])
    expect(fetchMock).toHaveBeenCalledWith('/api/tarefas')
  })

  it('com filtros monta a query string', async () => {
    fetchMock.mockResolvedValue(resposta(true, []))
    await api.listar('Pendente', 'prazo')
    expect(fetchMock).toHaveBeenCalledWith('/api/tarefas?status=Pendente&ordenacao=prazo')
  })

  it('com só um filtro inclui apenas ele', async () => {
    fetchMock.mockResolvedValue(resposta(true, []))
    await api.listar(undefined, 'prioridade')
    expect(fetchMock).toHaveBeenCalledWith('/api/tarefas?ordenacao=prioridade')
  })

  it('lança erro quando a API falha', async () => {
    fetchMock.mockResolvedValue(resposta(false))
    await expect(api.listar()).rejects.toThrow('Erro ao buscar tarefas')
  })
})

describe('buscarNotificacoes', () => {
  it('retorna as notificações', async () => {
    fetchMock.mockResolvedValue(resposta(true, ['nova tarefa']))
    await expect(api.buscarNotificacoes()).resolves.toEqual(['nova tarefa'])
    expect(fetchMock).toHaveBeenCalledWith('/api/notificacoes')
  })

  it('retorna lista vazia quando a API responde com erro', async () => {
    fetchMock.mockResolvedValue(resposta(false))
    await expect(api.buscarNotificacoes()).resolves.toEqual([])
  })

  it('retorna lista vazia quando a API está fora do ar', async () => {
    fetchMock.mockRejectedValue(new Error('rede'))
    await expect(api.buscarNotificacoes()).resolves.toEqual([])
  })
})

describe('criar', () => {
  const dto = { titulo: 'Estudar DevSecOps' }

  it('envia POST com o corpo em JSON', async () => {
    fetchMock.mockResolvedValue(resposta(true, { id: 1, ...dto }))
    await expect(api.criar(dto)).resolves.toEqual({ id: 1, ...dto })
    expect(fetchMock).toHaveBeenCalledWith('/api/tarefas', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(dto),
    })
  })

  it('lança erro quando a API falha', async () => {
    fetchMock.mockResolvedValue(resposta(false))
    await expect(api.criar(dto)).rejects.toThrow('Erro ao criar tarefa')
  })
})

describe('atualizar', () => {
  const dto = { titulo: 'Atualizada' }

  it('envia PUT para a tarefa', async () => {
    fetchMock.mockResolvedValue(resposta(true, { id: 5, ...dto }))
    await expect(api.atualizar(5, dto)).resolves.toEqual({ id: 5, ...dto })
    expect(fetchMock).toHaveBeenCalledWith('/api/tarefas/5', {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(dto),
    })
  })

  it('lança erro quando a API falha', async () => {
    fetchMock.mockResolvedValue(resposta(false))
    await expect(api.atualizar(5, dto)).rejects.toThrow('Erro ao atualizar tarefa')
  })
})

describe('excluir', () => {
  it('envia DELETE para a tarefa', async () => {
    fetchMock.mockResolvedValue(resposta(true))
    await expect(api.excluir(3)).resolves.toBeUndefined()
    expect(fetchMock).toHaveBeenCalledWith('/api/tarefas/3', { method: 'DELETE' })
  })

  it('lança erro quando a API falha', async () => {
    fetchMock.mockResolvedValue(resposta(false))
    await expect(api.excluir(3)).rejects.toThrow('Erro ao excluir tarefa')
  })
})

describe.each([
  ['concluir', api.concluir, 'Erro ao concluir tarefa'],
  ['reabrir', api.reabrir, 'Erro ao reabrir tarefa'],
  ['pausar', api.pausar, 'Erro ao pausar tarefa'],
  ['retomar', api.retomar, 'Erro ao retomar tarefa'],
])('%s', (acao, fn, mensagem) => {
  it('envia PATCH para a ação', async () => {
    fetchMock.mockResolvedValue(resposta(true, { id: 7 }))
    await expect(fn(7)).resolves.toEqual({ id: 7 })
    expect(fetchMock).toHaveBeenCalledWith(`/api/tarefas/7/${acao}`, { method: 'PATCH' })
  })

  it('lança erro quando a API falha', async () => {
    fetchMock.mockResolvedValue(resposta(false))
    await expect(fn(7)).rejects.toThrow(mensagem)
  })
})
