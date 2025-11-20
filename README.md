# CareerMap.Recommendations.Api - API de Recomendações (.NET 8)

Esta é a implementação da API de Recomendações para o projeto **CareerMap**, desenvolvida em .NET 8, conforme os requisitos da disciplina **Advanced Business Development with .NET** da Global Solution 2025.

## 🚀 Requisitos Atendidos

| Requisito | Status | Detalhes da Implementação |
| :--- | :--- | :--- |
| **1. Boas Práticas REST** | ✅ Completo | Uso de verbos HTTP corretos (GET, POST, PUT, DELETE), Status Codes adequados (200, 201, 204, 404) e implementação de **Paginação** e **HATEOAS** no endpoint de `Carreiras`. |
| **2. Monitoramento e Observabilidade** | ✅ Completo | Implementação de **Health Checks** (`/health/ready` e `/health/live`) e **Logging** estruturado via Serilog. |
| **3. Versionamento da API** | ✅ Completo | Estrutura de rotas com versionamento explícito (`api/v1/[controller]`). |
| **4. Integração e Persistência** | ✅ Completo | Uso de **Entity Framework Core** com banco de dados SQLite (`CareerMapRecommendations.db`). O banco é criado e populado automaticamente com dados iniciais (Seed Data) ao iniciar a aplicação em ambiente de desenvolvimento. |
| **5. Testes Integrados** | ✅ Completo | Implementação de testes de integração com **xUnit** e `WebApplicationFactory`, utilizando um banco de dados **InMemory** para isolamento e agilidade. |

## 🛠️ Como Rodar o Projeto Localmente

### Pré-requisitos

*   .NET 8 SDK instalado.

### Passos

1.  **Navegue até a pasta da API:**
    ```bash
    cd CareerMap.Recommendations/CareerMap.Recommendations.Api
    ```
2.  **Rode a aplicação:**
    ```bash
    dotnet run
    ```
3.  **Acesse o Swagger:**
    A API estará disponível em `http://localhost:5000` (ou outra porta configurada). O Swagger UI (documentação interativa) estará em:
    ```
    http://localhost:5000/swagger
    ```

## 🔗 Endpoints Principais (v1)

