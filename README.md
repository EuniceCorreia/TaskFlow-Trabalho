# TaskFlow

Sistema de gerenciamento de tarefas academicas desenvolvido como projeto final da disciplina.

## Integrantes

- Eunice Correia
- Maria Laura
- Vitoria Viana

---

## Projeto Final 02: Evolucao Arquitetural com Design Patterns

Esta segunda entrega evoluiu o prototipo inicial com a aplicacao de cinco Design Patterns (GoF) motivados por problemas reais identificados na arquitetura do MVP. O sistema permanece funcional e integrado apos a refatoracao.

### Problemas Identificados na Arquitetura Inicial

| Problema | Onde aparecia |
|---|---|
| Logica de transicao de status espalhada com `if/else` | `Tarefa.cs` e `AplicTarefa.cs` |
| Algoritmos de filtro/ordenacao misturados em um unico `switch` estatico | `FiltroTarefaStrategyFactory.cs` |
| Ausencia de mecanismo de notificacao desacoplado | Sem observadores |
| Controller acoplado diretamente a todos os metodos da camada de aplicacao | `TarefasController.cs` |
| Factory estatica misturando dois tipos de operacao distintos | `FiltroTarefaStrategyFactory.cs` |

### Novos Requisitos Recebidos

- O status de uma tarefa deve seguir transicoes controladas: nao e possivel fazer qualquer transicao a partir de qualquer estado.
- O sistema deve notificar automaticamente quando uma tarefa e criada, concluida, pausada ou esta proxima do vencimento.
- Deve ser possivel filtrar e ordenar tarefas de formas distintas sem alterar o core do sistema.
- A camada HTTP nao deve depender diretamente de todos os detalhes da camada de aplicacao.

### Design Patterns Aplicados

#### 1. State — Comportamental

**Problema resolvido:** a entidade `Tarefa` controlava transicoes de status com condicionais diretas. Qualquer mudanca de regra exigia alterar a propria entidade.

**Solucao anterior inadequada:**
```csharp
// logica de transicao diretamente na entidade
if (Status == Pendente) Status = Concluida;
else throw new Exception("...");
```

**Solucao com State:** cada estado e uma classe que conhece apenas suas proprias transicoes validas.

```
IEstadoTarefa
├── EstadoPendente   → Pausar() ou Concluir()
├── EstadoPausada    → Retomar() ou Concluir()
└── EstadoConcluida  → Reabrir()
```

**Justificativa tecnica:** as regras de cada estado ficam isoladas em classes proprias. A entidade `Tarefa` delega a validacao da operacao para o estado atual, em vez de concentrar todas as transicoes em blocos de `if/else`.

**Impacto:** eliminacao das condicionais de validacao de transicao dentro dos metodos `Pausar`, `Retomar`, `Concluir` e `Reabrir`. Erros de transicao invalida lancam `RegraDeNegocioException` com mensagem clara.

**Limitacao atual:** a entidade ainda possui o metodo `ObterEstadoAtual()`, que usa um `switch` para associar o enum persistido a uma implementacao de `IEstadoTarefa`. Portanto, adicionar um novo estado exige criar sua classe e registrar esse estado nesse metodo, mas nao exige modificar as regras internas dos estados existentes.

---

#### 2. Strategy — Comportamental

**Problema resolvido:** filtros e ordenacoes estavam acoplados ao servico de listagem. Adicionar um novo criterio exigia alterar `AplicTarefa`.

**Solucao anterior inadequada:** logica de ordenacao inline no metodo `ListarAsync`.

**Solucao com Strategy:** cada algoritmo de filtro ou ordenacao e uma classe independente intercambiavelmente.

```
IFiltroTarefaStrategy
├── OrdenarPorDataStrategy
├── OrdenarPorPrioridadeStrategy
├── OrdenarPorTituloStrategy
├── FiltrarAtrasadasStrategy
└── FiltrarConcluidasStrategy
```

**Justificativa tecnica:** `AplicTarefa` nao implementa os algoritmos de ordenacao ou filtragem. Ela informa a opcao solicitada para `FiltroTarefaFactory`, recebe uma implementacao de `IFiltroTarefaStrategy` e executa o algoritmo por meio da interface comum. Cada estrategia pode ser testada de forma isolada.

