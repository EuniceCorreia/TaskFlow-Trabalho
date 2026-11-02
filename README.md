# TaskFlow

Sistema de gerenciamento de tarefas academicas. O projeto foi desenvolvido inicialmente na disciplina de **Design Patterns** (Projetos Finais 01 e 02) e reaproveitado na disciplina de **DevOps**, onde recebeu uma pipeline DevSecOps completa: build, analise de qualidade, analise de seguranca, publicacao da imagem e deploy em um cluster Kubernetes (kind).

## Integrantes

**DevOps (Pipeline DevSecOps)**

- Eunice Correia
- Janaina Carlos
- Maria Laura
- Vitoria Viana

**Design Patterns (Projetos Finais 01 e 02)**

- Eunice Correia
- Maria Laura
- Vitoria Viana

---

## Projeto DevOps: Pipeline DevSecOps

Nesta etapa o codigo do TaskFlow foi mantido e o foco passou a ser a entrega automatizada. Foi construida uma pipeline no GitHub Actions que leva o codigo do commit ate o cluster kind, com analise de qualidade (SonarQube) e de seguranca (Trivy) bloqueando o deploy quando encontram problemas.

### Fluxo da Pipeline

```
git push na main
   |
   ├─▶ 1. Build ──▶ 3. Trivy ──┐
   |                           ├─▶ 4. Push ──▶ 5. Deploy (kind)
   └─▶ 2. Sonar ───────────────┘
```

Build e Sonar rodam em paralelo. O Trivy analisa as imagens geradas no Build. O **Push so acontece se Sonar e Trivy passarem**, e o Deploy so acontece depois do Push. Assim, nenhuma imagem reprovada chega ao Docker Hub nem ao cluster (shift left da seguranca).

### Etapas

| Etapa | O que faz | Onde roda |
|---|---|---|
| 1. Build | Gera as imagens Docker do frontend (React + nginx) e do backend (.NET 10), marcadas com o SHA do commit | GitHub (Linux) |
| 2. Sonar | Lint (ESLint), testes com cobertura (Vitest) e analise no SonarQube Cloud: bugs, vulnerabilidades e code smells. Falha se o Quality Gate reprovar | GitHub (Linux) |
| 3. Trivy | Analisa o codigo (dependencias, segredos expostos, Dockerfiles e manifests Kubernetes) e as duas imagens. Falha com vulnerabilidades HIGH ou CRITICAL que tenham correcao | GitHub (Linux) |
| 4. Push | Publica as imagens aprovadas no Docker Hub (tag do commit e `latest`) | GitHub (Linux) |
| 5. Deploy | Aplica os manifests no cluster kind usando as imagens do Docker Hub e confere se o frontend e a API respondem | Self-hosted runner (Windows) |

### Ferramentas

- **GitHub Actions**: orquestracao da pipeline (`.github/workflows/devsecops.yml`).
- **Docker**: imagens multi-stage para frontend e backend.
- **SonarQube Cloud**: analise estatica e Quality Gate do frontend.
- **Vitest + ESLint**: testes automatizados e padrao de codigo do frontend.
- **Trivy**: scan de vulnerabilidades, segredos e configuracoes inseguras.
- **Docker Hub**: registro das imagens (`taskflow-frontend` e `taskflow-backend`).
- **kind (Kubernetes in Docker)**: cluster Kubernetes local onde a aplicacao roda.
- **GitHub Self-hosted Runner**: executa o deploy na maquina onde o cluster kind esta.

### Relatorios

- **SonarQube Cloud**: painel do projeto `EuniceCorreia_TaskFlow-Trabalho`.
- **GitHub → aba Security → Code scanning**: resultados do Trivy (codigo, imagem frontend e imagem backend) em formato SARIF.
- **Artefato `relatorios-trivy`**: relatorios em texto e SARIF anexados a cada execucao da pipeline.

### Seguranca Aplicada (DevSecOps)

- Permissoes minimas do `GITHUB_TOKEN` (`contents: read`). Apenas o job do Trivy recebe permissao extra para enviar relatorios.
- Credenciais em **secrets** (`DOCKERHUB_TOKEN`, `SONAR_TOKEN`) e dados nao sensiveis em **variables** (`DOCKERHUB_USERNAME`). Nenhuma senha no codigo.
- Docker Hub acessado por **access token**, e nao pela senha da conta.
- Containers rodando **sem root**: nginx com usuario 101 e .NET com usuario 1654.
- Manifests Kubernetes com sistema de arquivos **somente leitura**, sem escalada de privilegio, sem capabilities do Linux e com limites de CPU e memoria.
- Imagens finais enxutas: o frontend leva apenas os arquivos compilados no nginx e o backend usa a imagem runtime Alpine, sem SDK nem codigo-fonte.
- Versao do Trivy fixada (`aquasecurity/trivy-action@v0.36.0`) em vez de `@master`, como protecao contra ataques a cadeia de suprimentos.
- Deploy executado apenas a partir da branch `main`.

