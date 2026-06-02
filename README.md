# TaskFlow

## Projeto Final 01: Prototipo Inicial

TaskFlow e um prototipo funcional de um sistema para gerenciamento de tarefas academicas. O projeto permite que estudantes cadastrem, organizem, editem, concluam e acompanhem tarefas relacionadas a disciplinas, prazos e prioridades.

Este repositorio contem as duas partes da aplicacao:

- `taskflow-backend`: API REST em .NET, com regras de negocio e persistencia em SQLite.
- `taskflow-frontend`: interface web em React/Vite, com cards em formato de sticky notes.

O objetivo desta primeira entrega e apresentar um MVP funcional de ponta a ponta, com escopo limitado, interface utilizavel, backend integrado e dados persistidos.

## Integrantes

- Eunice Correia
- Maria Laura
- Vitoria Viana

## Problema e Dominio

Estudantes precisam acompanhar varias tarefas academicas ao mesmo tempo, cada uma com disciplina, prazo, prioridade e status diferente. Sem uma organizacao visual simples, e facil perder prazos ou esquecer atividades importantes.

O dominio do TaskFlow e o gerenciamento de tarefas academicas. Cada tarefa possui titulo, descricao, disciplina, prioridade, data de entrega e status de conclusao.

## Funcionalidades Implementadas

- Cadastro de tarefas academicas.
- Listagem de tarefas.
- Filtro por status: todas, pendentes e concluidas.
- Consulta de tarefa por id no backend.
- Edicao de tarefas existentes.
- Exclusao de tarefas.
- Marcacao de tarefa como concluida.
- Reabertura de tarefa concluida.
- Contadores de total, pendentes e concluidas no frontend.
- Definicao de prioridade: baixa, media e alta.
- Indicacao visual de tarefa vencida.
- Cards em formato de sticky note.
- Movimentacao livre dos cards na tela.
- Escolha individual de cor para cada sticky note.
- Persistencia dos dados principais no SQLite.
- Persistencia local de posicao e cor dos cards via `localStorage`.

## Estrutura do Repositorio

```text
TaskFlow-Trabalho/
├── taskflow-backend/
│   ├── TaskFlow.API/
│   ├── TaskFlow.Aplicacao/
│   ├── TaskFlow.Dominio/
│   ├── TaskFlow.Repositorio/
│   ├── TaskFlow.sln
│   └── README.md
│
├── taskflow-frontend/
│   ├── src/
│   ├── package.json
│   └── README.md
│
└── README.md
```

## Backend

O backend e uma API REST desenvolvida em ASP.NET Core. Ele concentra a comunicacao HTTP, a orquestracao dos casos de uso, as regras de negocio e a persistencia dos dados.

### Tecnologias do Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / Swashbuckle

### Camadas do Backend

- `TaskFlow.API`: controllers, middlewares, Swagger, CORS e injecao de dependencia.
- `TaskFlow.Aplicacao`: orquestracao dos fluxos da aplicacao.
- `TaskFlow.Dominio`: entidades, enums, interfaces e regras de negocio.
- `TaskFlow.Repositorio`: acesso ao banco de dados com Entity Framework Core.

### Regras de Negocio

- Titulo obrigatorio.
- Limite maximo de caracteres para titulo.
- Limite maximo de caracteres para descricao.
- Disciplina obrigatoria.
- Limite maximo de caracteres para disciplina.
- Prioridade valida.
- Data de entrega obrigatoria.
- Impedimento de tarefas duplicadas com mesmo titulo e disciplina.

### Banco de Dados

O backend utiliza SQLite. O arquivo do banco esta versionado no projeto para esta entrega:

```text
taskflow-backend/TaskFlow.API/taskflow.db
```

A tabela principal e:

```text
Tarefas
```

### Executar o Backend

Na raiz do repositorio:

```powershell
cd taskflow-backend
dotnet restore TaskFlow.sln
dotnet build TaskFlow.sln -m:1
dotnet run --project TaskFlow.API
```

A API ficara disponivel em:

```text
http://localhost:5273
```

Swagger:

```text
http://localhost:5273/swagger
```

### Endpoints Principais