| Método | Rota | Descrição | Boas Práticas |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/Carreiras` | Lista paginada de carreiras. | Paginação, HATEOAS |
| `GET` | `/api/v1/Carreiras/{id}` | Detalhe de uma carreira. | Status Codes |
| `POST` | `/api/v1/Carreiras` | Cria uma nova carreira. | Status Code 201 (Created), HATEOAS |
| `PUT` | `/api/v1/Carreiras/{id}` | Atualiza uma carreira. | Status Code 204 (No Content) |
| `DELETE` | `/api/v1/Carreiras/{id}` | Exclui uma carreira. | Status Code 204 (No Content) |
| `GET` | `/health/ready` | Health Check de prontidão (inclui status do DB). | Observabilidade |
| `GET` | `/health/live` | Health Check de atividade. | Observabilidade |

## 🧪 Executando os Testes

1.  **Navegue até a pasta raiz da solução:**
    ```bash
    cd CareerMap.Recommendations
    ```
2.  **Execute os testes:**
    ```bash
    dotnet test
    ```

## ⚙️ Estrutura do Projeto

O projeto segue a arquitetura de Camadas (Domain, Infrastructure, API):

*   **`CareerMap.Recommendations.Domain`**: Contém as entidades de negócio (`Carreira`, `Competencia`).
*   **`CareerMap.Recommendations.Infrastructure`**: Contém a lógica de persistência (Entity Framework Core, `RecommendationsDbContext`, Migrations).
*   **`CareerMap.Recommendations.Api`**: O projeto principal que expõe os *endpoints* REST, contém os *Controllers*, DTOs e a configuração de *middleware* (Swagger, Serilog, Health Checks).
*   **`CareerMap.Recommendations.Tests`**: Contém os testes de integração.

---
*Desenvolvido por Manus AI para Global Solution 2025.*


---

# Projeto GS.NET2 - DevOps Tools & Cloud Computing

## 1. Descrição da Solução Proposta

Este projeto implementa uma API RESTful em **ASP.NET Core 8** para gerenciar informações de **Carreiras** e **Competências** (entidades principais), simulando um sistema de recomendação de carreira. A solução atende integralmente aos requisitos do desafio de **DevOps Tools & Cloud Computing**, utilizando **Azure DevOps** para CI/CD e **Azure CLI** para provisionamento de infraestrutura em nuvem.

### Arquitetura Macro

A arquitetura é baseada em três pilares principais:

1.  **Aplicação (.NET API):** Uma API RESTful que expõe endpoints de CRUD para as entidades.
2.  **Infraestrutura (Azure):** Provisionada via Azure CLI, composta por um **Azure Web App (PaaS)** para hospedagem da API e um **Azure SQL Database (PaaS)** para persistência de dados.
3.  **DevOps (Azure DevOps):** Utiliza **Azure Repos** para código-fonte, **Azure Boards** para gestão de tarefas e **Azure Pipelines** para automação de Build (CI) e Release (CD).

**Ferramenta Sugerida para Desenho:** Visual Paradigm Azure Diagram.

## 2. Tarefas Obrigatórias e Implementação

| Requisito | Status | Detalhes da Implementação |
| :--- | :--- | :--- |
| **1) Provisionamento em Nuvem (Azure CLI)** | **Implementado** | Script `scripts/script-infra.sh` cria o Resource Group, Azure SQL Server, Azure SQL Database, App Service Plan e Azure Web App. |
| **2) Projeto no Azure DevOps** | **Manual** | O projeto deve ser criado no Azure DevOps, e este repositório deve ser importado para o **Azure Repos**. |
| **3) Código no Azure Repos** | **Implementado** | Código-fonte em .NET Core 8. |
| **4) Azure Boards** | **Manual** | Criar uma **Tarefa inicial** e vincular commits, branches e Pull Requests a ela. |
| **5) Pipeline de Build (CI)** | **Implementado** | Arquivo `azure-pipeline.yml` na raiz. Roda automaticamente a cada commit na branch principal, executa testes (XUnit) e publica artefatos. |
| **6) Pipeline de Release (CD)** | **Manual** | Deve ser criado no Azure DevOps (Classic ou YAML multi-stage) para rodar automaticamente após o Build e fazer o deploy no Azure Web App (PaaS). |
| **7) Deploy** | **Implementado** | O script de infraestrutura provisiona um **Web App PaaS**. O `azure-pipeline.yml` gera o artefato de deploy para este ambiente. |
| **8) Banco de dados** | **Implementado** | Utiliza **Serviço PaaS (Azure SQL)**. O script `script-bd.sql` cria as tabelas e dados de exemplo. |
| **9) Imagens oficiais** | **Implementado** | O `Dockerfile` utiliza imagens oficiais da Microsoft (`mcr.microsoft.com/dotnet/sdk` e `mcr.microsoft.com/dotnet/aspnet`). |
| **10) Scripts de infraestrutura** | **Implementado** | Arquivo `script-infra.sh` em `/scripts`. |
| **11) Arquivo script-bd.sql** | **Implementado** | Arquivo `script-bd.sql` na raiz do repositório. |
| **12) Scripts Azure CLI com prefixo `script-infra`** | **Implementado** | Arquivo `scripts/script-infra.sh`. |
| **13) Dockerfiles** | **Implementado** | Arquivo `dockerfiles/Dockerfile` em `/dockerfiles`. |
| **14) Arquivo azure-pipeline.yml** | **Implementado** | Arquivo `azure-pipeline.yml` na raiz. |
| **16) Variáveis de ambiente** | **Implementado** | O script `script-infra.sh` configura a Connection String como Application Setting no Web App, atendendo ao requisito de proteger dados sensíveis. |

## 3. CRUD Exposto em JSON (API)

A API expõe endpoints de CRUD para a entidade principal **Carreira** (e indiretamente para **Competência**).

**Base URL:** `https://<WEB_APP_NAME>.azurewebsites.net/api/v1/Carreiras`

### 3.1. CREATE (POST)

Cria uma nova Carreira.

| Método | Endpoint | Corpo da Requisição (JSON) |
| :--- | :--- | :--- |
| `POST` | `/api/v1/Carreiras` | Exemplo abaixo |

```json
{
  "nome": "Cientista de Dados Pleno",
  "descricao": "Profissional focado em análise e modelagem de dados para insights de negócio.",
  "area": "Data Science",
  "nivel": 2,
  "competenciasNecessarias": [
    {
      "nome": "Python",
      "tipo": "Hard Skill"
    },
    {
      "nome": "Estatística",
      "tipo": "Hard Skill"
    }
  ]
}
```

### 3.2. READ (GET)

Retorna uma lista paginada de Carreiras ou uma Carreira específica.

