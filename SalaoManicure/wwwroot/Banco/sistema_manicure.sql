CREATE DATABASE IF NOT EXISTS sistema_manicure
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE sistema_manicure;


-- =========================================
-- TABELA: Clientes
-- =========================================

CREATE TABLE Clientes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(150) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    Telefone VARCHAR(20),
    DataCadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE Clientes ADD COLUMN UsuarioId INT NULL;
ALTER TABLE Clientes MODIFY COLUMN UsuarioId INT NOT NULL;
ALTER TABLE Clientes ADD CONSTRAINT FK_Clientes_Usuarios FOREIGN KEY (UsuarioId) REFERENCES usuarios(Id);
ALTER TABLE Clientes ADD CONSTRAINT UQ_Clientes_UsuarioId UNIQUE (UsuarioId);

ALTER TABLE Disponibilidades
DROP FOREIGN KEY FK_Disponibilidades_Profissionais_ProfissionalId;
-- =========================================
-- TABELA: Profissionais
-- =========================================

CREATE TABLE Profissionais (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(150) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    Telefone VARCHAR(20),
    Ativo BOOLEAN NOT NULL DEFAULT TRUE
);


-- =========================================
-- TABELA: Servicos
-- =========================================

CREATE TABLE Servicos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(150) NOT NULL,
    Descricao VARCHAR(500),
    Preco DECIMAL(10,2) NOT NULL,
    DuracaoMinutos INT NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE
);


-- =========================================
-- TABELA: Usuarios
-- =========================================

/*CREATE TABLE Usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(150) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    SenhaHash VARCHAR(255) NOT NULL,
    Perfil VARCHAR(50) NOT NULL DEFAULT 'Profissional',
    Ativo BOOLEAN NOT NULL DEFAULT TRUE
);
*/
drop table if exists usuarios;
CREATE TABLE usuarios ( 
Id INT NOT NULL AUTO_INCREMENT, 
Nome VARCHAR(150) NOT NULL, 
Email VARCHAR(150) NOT NULL, 
SenhaHash VARCHAR(255) NOT NULL, 
Perfil VARCHAR(30) NOT NULL, 
Ativo TINYINT(1) NOT NULL, 
dataCadastro DATETIME NOT NULL, 
PRIMARY KEY (Id), UNIQUE KEY uq_usuarios_email (email)
 );
select * from Usuarios;
INSERT INTO usuarios ( Nome, Email, SenhaHash, Perfil, Ativo, DataCadastro ) 
VALUES ( 'Nilson Teste', 'teste@teste.com', '$2a$11$7EqJtq98hPqEX7fNZaFWoO5x5K5xK5xK5xK5xK5xK5xK5xK5xK5', 'Cliente', 1, NOW() );

-- =========================================
-- TABELA: Agendamentos
-- =========================================

CREATE TABLE Agendamentos (
    Id INT AUTO_INCREMENT PRIMARY KEY,

    Data DATE NOT NULL,

    Horario TIME NOT NULL,

    Status VARCHAR(30) NOT NULL DEFAULT 'Agendado',

    ClienteId INT NOT NULL,

    ProfissionalId INT NOT NULL,

    ServicoId INT NOT NULL,

    CONSTRAINT FK_Agendamento_Cliente
        FOREIGN KEY (ClienteId)
        REFERENCES Clientes(Id),

    CONSTRAINT FK_Agendamento_Profissional
        FOREIGN KEY (ProfissionalId)
        REFERENCES Profissionais(Id),

    CONSTRAINT FK_Agendamento_Servico
        FOREIGN KEY (ServicoId)
        REFERENCES Servicos(Id)
);
DROP TABLE IF EXISTS Disponibilidades;

CREATE TABLE Disponibilidades (
    Id INT NOT NULL AUTO_INCREMENT,
    Data DATE NOT NULL,
    Horario TIME NOT NULL,
    Disponivel BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_Disponibilidade_Data_Horario (Data, Horario)
);
DESCRIBE Disponibilidades;
INSERT INTO Disponibilidades
    (Data, Horario, Disponivel)