- `GET /api/tarefas`
- `GET /api/tarefas/{id}`
- `GET /api/tarefas?status=Pendente`
- `GET /api/tarefas?status=Concluida`
- `POST /api/tarefas`
- `PUT /api/tarefas/{id}`
- `DELETE /api/tarefas/{id}`
- `PATCH /api/tarefas/{id}/concluir`
- `PATCH /api/tarefas/{id}/reabrir`

## Frontend

O frontend e uma aplicacao React desenvolvida com Vite. Ele consome a API do backend e apresenta as tarefas como sticky notes, permitindo interacao visual com os registros.

### Tecnologias do Frontend

- React
- Vite
- JavaScript
- CSS
- Fetch API
- LocalStorage

### Funcionalidades Visuais

- Criacao e edicao de tarefas por formulario.
- Cards em formato de sticky note.
- Cores personalizaveis por card.
- Movimentacao livre dos cards.
- Salvamento local da posicao dos cards.
- Salvamento local da cor dos cards.
- Destaque para tarefas vencidas.
- Tooltip/aviso para datas vencidas ou vencendo no dia atual.
- Filtros e contadores de tarefas.

### Executar o Frontend

Antes de iniciar o frontend, deixe o backend rodando em:

```text
http://localhost:5273
```

Depois, em outro terminal, execute:

```powershell
cd taskflow-frontend
npm install
npm run dev
```

Acesse no navegador:

```text
http://localhost:5173
```

### Build do Frontend

```powershell
cd taskflow-frontend
npm run build
```

## Como Executar o Projeto Completo

1. Abrir um terminal para o backend:

```powershell
cd taskflow-backend
dotnet run --project TaskFlow.API
```

2. Abrir outro terminal para o frontend:

```powershell
cd taskflow-frontend
npm install
npm run dev
```

3. Acessar a aplicacao:

```text
http://localhost:5173
```

4. Opcionalmente, acessar o Swagger:

```text
http://localhost:5273/swagger
```

## O Que o MVP Entrega

O MVP permite demonstrar o fluxo principal do sistema:

1. Criar uma tarefa academica.
2. Persistir a tarefa no banco SQLite.
3. Listar as tarefas cadastradas.
4. Exibir as tarefas no frontend em formato de sticky note.
5. Editar dados da tarefa.
6. Filtrar por status.
7. Marcar como concluida ou reabrir.
8. Excluir tarefa.
9. Personalizar cor e posicao visual dos cards.

## Dificuldades Encontradas

- Definir um escopo pequeno o suficiente para a primeira entrega.
- Integrar frontend e backend mantendo as rotas consistentes.
- Separar regra de negocio no servico de dominio e orquestracao na camada de aplicacao.
- Configurar injecao de dependencia entre API, aplicacao, dominio e repositorio.
- Manter a organizacao visual dos sticky notes sem sobreposicao inicial.
- Guardar preferencias visuais no navegador usando `localStorage`.
- Visualizar e conferir os dados persistidos no SQLite durante o desenvolvimento.

## Pontos Dificeis de Manter ou Expandir

- A logica de posicionamento dos sticky notes pode ficar mais complexa com muitas tarefas.
- Cores e posicoes ficam no `localStorage`, entao nao sao compartilhadas entre dispositivos.
- O componente visual dos cards pode concentrar muitas responsabilidades se novas interacoes forem adicionadas.
- O backend ainda pode evoluir com testes automatizados e migrations do Entity Framework.
- A aplicacao nao possui autenticacao nem separacao de tarefas por usuario.
- Validacoes de entrada podem ser melhoradas no frontend e no backend.

## Possiveis Evolucoes Futuras

- Autenticacao de usuarios.
- Separacao de tarefas por usuario, turma ou disciplina.
- Calendario de entregas.
- Notificacoes de prazos proximos.
- Persistencia das posicoes e cores dos cards no backend.
- Melhorias de responsividade para telas pequenas.
- Testes automatizados.
- Refatoracao para separar melhor estado visual, regras de negocio e componentes.

## Uso de IA Generativa

Ferramentas de IA generativa utilizadas:

- Codex / OpenAI ChatGPT: apoio em implementacao, organizacao do codigo, correcoes, README e revisoes.
- Claude / Code Anthropic: apoio na criacao base do codigo.