**Impacto:** os algoritmos deixaram de ficar misturados no fluxo de listagem. Novos criterios sao implementados em novas classes Strategy e registrados na Factory correspondente, sem aumentar a complexidade de `ListarAsync`.

---

#### 3. Observer — Comportamental

**Problema resolvido:** ausencia de notificacoes desacopladas. Qualquer canal de notificacao futuro exigiria alterar o fluxo principal.

**Solucao com Observer:** o `GerenciadorNotificacoes` recebe todos os observadores via injecao de dependencia e os notifica a cada evento relevante.

```
IObservadorTarefa
├── NotificacaoTelaObservador   → log de eventos para exibicao
├── EmailObservador             → simula envio de e-mail para vencimento
└── LogSistemaObservador        → registra todos os eventos no log
```

**Eventos disparados:** `Criada`, `Concluida`, `Pausada`, `ProximaDoVencimento`, `Atrasada`.

**Justificativa tecnica:** adicionar um novo canal, como push mobile ou SMS, exige criar uma classe que implemente `IObservadorTarefa` e registra-la no container de injecao de dependencia. O caso de uso nao precisa conhecer a classe concreta do novo canal.

**Impacto:** reducao significativa do acoplamento entre quem publica o evento e os canais concretos que reagem a ele. `TarefaAplic` ainda conhece `IGerenciadorNotificacoes` e decide quando publicar cada `EventoTarefa`, mas nao depende de `EmailObservador`, `LogSistemaObservador` ou `NotificacaoTelaObservador`.

**Observacao sobre a implementacao atual:** `EmailObservador` simula o envio por meio de log. `NotificacaoTelaObservador` tambem registra uma mensagem no log do backend; ele nao envia diretamente a notificacao para o React. As notificacoes visuais do frontend sao montadas pelo proprio frontend a partir das tarefas carregadas.

---

#### 4. Factory Method — Criacional

**Problema resolvido:** `FiltroTarefaStrategyFactory` era uma classe estatica com um unico `switch` que misturava dois tipos distintos de operacao (ordenacao e filtragem), dificultando extensao e violando o principio de responsabilidade unica.

**Solucao anterior inadequada:**
```csharp
// estatica, sem hierarquia, mistura dois dominios
public static IFiltroTarefaStrategy? Criar(EnumOrdenacaoTarefa? ordenacao) => ordenacao switch { ... }
```

**Solucao com Factory Method:** classe abstrata declara o Factory Method `CriarStrategy()`. Subclasses concretas especializam a criacao.

```
FiltroTarefaFactory (abstract)
│   CriarStrategy()  ← Factory Method
├── OrdenacaoFactory  → PorData, PorPrioridade, PorTitulo
└── FiltragemFactory  → ApenasAtrasadas, ApenasConcluidas
```

**Justificativa tecnica:** o metodo abstrato `CriarStrategy()` define o ponto de criacao, enquanto `OrdenacaoFactory` e `FiltragemFactory` decidem quais Strategies concretas podem criar. Assim, a criacao fica separada do uso realizado por `TarefaAplic`.

**Impacto:** separacao entre os grupos de ordenacao e filtragem e retirada da instanciacao direta de Strategies da camada de aplicacao.

**Limitacao atual:** `ObterFactory()` ainda possui um `switch` para escolher entre `OrdenacaoFactory` e `FiltragemFactory`, e cada Factory concreta possui seu proprio `switch`. Um novo criterio exige criar a Strategy e registra-la na Factory correspondente. Mesmo assim, a logica de criacao deixou de ficar misturada ao caso de uso.

---

#### 5. Facade — Estrutural

**Problema resolvido:** `TarefasController` dependia diretamente de `ITarefaAplic` e precisava conhecer separadamente todos os casos de uso, incluindo quatro operacoes de transicao de estado. A camada HTTP ficava diretamente ligada ao contrato completo da aplicacao.

**Solucao anterior inadequada:**
```csharp
// controller conhecia todos os metodos de transicao individualmente
private readonly ITarefaAplic _tarefaAplic;
await _tarefaAplic.PausarAsync(id, ct);
await _tarefaAplic.RetomarAsync(id, ct);
await _tarefaAplic.ConcluirAsync(id, ct);
await _tarefaAplic.ReabrirAsync(id, ct);
```