### Arquitetura no Cluster

```
Navegador → localhost:8080
      |
      v
Service taskflow-frontend (NodePort 30080)
      |
      v
Pods do frontend (nginx, 2 replicas)
   ├── /       → arquivos do React
   └── /api/*  → Service taskflow-backend:8080
                       |
                       v
               Pod do backend (.NET 10, 1 replica)
                       |
                       v
               PersistentVolumeClaim (SQLite em /data)
```

O backend usa 1 replica com estrategia `Recreate` porque o SQLite aceita apenas um processo gravando por vez. O banco fica em um volume persistente e sobrevive a novos deploys. Ele e separado do banco local de desenvolvimento.

### Configuracao Necessaria

**GitHub (Settings → Secrets and variables → Actions)**

| Tipo | Nome | Conteudo |
|---|---|---|
| Secret | `DOCKERHUB_TOKEN` | Access token do Docker Hub (Read & Write) |
| Secret | `SONAR_TOKEN` | Token do SonarQube Cloud |
| Variable | `DOCKERHUB_USERNAME` | Usuario do Docker Hub |

**SonarQube Cloud**: projeto importado e **Analise Automatica desligada** (a analise e feita pela pipeline).

**Maquina do deploy (Windows com Docker Desktop, kind e kubectl)**

```powershell
# Criar o cluster com a porta 8080 mapeada
kind create cluster --config kind-config.yaml

# Liberar scripts locais para os passos em PowerShell do runner
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

Registrar o self-hosted runner em Settings → Actions → Runners → New self-hosted runner (Windows), com o label `kind`, e manter o `run.cmd` em execucao.

### Executar a Pipeline

Qualquer push na `main` dispara a pipeline. Ela tambem pode ser executada manualmente em Actions → DevSecOps Pipeline → Run workflow.

Depois do deploy:

- Aplicacao: `http://localhost:8080`
- API pelo frontend: `http://localhost:8080/api/tarefas`

Comandos uteis para conferir o cluster:

```powershell
kubectl get deploy,pods,svc,pvc
kubectl logs deployment/taskflow-backend --tail 20
kubectl get deploy taskflow-frontend -o jsonpath="{.spec.template.spec.containers[0].image}"
```

### Ajustes de Qualidade Feitos no Codigo

- Criados 21 testes automatizados para `src/api/tarefas.js`, com 100% de cobertura nesse modulo.
- Corrigidos dois erros do ESLint (`react-hooks/set-state-in-effect`):
  - `TarefaForm.jsx`: o estado do formulario passou a ser inicializado a partir da tarefa (`formInicial`), sem `useEffect`.
  - `App.jsx`: a funcao `carregar` passou a usar `.then/.catch/.finally`. Isso tambem eliminou o efeito de "piscar" do quadro a cada acao.

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

**Impacto:** eliminacao das condicionais de validacao de transicao dentro dos metodos `Pausar`, `Retomar`, `Concluir`e `Reabrir`. Erros de transicao invalida lancam `RegraDeNegocioException` com mensagem clara.

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

**Impacto:** reducao significativa do acoplamento entre quem publica o evento e os canais concretos que reagem a ele.`TarefaAplic` ainda conhece `IGerenciadorNotificacoes` e decide quando publicar cada `EventoTarefa`, mas nao depende de `EmailObservador`, `LogSistemaObservador` ou `NotificacaoTelaObservador`.

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

**Justificativa tecnica:** o metodo abstrato `CriarStrategy()` define o ponto de criacao, enquanto `OrdenacaoFactory`e `FiltragemFactory` decidem quais Strategies concretas podem criar. Assim, a criacao fica separada do uso realizado por `TarefaAplic`.

**Impacto:** separacao entre os grupos de ordenacao e filtragem e retirada da instanciacao direta de Strategies da camada de aplicacao.