| Método | Endpoint | Descrição |
| :--- | :--- | :--- |
| `GET` | `/api/v1/Carreiras?page=1&pageSize=10` | Lista paginada de Carreiras. |
| `GET` | `/api/v1/Carreiras/{id}` | Retorna a Carreira com o ID especificado. |

**Exemplo de Resposta (GET /api/v1/Carreiras/1):**

```json
{
  "id": 1,
  "nome": "Desenvolvedor Backend Junior",
  "descricao": "Desenvolvimento de APIs e serviços em .NET.",
  "area": "Tecnologia",
  "nivel": 1,
  "competenciasNecessarias": [
    {
      "nome": "C#",
      "tipo": "Hard Skill"
    },
    {
      "nome": "Comunicação",
      "tipo": "Soft Skill"
    }
  ],
  "links": {
    "self": "https://<WEB_APP_NAME>.azurewebsites.net/api/v1/Carreiras/1",
    "update": "https://<WEB_APP_NAME>.azurewebsites.net/api/v1/Carreiras/1",
    "delete": "https://<WEB_APP_NAME>.azurewebsites.net/api/v1/Carreiras/1"
  }
}
```

### 3.3. UPDATE (PUT)

Atualiza uma Carreira existente.

| Método | Endpoint | Corpo da Requisição (JSON) |
| :--- | :--- | :--- |
| `PUT` | `/api/v1/Carreiras/{id}` | O corpo deve conter o objeto completo da Carreira, incluindo o `id`. |

```json
{
  "id": 1,
  "nome": "Desenvolvedor Backend Junior",
  "descricao": "Desenvolvimento de APIs e serviços em .NET. Foco em microserviços.",
  "area": "Tecnologia",
  "nivel": 1,
  "competenciasNecessarias": [
    {
      "nome": "C#",
      "tipo": "Hard Skill"
    },
    {
      "nome": "Comunicação",
      "tipo": "Soft Skill"
    }
  ]
}
```

### 3.4. DELETE (DELETE)

Exclui uma Carreira.

| Método | Endpoint |
| :--- | :--- |
| `DELETE` | `/api/v1/Carreiras/{id}` |

## 4. Próximos Passos (Ações Manuais do Usuário)

Para completar o projeto, o usuário deve realizar as seguintes ações no Azure DevOps:

1.  **Criar Projeto e Importar Repositório:** Criar um novo projeto no Azure DevOps e importar o código-fonte deste repositório para o **Azure Repos**.
2.  **Configurar Branch Principal Protegida:** Configurar a branch principal (ex: `main`) com as políticas de proteção:
    *   Revisor obrigatório.
    *   Vinculação de Work Item.
    *   Revisor padrão (seu RM).
3.  **Criar Tarefa no Azure Boards:** Criar uma Tarefa inicial e vincular o primeiro commit/PR a ela.
4.  **Configurar Pipeline de Build:** Criar um novo Pipeline no Azure Pipelines, selecionando a opção **YAML** e apontando para o arquivo `azure-pipeline.yml`.
5.  **Configurar Pipeline de Release:** Criar um Pipeline de Release (Classic ou YAML multi-stage) que:
    *   Use o artefato gerado pelo Pipeline de Build.
    *   Seja acionado automaticamente após um novo artefato.
    *   Faça o deploy no Azure Web App provisionado pelo script `script-infra.sh`.
6.  **Executar Script de Infraestrutura:** Executar o script `scripts/script-infra.sh` no Azure Cloud Shell ou localmente com o Azure CLI logado para provisionar os recursos.
7.  **Executar Script SQL:** Após o deploy, conectar-se ao Azure SQL Database e executar o `script-bd.sql` para criar as tabelas e popular os dados de teste (CRUD).
8.  **Gravação do Vídeo:** Seguir o roteiro de gravação, demonstrando todos os passos (Boards, Repos, Pipelines, execução do CRUD).

## 5. Estrutura de Pastas

```
GS.NET2/
├── CareerMapSolution.sln
├── CareerMap.Recommendations.Api/
├── CareerMap.Recommendations.Domain/
├── CareerMap.Recommendations.Infrastructure/
├── script-bd.sql                 <-- Script SQL para criação do DB (Requisito 11)
├── azure-pipeline.yml            <-- Pipeline de Build CI (Requisito 14)
├── scripts/                      <-- Pasta para scripts de infraestrutura (Requisito 10)
│   └── script-infra.sh           <-- Script de provisionamento Azure CLI (Requisito 12)
└── dockerfiles/                  <-- Pasta para Dockerfiles (Requisito 13)
    └── Dockerfile                <-- Dockerfile para containerização (Requisito 13)
```