**Solucao com Facade:** `TarefaFacade` encapsula `ITarefaAplic` e expoe uma interface simplificada. As 4 transicoes sao unificadas em `TransicionarAsync`.

```
TarefasController → ITarefaFacade → ITarefaAplic
```

```csharp
// controller agora delega sem conhecer qual metodo chamar
await _facade.TransicionarAsync(id, "pausar", ct);
```

**Justificativa tecnica:** o controller passou a depender de uma interface voltada para a API, `ITarefaFacade`. A Facade esconde a chamada concreta da camada de aplicacao e oferece um ponto unico para as transicoes. Ela tambem pode futuramente coordenar outros servicos sem expor essa complexidade ao controller.

**Impacto:** reducao do conhecimento do controller sobre o contrato interno da aplicacao. As quatro transicoes continuam possuindo rotas HTTP distintas, mas sao encaminhadas pela operacao unica `TransicionarAsync`.

**Limitacao atual:** `TransicionarAsync` recebe a acao como `string` (`pausar`, `retomar`, `concluir` ou `reabrir`). Isso funciona, mas um enum de acao seria mais seguro contra erros de digitacao em uma evolucao futura.

---

### Mapa de Patterns na Arquitetura

```
Frontend (React)
      |
      v
TaskFlow.API
  TarefasController → ITarefaFacade         [Facade]
                           |
                           v
TaskFlow.Aplicacao
  TarefaAplic → FiltroTarefaFactory         [Factory Method]
             → IFiltroTarefaStrategy        [Strategy]
             → IGerenciadorNotificacoes     [Observer]
                           |
                           v
TaskFlow.Dominio
  Tarefa → IEstadoTarefa                    [State]
  EstadoPendente / EstadoPausada / EstadoConcluida
                           |
                           v
TaskFlow.Repositorio
  TarefaRep → SQLite (Entity Framework Core)
```

---

### Modelo de Estados (State Pattern)

| Status | Pode Pausar | Pode Retomar | Pode Concluir | Pode Reabrir |
|---|---|---|---|---|
| Pendente | Sim → Pausada | Nao | Sim → Concluida | Nao |
| Pausada | Nao | Sim → Pendente | Sim → Concluida | Nao |
| Concluida | Nao | Nao | Nao | Sim → Pendente |

**Vencida** e um indicador visual automatico no frontend: aparece quando o status e `Pendente` ou `Pausada` e a data de entrega ja passou. Nao e um estado persistido.

---

## Projeto Final 01: Prototipo Inicial

O MVP inicial entregou um sistema funcional de ponta a ponta com:

- Cadastro, listagem, edicao e exclusao de tarefas academicas.
- Persistencia em SQLite via Entity Framework Core.
- Interface React com cards em formato de sticky note.
- Filtro por status, contadores, definicao de prioridade.
- Personalizacao de cor e posicao dos cards com persistencia em `localStorage`.

---

## Problema e Dominio

Estudantes precisam acompanhar varias tarefas academicas ao mesmo tempo, cada uma com disciplina, prazo, prioridade e status diferente. Sem uma organizacao visual simples, e facil perder prazos ou esquecer atividades importantes.

O dominio do TaskFlow e o gerenciamento de tarefas academicas. Cada tarefa possui titulo, descricao, disciplina, prioridade, data de entrega e status.

---

## Funcionalidades do Sistema

- Cadastro de tarefas academicas.
- Listagem de tarefas com filtro por status.
- Filtros e ordenacoes: por data, prioridade, titulo, apenas atrasadas, apenas concluidas.
- Edicao e exclusao de tarefas.
- Transicoes de estado: pausar, retomar, concluir, reabrir.
- Indicacao visual automatica de tarefa vencida.
- Notificacoes internas por Observer (log, e-mail simulado, tela).
- Cards em formato de sticky note com cor e posicao personalizaveis.
- Persistencia de posicao e cor dos cards via `localStorage`.

---

## Estrutura do Repositorio

```text
TaskFlow-Trabalho/
├── taskflow-backend/
│   ├── TaskFlow.API/
│   │   ├── Controllers/
│   │   ├── Facade/              ← Facade (novo)
│   │   ├── Convencoes/
│   │   └── Middlewares/
│   ├── TaskFlow.Aplicacao/
│   │   ├── Tarefa/
│   │   └── Observadores/        ← Observer (novo)
│   ├── TaskFlow.Dominio/
│   │   ├── Tarefa/
│   │   │   └── Estado/          ← State (novo)
│   │   ├── Filtros/             ← Strategy + Factory Method (novo)
│   │   └── Observadores/
│   ├── TaskFlow.Repositorio/
│   └── TaskFlow.sln
├── taskflow-frontend/
│   ├── src/
│   └── package.json
└── README.md
```

