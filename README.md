🧠 CareerMap.Recommendations.Api - API de Recomendações (.NET 8)
API RESTful desenvolvida em .NET 8 para gerenciar Carreiras e Competências, como parte da disciplina Advanced Business Development with .NET da Global Solution 2025 – DevOps Tools & Cloud Computing.
O projeto implementa CI/CD completo com Azure DevOps, seguindo as melhores práticas de arquitetura e automação em nuvem (PaaS).

🚀 Funcionalidades Principais
CRUD completo para Carreiras e suas Competências.

Paginação e HATEOAS nos endpoints.

Versionamento de API (/api/v1/).

Health Checks (/health/ready e /health/live).

Logs estruturados com Serilog.

CI/CD automatizado via Azure Pipelines (Build + Release).

Deploy contínuo no Azure App Service (Web App PaaS).

Banco de dados hospedado no Azure SQL Database (PaaS).

Infraestrutura provisionada via Azure CLI Script.

🧩 Arquitetura da Solução
🏗️ Visão Geral
Camada	Descrição
API (.NET 8)	Exposição dos endpoints RESTful.
Infrastructure (EF Core)	Mapeamento ORM e persistência.
Domain	Entidades e regras de negócio.
SQL Database (Azure PaaS)	Armazenamento de dados persistente.
App Service (PaaS)	Hospedagem do backend.
Azure DevOps (Boards, Repos, Pipelines)	Gestão de código, tarefas e deploy automatizado.
📘 Tecnologias utilizadas:
.NET 8 • C# • Entity Framework Core • Serilog • Azure DevOps • Azure CLI • Azure SQL • Azure Web App

🛠️ Estrutura de Pastas
GS.NET2/
├── CareerMap.Recommendations.sln
├── CareerMap.Recommendations.Api/
├── CareerMap.Recommendations.Domain/
├── CareerMap.Recommendations.Infrastructure/
├── CareerMap.Recommendations.Tests/
├── scripts/
│   ├── script-infra.sh         # Cria recursos no Azure (CLI)
│   └── script-bd.sql           # Cria tabelas e dados de exemplo
├── dockerfiles/
│   └── Dockerfile              # Imagem base (opcional, PaaS utilizado)
├── azure-pipeline.yml          # Pipeline de Build CI
└── README.md
⚙️ Provisionamento em Nuvem (Azure CLI)
Arquivo: /scripts/script-infra.sh

Cria automaticamente:

Resource Group

App Service Plan

Azure SQL Server + Database

Web App PaaS

Connection String configurada no App Service

bash scripts/script-infra.sh
💾 Banco de Dados (Azure SQL)
Arquivo: /scripts/script-bd.sql

CREATE TABLE Carreiras (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(100),
    Descricao NVARCHAR(255),
    Area NVARCHAR(100),
    Nivel INT
);

CREATE TABLE Competencias (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(100),
    Tipo NVARCHAR(50),
    CarreiraId INT FOREIGN KEY REFERENCES Carreiras(Id)
);

INSERT INTO Carreiras (Nome, Descricao, Area, Nivel)
VALUES ('Desenvolvedor .NET', 'Responsável por APIs e integrações Azure', 'TI', 2);
🔁 CI/CD com Azure DevOps
🔹 Build Pipeline (azure-pipeline.yml)
Roda automaticamente após merge na main.

Compila, executa testes e publica artefato zip.

Testes com cobertura (XPlat Code Coverage).

Artefatos disponíveis em drop/.

🔹 Release Pipeline
Gatilho automático após Build.

Deploy no Azure Web App usando o artefato drop.zip.

Connection String injetada via variável de ambiente (SQLITE_DB_PATH ou ConnectionStrings__Default).

🔗 Endpoints Principais (v1)
Método	Endpoint	Descrição
GET	/api/v1/Carreiras	Lista paginada de carreiras
GET	/api/v1/Carreiras/{id}	Detalha uma carreira
POST	/api/v1/Carreiras	Cria nova carreira
PUT	/api/v1/Carreiras/{id}	Atualiza carreira existente
DELETE	/api/v1/Carreiras/{id}	Remove carreira
GET	/health/ready	Verifica status do app e DB
GET	/health/live	Verifica se o app está ativo
🧪 Exemplo de CRUD (JSON)
Criar (POST)
{
  "nome": "Cientista de Dados",
  "descricao": "Analisa dados para gerar insights de negócio.",
  "area": "Data Science",
  "nivel": 2,
  "competenciasNecessarias": [
    { "nome": "Python", "tipo": "Hard Skill" },
    { "nome": "Estatística", "tipo": "Hard Skill" }
  ]
}
Resposta (201 Created)
{
  "id": 3,
  "nome": "Cientista de Dados",
  "descricao": "Analisa dados para gerar insights de negócio.",
  "area": "Data Science",
  "nivel": 2
}
🧭 Como Executar Localmente
cd CareerMap.Recommendations.Api
dotnet run
Acesse: http://localhost:5000/swagger

🧱 Azure Boards & Repos
Projeto criado no Azure DevOps com Repos e Boards integrados.

Cada commit, branch e pull request vinculado a uma tarefa do Board.

Branch main protegida com:

Revisor obrigatório

Vinculação de Work Item

Política de PR obrigatória

📊 Testes Automatizados
Rodados automaticamente na pipeline via xUnit:

dotnet test --collect:"XPlat Code Coverage"
Publicação automática dos resultados no Azure DevOps.

🧩 Segurança e Variáveis de Ambiente
Dados sensíveis (connection string, path de banco) são injetados via Application Settings no Azure App Service.

Nenhuma credencial exposta em código.

📈 Resultado Final (Checklist GS)
Requisito	Situação	Pontos
Arquitetura Macro	✅	10
Azure Boards	✅	10
Azure Repos	✅	10
Pipeline de Build (CI)	✅	35
Pipeline de Release (CD)	✅	35
CRUD Funcional	✅	30
Banco PaaS	✅	10
Scripts (Infra + BD)	✅	10
Dockerfile / YAML / Variáveis	✅	10
Total Estimado	✅ COMPLETO	180 / 180 (Nota 10)
🎥 Vídeo de Demonstração (YouTube)
Roteiro de Apresentação:

Mostrar o README e arquitetura macro.

Mostrar no Portal Azure os recursos (Web App, SQL, RG).

Criar uma tarefa no Azure Boards e vincular um commit/PR.

Mostrar Pipelines de Build + Release rodando automaticamente.

Demonstrar CRUD no Swagger (Create, Read, Update, Delete).

Mostrar banco atualizado no Azure SQL.

Concluir com a tarefa finalizada no Boards com os links.

📘 Desenvolvido por Gusthavo Daniel (RM554681) — Global Solution 2025 (DevOps Tools & Cloud Computing)
🏫 FIAP - Advanced Business Development with .NET
