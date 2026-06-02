# TaskFlow

## Projeto Final 01: Prototipo Inicial

TaskFlow e um sistema de gerenciamento de tarefas academicas desenvolvido como prototipo inicial para a disciplina. O objetivo do sistema e permitir o cadastro, consulta e acompanhamento de tarefas relacionadas a disciplinas, ajudando estudantes a organizar entregas, prioridades e status de conclusao.

Nesta primeira entrega, o foco do projeto foi construir um MVP funcional, com persistencia de dados e fluxo basico de uso de ponta a ponta, sem buscar uma arquitetura sofisticada ou uso antecipado de padroes de projeto.

## Descricao do Sistema

O sistema permite registrar tarefas academicas contendo titulo, descricao, disciplina, prioridade, data de entrega e status de conclusao. A API disponibiliza endpoints para criar, listar, atualizar, excluir, concluir e reabrir tarefas.

A interface utilizavel para demonstracao nesta etapa e o Swagger, que permite executar as operacoes da API diretamente pelo navegador.

## Funcionalidades Implementadas

- Cadastro de tarefas.
- Listagem de tarefas.
- Filtro de tarefas por status.
- Consulta de tarefa por id.
- Atualizacao de tarefa.
- Exclusao de tarefa.
- Marcacao de tarefa como concluida.
- Reabertura de tarefa concluida.
- Persistencia dos dados em banco SQLite.
- Documentacao e teste dos endpoints via Swagger.

## Tecnologias Utilizadas

- C#.
- .NET 10.
- ASP.NET Core Web API.
- Entity Framework Core.
- SQLite.
- Swagger / Swashbuckle.

## Estrutura do Projeto

- `TaskFlow.API`: camada de API, controllers, middlewares, configuracao da aplicacao e injecao de dependencia.
- `TaskFlow.Aplicacao`: camada de aplicacao, responsavel pela orquestracao dos fluxos.
- `TaskFlow.Dominio`: entidades, enums, interfaces e regras de negocio.
- `TaskFlow.Repositorio`: persistencia de dados usando Entity Framework Core e SQLite.

## Regras de Negocio

As regras de negocio estao concentradas na camada de dominio, especialmente nos servicos. Entre as validacoes implementadas estao:

- titulo obrigatorio;
- limite maximo de caracteres para titulo;
- limite maximo de caracteres para descricao;
- disciplina obrigatoria;
- limite maximo de caracteres para disciplina;
- prioridade valida;
- data de entrega obrigatoria;
- impedimento de tarefas duplicadas com mesmo titulo e disciplina.

## Pontos Difceis de Manter ou Expandir

Alguns pontos que podem exigir refatoracao em etapas futuras:

- Separacao mais clara entre entidade, servico de dominio e regras de validacao.
- Evolucao da camada de aplicacao para reduzir repeticao em fluxos de buscar, alterar e salvar.
- Criacao de testes automatizados para regras de negocio e endpoints.
- Melhor tratamento de DTOs e validacoes de entrada.
- Implementacao de um frontend dedicado, alem do Swagger.
- Organizacao de migrations do Entity Framework para evolucao controlada do banco.

## Dificuldades Encontradas

- Ajuste de namespaces e nomes de enums para manter consistencia entre as camadas.
- Separacao entre regra de negocio no servico de dominio e orquestracao na camada de aplicacao.
- Configuracao da injecao de dependencia entre API, aplicacao, dominio e repositorio.
- Visualizacao dos dados persistidos no SQLite durante o desenvolvimento.
- Organizacao dos status codes do controller sem poluir os metodos da API.

## Uso de IA Generativa

Foi utilizada IA generativa como apoio durante o desenvolvimento para:

- identificar erros de compilacao;
- auxiliar na organizacao das camadas;
- sugerir correcoes de codigo;
- apoiar a escrita deste README.

Ferramenta utilizada:

- Codex / ChatGPT.
- Claude / Code. Anthropic.

## Como Executar o Projeto

### Pre-requisitos

- .NET SDK instalado.
- Visual Studio, Visual Studio Code ou terminal com suporte ao `dotnet`.

### Executar pelo terminal

Na raiz do projeto, execute:

```powershell
dotnet restore TaskFlow.sln
dotnet build TaskFlow.sln -m:1
dotnet run --project TaskFlow.API
```

Depois acesse no navegador:

```text
http://localhost:5273/swagger
```

ou, se estiver usando o perfil HTTPS:

```text
https://localhost:7268/swagger
```

### Endpoints principais

- `GET /api/tarefas`: lista tarefas.
- `GET /api/tarefas/{id}`: busca tarefa por id.
- `POST /api/tarefas`: cria tarefa.
- `PUT /api/tarefas/{id}`: atualiza tarefa.
- `DELETE /api/tarefas/{id}`: exclui tarefa.
- `PATCH /api/tarefas/{id}/concluir`: marca tarefa como concluida.
- `PATCH /api/tarefas/{id}/reabrir`: reabre tarefa concluida.

## Banco de Dados

O sistema utiliza SQLite. O arquivo do banco fica em:

```text
TaskFlow.API/taskflow.db
```

A tabela principal e:

```text
Tarefas
```