VALUES
    ('2026-10-05', '09:00:00', TRUE),
    ('2026-10-05', '09:30:00', TRUE),
    ('2026-10-05', '10:00:00', TRUE),
    ('2026-10-05', '10:30:00', TRUE),
    ('2026-10-05', '11:00:00', TRUE),
    ('2026-10-05', '11:30:00', TRUE),

    ('2026-10-06', '09:00:00', TRUE),
    ('2026-10-06', '09:30:00', TRUE),
    ('2026-10-06', '10:00:00', TRUE),
    ('2026-10-06', '10:30:00', TRUE),

    ('2026-10-07', '13:00:00', TRUE),
    ('2026-10-07', '13:30:00', TRUE),
    ('2026-10-07', '14:00:00', TRUE),
    ('2026-10-07', '14:30:00', TRUE);
SELECT Id, Data, Horario, Disponivel
FROM Disponibilidades
WHERE Disponivel = 1
ORDER BY Data, Horario
/*
CREATE TABLE Disponibilidades (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProfissionalId INT NOT NULL,
    DiaSemana INT NOT NULL,
    Horario TIME NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT FK_Disponibilidade_Profissional
        FOREIGN KEY (ProfissionalId)
        REFERENCES Profissionais(Id)
);
*/

select * from usuario;

DESCRIBE Disponibilidades;
CREATE TABLE Bloqueios (
    Id INT AUTO_INCREMENT PRIMARY KEY,

    ProfissionalId INT NOT NULL,

    Data DATE NOT NULL,

    Horario TIME NULL,

    Motivo VARCHAR(255),

    Ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT FK_Bloqueio_Profissional
        FOREIGN KEY (ProfissionalId)
        REFERENCES Profissionais(Id)
);

INSERT INTO Disponibilidades
    (ProfissionalId, DiaSemana, Horario)
VALUES
    (1, 2, '09:00:00'),
    (1, 2, '10:00:00'),
    (1, 2, '11:00:00'),
    (1, 2, '13:00:00'),
    (1, 2, '14:00:00'),
    (1, 2, '15:00:00');
    
INSERT INTO Bloqueios
    (ProfissionalId, Data, Horario, Motivo)
VALUES
    (1, '2026-10-12', NULL, 'Feriado');
    
INSERT INTO Bloqueios
    (ProfissionalId, Data, Horario, Motivo)
VALUES
    (1, '2026-10-15', '14:00:00', 'Compromisso pessoal');
ALTER TABLE Disponibilidades
ADD CONSTRAINT UK_Disponibilidade
UNIQUE (ProfissionalId, DiaSemana, Horario);

ALTER TABLE Bloqueios
ADD CONSTRAINT UK_Bloqueio
UNIQUE (ProfissionalId, Data, Horario);
    
INSERT INTO Clientes
    (Nome, Email, Telefone)
VALUES
    ('Maria Silva', 'maria@gmail.com', '11999999999'),
    ('Ana Souza', 'ana@gmail.com', '11988888888'),
    ('Juliana Santos', 'juliana@gmail.com', '11977777777');
    
    INSERT INTO Profissionais
    (Nome, Email, Telefone)
VALUES
    ('Carla Manicure', 'carla@gmail.com', '11966666666'),
    ('Fernanda Oliveira', 'fernanda@gmail.com', '11955555555');
    
    INSERT INTO Servicos
    (Nome, Descricao, Preco, DuracaoMinutos)
VALUES
    ('Manicure', 'Manicure tradicional', 30.00, 60),

    ('Pedicure', 'Pedicure tradicional', 35.00, 60),

    ('Manicure + Pedicure', 'Serviço completo', 60.00, 120),

    ('Alongamento de Unhas', 'Alongamento de unhas em gel', 120.00, 150);
    
    INSERT INTO Usuarios
    (Nome, Email, SenhaHash, Perfil)
VALUES
    ('Administrador', 'admin@manicure.com', '123456', 'Administrador');
    
    INSERT INTO Agendamentos
    (Data, Horario, Status, ClienteId, ProfissionalId, ServicoId)
VALUES
    ('2026-10-05', '09:00:00', 'Agendado', 1, 1, 1);
    
    SELECT
    a.Id,
    a.Data,
    a.Horario,
    a.Status,
    c.Nome AS Cliente,
    p.Nome AS Profissional,
    s.Nome AS Servico,
    s.Preco,
    s.DuracaoMinutos
FROM Agendamentos a
INNER JOIN Clientes c
    ON a.ClienteId = c.Id
INNER JOIN Profissionais p
    ON a.ProfissionalId = p.Id
INNER JOIN Servicos s
    ON a.ServicoId = s.Id;