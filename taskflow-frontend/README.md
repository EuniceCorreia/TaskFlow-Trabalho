# TaskFlow

TaskFlow é um protótipo inicial funcional de um sistema para gerenciamento de tarefas acadêmicas. O sistema permite que estudantes cadastrem, organizem, acompanhem e concluam atividades de disciplinas, como trabalhos, provas, projetos e entregas.

Este repositório contém o frontend da aplicação, desenvolvido com React e Vite. A aplicação consome uma API backend em `/api/tarefas`, configurada no Vite para encaminhar as requisições para `http://localhost:5273`.

## Projeto Final 01: Protótipo Inicial

Este projeto foi desenvolvido para a primeira entrega do Projeto Final da disciplina. O foco desta etapa é criar um MVP funcional, com escopo limitado, interface utilizável e fluxo básico funcionando de ponta a ponta.

O objetivo não é apresentar uma arquitetura definitiva ou aplicar padrões de projeto de forma antecipada. A proposta é validar as funcionalidades principais e identificar pontos que podem precisar de melhoria nas próximas etapas.

## Integrantes

Preencher com os nomes dos integrantes do grupo:

- Eunice Correia
- Maria Laura 
- Vitoria Viana

## Problema e Domínio

Estudantes normalmente precisam acompanhar várias tarefas acadêmicas ao mesmo tempo, cada uma com disciplina, prazo, prioridade e status diferente. Sem uma organização visual simples, é fácil perder prazos ou esquecer atividades importantes.

O domínio do TaskFlow é o gerenciamento de tarefas acadêmicas. O sistema representa tarefas como cartões do tipo sticky note, permitindo que o usuário visualize rapidamente o que está pendente, concluído ou vencido.

## Funcionalidades Implementadas

- Cadastro de novas tarefas acadêmicas.
- Listagem das tarefas cadastradas.
- Edição de tarefas existentes.
- Exclusão de tarefas.
- Marcação de tarefa como concluída.
- Reabertura de tarefa concluída.
- Filtros por status: todas, pendentes e concluídas.
- Contadores de total, pendentes e concluídas.
- Definição de prioridade: baixa, média e alta.
- Exibição da disciplina, título, descrição, prioridade e data de entrega.
- Indicação visual de tarefa vencida.
- Aviso ao passar o mouse sobre a data:
  - data passada: informa que a data já passou;
  - data atual: informa que a tarefa vence hoje.
- Cards em formato de sticky note.
- Movimentação livre dos sticky notes pela tela.
- Salvamento local da posição dos sticky notes usando `localStorage`.
- Escolha individual de cor para cada sticky note.
- Salvamento local da cor escolhida usando `localStorage`.
- Posicionamento automático de novas tarefas em espaço livre, evitando sobreposição inicial.

## Tecnologias Utilizadas

- React
- Vite
- JavaScript
- CSS
- Fetch API
- LocalStorage
- Backend/API REST em `http://localhost:5273`

Dependências principais do frontend:

- `react`
- `react-dom`
- `@vitejs/plugin-react`
- `vite`

## Como Executar

### Pré-requisitos

- Node.js instalado.
- npm instalado.
- Backend da aplicação rodando em `http://localhost:5273`.

### Passos

1. Instalar as dependências:

```bash
npm install
```

2. Iniciar o frontend:

```bash
npm run dev
```

3. Acessar no navegador:

```text
http://localhost:5173
```

### Build de Produção

```bash
npm run build
```

### Observação Sobre a API

O frontend faz requisições para:

```text
/api/tarefas
```

No arquivo `vite.config.js`, essas requisições são redirecionadas para:

```text
http://localhost:5273
```

Portanto, para o funcionamento completo, a API backend deve estar disponível nessa porta.

## Endpoints Consumidos Pelo Frontend

O frontend espera os seguintes endpoints:

- `GET /api/tarefas`
- `GET /api/tarefas?status=Pendente`
- `GET /api/tarefas?status=Concluida`
- `POST /api/tarefas`
- `PUT /api/tarefas/:id`
- `DELETE /api/tarefas/:id`
- `PATCH /api/tarefas/:id/concluir`
- `PATCH /api/tarefas/:id/reabrir`

## O Que o MVP Entrega

O TaskFlow entrega um MVP adequado para a primeira etapa porque permite demonstrar o ciclo principal do sistema:

1. Criar uma tarefa acadêmica.
2. Visualizar a tarefa em formato de sticky note.
3. Alterar dados da tarefa.
4. Filtrar tarefas por status.
5. Marcar como concluída ou reabrir.
6. Excluir uma tarefa.
7. Organizar visualmente os cards na tela.
8. Personalizar a cor dos cards.

Esse fluxo cobre cadastro, listagem, edição, remoção, atualização de status e organização visual, que são funcionalidades centrais do domínio.

## Dificuldades Encontradas

- Definir um escopo pequeno o suficiente para ser viável como MVP.
- Manter a organização visual dos sticky notes sem criar sobreposição ao adicionar novos cards.
- Implementar movimentação livre dos cards na tela usando eventos de ponteiro.
- Guardar preferências visuais, como posição e cor dos cards, sem depender do backend.
- Diferenciar corretamente tarefas vencidas de tarefas que vencem no dia atual.
- Centralizar e ajustar elementos visuais em cards pequenos sem comprometer a legibilidade.

## Pontos Difíceis de Manter ou Expandir

- A lógica de posicionamento dos sticky notes está no frontend e pode ficar mais complexa se houver muitos cards.
- As posições e cores são salvas no `localStorage`, então ficam restritas ao navegador do usuário e não são compartilhadas entre dispositivos.
- O componente `TarefaCard` concentra bastante comportamento visual, como cores, tooltip, ações e posição.
- A interface ainda não possui autenticação, então não separa tarefas por usuário.
- A aplicação depende de uma API externa em execução; se o backend estiver fora do ar, o frontend não consegue carregar ou salvar tarefas.
- A validação dos dados ainda é simples e pode ser expandida.

## Possíveis Evoluções Futuras

- Autenticação de usuários.
- Separação de tarefas por usuário ou turma.
- Calendário de entregas.
- Notificações de prazos próximos.
- Persistência das posições e cores dos sticky notes no backend.
- Melhorias de responsividade para telas pequenas.
- Organização por quadros, disciplinas ou categorias.
- Testes automatizados.
- Refatoração para separar melhor regras de negócio, estado visual e componentes de interface.

## Uso de IA Generativa

Ferramentas de IA generativa utilizadas:

- Codex/OpenAI ChatGPT para apoio na implementação, ajustes visuais, organização do código e elaboração deste README.

- Claude/Code Anthropic Para Criação base do Código.


## Conclusão

O TaskFlow atende à proposta do Projeto Final 01 como protótipo inicial funcional. O sistema possui domínio claro, funcionalidades centrais implementadas, interface demonstrável e possibilidade de expansão futura. Também apresenta pontos reais de melhoria arquitetural, o que está de acordo com a proposta desta primeira etapa do trabalho.