**Limitacao atual:** `ObterFactory()` ainda possui um `switch` para escolher entre `OrdenacaoFactory` e `FiltragemFactory`, e cada Factory concreta possui seu proprio `switch`. Um novo criterio exige criar a Strategy e registra-la na Factory correspondente. Mesmo assim, a logica de criacao deixou de ficar misturada ao caso de uso.

---

#### 5. Facade — Estrutural

**Problema resolvido:** `TarefasController` dependia diretamente de `ITarefaAplic` e precisava conhecer separadamentetodos os casos de uso, incluindo quatro operacoes de transicao de estado. A camada HTTP ficava diretamente ligada ao contrato completo da aplicacao.

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

Estudantes precisam acompanhar varias tarefas academicas ao mesmo tempo, cada uma com disciplina, prazo, prioridade estatus diferente. Sem uma organizacao visual simples, e facil perder prazos ou esquecer atividades importantes.

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
├── .github/
│   └── workflows/
│       └── devsecops.yml        ← Pipeline DevSecOps (DevOps)
├── k8s/                         ← Manifests Kubernetes (DevOps)
│   ├── backend-deployment.yaml
│   ├── backend-pvc.yaml
│   ├── backend-service.yaml
│   ├── frontend-deployment.yaml
│   └── frontend-service.yaml
├── taskflow-backend/
│   ├── Dockerfile               ← Imagem do backend (DevOps)
│   ├── TaskFlow.API/
│   │   ├── Controllers/
│   │   ├── Facade/              ← Facade
│   │   ├── Convencoes/
│   │   └── Middlewares/
│   ├── TaskFlow.Aplicacao/
│   │   ├── Tarefa/
│   │   └── Observadores/        ← Observer
│   ├── TaskFlow.Dominio/
│   │   ├── Tarefa/
│   │   │   └── Estado/          ← State
│   │   ├── Filtros/             ← Strategy + Factory Method
│   │   └── Observadores/
│   ├── TaskFlow.Repositorio/
│   └── TaskFlow.sln
├── taskflow-frontend/
│   ├── Dockerfile               ← Imagem do frontend (DevOps)
│   ├── nginx.conf               ← Servidor web e proxy /api (DevOps)
│   ├── sonar-project.properties ← Configuracao do SonarQube (DevOps)
│   ├── src/
│   │   └── api/tarefas.test.js  ← Testes automatizados (DevOps)
│   └── package.json
├── kind-config.yaml             ← Cluster kind com porta 8080 (DevOps)
├── .trivyignore                 ← Excecoes justificadas do Trivy (DevOps)
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

O backend utiliza SQLite. Em desenvolvimento, o arquivo gerado automaticamente pelo `EnsureCreatedAsync` e:

```text
taskflow-backend/TaskFlow.API/taskflow.db
```

No container e no cluster, o caminho e definido pela variavel `ConnectionStrings__TaskFlow` (`/data/taskflow.db`). O arquivo `.db` local nao entra na imagem Docker.

> Se o arquivo existir com dados do modelo anterior (com status `EmAndamento` ou `Cancelada`), delete-o antes de iniciar a API. O banco sera recriado automaticamente.

### Executar o Backend

```powershell
cd taskflow-backend
dotnet restore TaskFlow.sln
dotnet run --project TaskFlow.API
```

API disponivel em `http://localhost:5273`. Swagger em `http://localhost:5273/swagger`.

### Executar o Backend com Docker

```powershell
docker build -t taskflow-backend taskflow-backend
docker run --rm -p 5080:8080 taskflow-backend
```

API disponivel em `http://localhost:5080/api/tarefas` (o Swagger fica desligado fora do ambiente de desenvolvimento).

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
- Vitest (testes) e ESLint (padrao de codigo)

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

### Testes e Lint

```powershell
cd taskflow-frontend
npm run lint
npm run test:coverage
```

---

## Como Executar o Projeto Completo

**Em desenvolvimento:**

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

**No cluster kind (via pipeline):** faca push na `main` e acompanhe em Actions. Ao final, acesse `http://localhost:8080`.

---

## Uso de IA Generativa

- Claude Code (Anthropic): apoio na implementacao dos Design Patterns, refatoracao arquitetural e atualizacao do README.
- Claude (Anthropic): apoio na construcao da pipeline DevSecOps (GitHub Actions, Dockerfiles, manifests Kubernetes, configuracao do SonarQube e do Trivy), nos testes automatizados, nas correcoes de lint.
- ChatGPT (OpenAI): apoio na implementacao base, organizacao do codigo e revisoes.