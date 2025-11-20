-- Arquivo: script-bd.sql
-- Script SQL para criação das tabelas e inserção de dados de exemplo.

-- Requisito: CRUD em pelo menos duas tabelas.
-- Tabelas baseadas nas entidades Carreira e Competencia.

-- 1. Tabela Competencia
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Competencias' and xtype='U')
CREATE TABLE Competencias (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(100) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL -- Ex: 'Hard Skill', 'Soft Skill'
);

-- 2. Tabela Carreira
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Carreiras' and xtype='U')
CREATE TABLE Carreiras (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(100) NOT NULL,
    Descricao NVARCHAR(500) NOT NULL,
    Area NVARCHAR(50) NOT NULL,
    Nivel INT NOT NULL -- Ex: 1 (Junior), 2 (Pleno), 3 (Senior)
);

-- 3. Tabela de Relacionamento (Carreira - Competencia)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CarreiraCompetencia' and xtype='U')
CREATE TABLE CarreiraCompetencia (
    CarreiraId INT NOT NULL,
    CompetenciaId INT NOT NULL,
    PRIMARY KEY (CarreiraId, CompetenciaId),
    FOREIGN KEY (CarreiraId) REFERENCES Carreiras(Id),
    FOREIGN KEY (CompetenciaId) REFERENCES Competencias(Id)
);

-- Inserção de Dados (CREATE)
-- Competencias
INSERT INTO Competencias (Nome, Tipo) VALUES ('C#', 'Hard Skill'); -- Id 1
INSERT INTO Competencias (Nome, Tipo) VALUES ('.NET Core', 'Hard Skill'); -- Id 2
INSERT INTO Competencias (Nome, Tipo) VALUES ('Azure DevOps', 'Hard Skill'); -- Id 3
INSERT INTO Competencias (Nome, Tipo) VALUES ('Comunicação', 'Soft Skill'); -- Id 4
INSERT INTO Competencias (Nome, Tipo) VALUES ('Resolução de Problemas', 'Soft Skill'); -- Id 5

-- Carreiras
INSERT INTO Carreiras (Nome, Descricao, Area, Nivel) VALUES ('Desenvolvedor Backend Junior', 'Desenvolvimento de APIs e serviços em .NET.', 'Tecnologia', 1); -- Id 1
INSERT INTO Carreiras (Nome, Descricao, Area, Nivel) VALUES ('Engenheiro DevOps Pleno', 'Implementação e manutenção de pipelines CI/CD no Azure.', 'DevOps', 2); -- Id 2

-- Relacionamento
INSERT INTO CarreiraCompetencia (CarreiraId, CompetenciaId) VALUES (1, 1); -- Dev Jr precisa de C#
INSERT INTO CarreiraCompetencia (CarreiraId, CompetenciaId) VALUES (1, 4); -- Dev Jr precisa de Comunicação
INSERT INTO CarreiraCompetencia (CarreiraId, CompetenciaId) VALUES (2, 2); -- Eng DevOps precisa de .NET Core
INSERT INTO CarreiraCompetencia (CarreiraId, CompetenciaId) VALUES (2, 3); -- Eng DevOps precisa de Azure DevOps
INSERT INTO CarreiraCompetencia (CarreiraId, CompetenciaId) VALUES (2, 5); -- Eng DevOps precisa de Resolução de Problemas

-- Exemplo de Consulta (READ)
SELECT * FROM Carreiras;
SELECT * FROM Competencias;

-- Exemplo de Atualização (UPDATE)
UPDATE Carreiras SET Nivel = 3 WHERE Nome = 'Engenheiro DevOps Pleno';

-- Exemplo de Deleção (DELETE)
-- DELETE FROM CarreiraCompetencia WHERE CarreiraId = 1;
-- DELETE FROM Carreiras WHERE Id = 1;