---

## Backend

### Tecnologias

- C# / .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / Swashbuckle

### Camadas

- `TaskFlow.API`: controllers, facade, middlewares, Swagger, CORS e injecao de dependencia.
- `TaskFlow.Aplicacao`: orquestracao dos casos de uso, observadores e mapper.
- `TaskFlow.Dominio`: entidades, estados, filtros, enums, interfaces e regras de negocio.
- `TaskFlow.Repositorio`: acesso ao banco de dados com Entity Framework Core.

### Regras de Negocio

- Titulo obrigatorio, maximo 120 caracteres.
- Descricao com maximo 1000 caracteres.
- Disciplina obrigatoria, maximo 100 caracteres.
- Prioridade valida (Baixa, Media, Alta).
- Data de entrega obrigatoria.
- Impedimento de tarefas duplicadas com mesmo titulo e disciplina.
- Transicoes de estado validadas pelo pattern State.

### Banco de Dados

O backend utiliza SQLite. O arquivo gerado automaticamente pelo `EnsureCreatedAsync` e:

```text
taskflow-backend/TaskFlow.API/taskflow.db
```

> Se o arquivo existir com dados do modelo anterior (com status `EmAndamento` ou `Cancelada`), delete-o antes de iniciar a API. O banco sera recriado automaticamente.

### Executar o Backend

```powershell
cd taskflow-backend
dotnet restore TaskFlow.sln
dotnet run --project TaskFlow.API
```

API disponivel em `http://localhost:5273`. Swagger em `http://localhost:5273/swagger`.

### Endpoints

| Metodo | Rota | Descricao |
|---|---|---|
| GET | `/api/tarefas` | Lista todas as tarefas |
| GET | `/api/tarefas?status=Pendente` | Filtra por status |
| GET | `/api/tarefas?ordenacao=PorData` | Ordena/filtra |
| GET | `/api/tarefas/{id}` | Obtem tarefa por id |
| POST | `/api/tarefas` | Cria nova tarefa |
| PUT | `/api/tarefas/{id}` | Atualiza tarefa |
| DELETE | `/api/tarefas/{id}` | Exclui tarefa |
| PATCH | `/api/tarefas/{id}/pausar` | Pendente → Pausada |
| PATCH | `/api/tarefas/{id}/retomar` | Pausada → Pendente |
| PATCH | `/api/tarefas/{id}/concluir` | Pendente ou Pausada → Concluida |
| PATCH | `/api/tarefas/{id}/reabrir` | Concluida → Pendente |

**Valores validos para `ordenacao`:** `PorData`, `PorPrioridade`, `PorTitulo`, `ApenasAtrasadas`, `ApenasConcluidas`.

---

## Frontend

### Tecnologias

- React / Vite
- JavaScript
- CSS
- Fetch API / LocalStorage

### Funcionalidades Visuais

- Cards em formato de sticky note com cores personalizaveis.
- Movimentacao livre dos cards com posicao salva em `localStorage`.
- Botoes de acao por estado: Pausar, Retomar, Concluir, Reabrir.
- Badge visual automatico para tarefas vencidas.
- Filtros e contadores de tarefas.
- Formulario de criacao e edicao.

### Executar o Frontend

Com o backend rodando em `http://localhost:5273`:

```powershell
cd taskflow-frontend
npm install
npm run dev
```

Acesse em `http://localhost:5173`.

---

## Como Executar o Projeto Completo

```powershell
# Terminal 1 — backend
cd taskflow-backend
dotnet run --project TaskFlow.API

# Terminal 2 — frontend
cd taskflow-frontend
npm install
npm run dev
```

Acesse `http://localhost:5173`. Swagger em `http://localhost:5273/swagger`.

---

## Uso de IA Generativa

- Claude Code (Anthropic): apoio na implementacao dos Design Patterns, refatoracao arquitetural e atualizacao do README.
- ChatGPT (OpenAI): apoio na implementacao base, organizacao do codigo e revisoes.

