import { describe, it, expect, vi, afterEach } from 'vitest'
import { render, screen, fireEvent, cleanup } from '@testing-library/react'
import TarefaForm from './TarefaForm'

afterEach(cleanup)

const tarefa = {
  titulo: 'Ferias',
  descricao: 'Sem comentarios.',
  disciplina: 'Engenharia de Software',
  prioridade: 'Baixa',
  dataEntrega: '2026-06-21T00:00:00',
}

function campoTitulo() {
  return screen.getByPlaceholderText('Ex: Estudar para a prova de Cálculo')
}

describe('TarefaForm', () => {
  it('abre vazio para uma nova tarefa', () => {
    render(<TarefaForm tarefaEditando={null} onSalvar={vi.fn()} onCancelar={vi.fn()} />)
    expect(screen.getByText(/Nova Tarefa/)).toBeTruthy()
    expect(campoTitulo().value).toBe('')
  })

  it('abre preenchido ao editar uma tarefa', () => {
    render(<TarefaForm tarefaEditando={tarefa} onSalvar={vi.fn()} onCancelar={vi.fn()} />)
    expect(screen.getByText(/Editar Tarefa/)).toBeTruthy()
    expect(campoTitulo().value).toBe('Ferias')
    expect(screen.getByDisplayValue('2026-06-21')).toBeTruthy()
  })

  it('envia os dados alterados ao salvar', () => {
    const onSalvar = vi.fn()
    const { container } = render(
      <TarefaForm tarefaEditando={tarefa} onSalvar={onSalvar} onCancelar={vi.fn()} />
    )
    fireEvent.change(campoTitulo(), { target: { value: 'Ferias 2' } })
    fireEvent.submit(container.querySelector('form'))

    expect(onSalvar).toHaveBeenCalledWith({
      titulo: 'Ferias 2',
      descricao: 'Sem comentarios.',
      disciplina: 'Engenharia de Software',
      prioridade: 'Baixa',
      dataEntrega: new Date('2026-06-21T00:00:00').toISOString(),
    })
  })

  it('chama onCancelar ao clicar em Cancelar', () => {
    const onCancelar = vi.fn()
    render(<TarefaForm tarefaEditando={null} onSalvar={vi.fn()} onCancelar={onCancelar} />)
    fireEvent.click(screen.getByText('Cancelar'))
    expect(onCancelar).toHaveBeenCalled()
  })
})