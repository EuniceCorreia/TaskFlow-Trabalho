import { describe, it, expect, vi, afterEach } from 'vitest'
import { render, screen, cleanup } from '@testing-library/react'
import App from './App'
import * as api from './api/tarefas'

vi.mock('./api/tarefas', () => ({
  listar: vi.fn(),
  buscarNotificacoes: vi.fn(),
  criar: vi.fn(),
  atualizar: vi.fn(),
  excluir: vi.fn(),
  concluir: vi.fn(),
  reabrir: vi.fn(),
  pausar: vi.fn(),
  retomar: vi.fn(),
}))

afterEach(() => {
  cleanup()
  vi.clearAllMocks()
})

describe('App', () => {
  it('mostra o quadro vazio quando a API não retorna tarefas', async () => {
    api.listar.mockResolvedValue([])
    api.buscarNotificacoes.mockResolvedValue([])

    render(<App />)

    expect(await screen.findByText('Nenhuma tarefa encontrada.')).toBeTruthy()
    expect(api.listar).toHaveBeenCalledWith(undefined, undefined)
  })

  it('mostra mensagem de erro quando a API está fora do ar', async () => {
    api.listar.mockRejectedValue(new Error('rede'))
    api.buscarNotificacoes.mockResolvedValue([])

    render(<App />)

    expect(
      await screen.findByText(/Não foi possível conectar ao servidor/)
    ).toBeTruthy()
  })
})