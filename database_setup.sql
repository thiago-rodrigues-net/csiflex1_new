-- Script de criação da tabela de usuários para CSIFLEX
-- Compatible com MySQL 5.7+

-- Criar database se não existir
CREATE DATABASE IF NOT EXISTS `csi_auth` 
DEFAULT CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `csi_auth`;

-- Criar tabela de usuários
CREATE TABLE IF NOT EXISTS `users` (
  `username_` VARCHAR(50) NOT NULL,
  `password_` TEXT NOT NULL,
  `salt_` TEXT NOT NULL,
  `firstname_` VARCHAR(100) DEFAULT '',
  `Name_` VARCHAR(100) DEFAULT '',
  `displayname` VARCHAR(100) DEFAULT '',
  `email_` VARCHAR(100) DEFAULT '',
  `usertype` VARCHAR(20) DEFAULT 'user',
  `refId` VARCHAR(50) DEFAULT '',
  `title` VARCHAR(100) DEFAULT '',
  `dept` VARCHAR(100) DEFAULT '',
  `machines` TEXT,
  `phoneext` VARCHAR(20) DEFAULT '',
  `EditTimeline` BOOLEAN DEFAULT FALSE,
  `EditMasterPartData` BOOLEAN DEFAULT FALSE,
  PRIMARY KEY (`username_`),
  INDEX `idx_usertype` (`usertype`),
  INDEX `idx_email` (`email_`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Criar usuário admin padrão
-- Senha: admin123 (ALTERE APÓS O PRIMEIRO LOGIN!)
-- Hash gerado com PBKDF2-SHA256, 10000 iterações
INSERT INTO `users` (
  `username_`,
  `password_`,
  `salt_`,
  `firstname_`,
  `Name_`,
  `displayname`,
  `email_`,
  `usertype`,
  `EditTimeline`,
  `EditMasterPartData`
) VALUES (
  'admin',
  'YourHashHere',  -- Será gerado pelo sistema
  'YourSaltHere',  -- Será gerado pelo sistema
  'Administrator',
  'System',
  'Administrator',
  'admin@csiflex.com',
  'admin',
  TRUE,
  TRUE
) ON DUPLICATE KEY UPDATE 
  `usertype` = 'admin';

-- Comentários sobre a estrutura
-- username_: Nome de usuário único (chave primária)
-- password_: Hash PBKDF2 da senha em Base64
-- salt_: Salt único do usuário em Base64
-- firstname_: Primeiro nome
-- Name_: Sobrenome (mantido nome original por compatibilidade)
-- displayname: Nome de exibição
-- email_: E-mail do usuário
-- usertype: Tipo de usuário (admin, user, programer)
-- refId: ID de referência externa
-- title: Cargo
-- dept: Departamento
-- machines: Lista de máquinas separadas por vírgula (ou "ALL")
-- phoneext: Ramal telefônico
-- EditTimeline: Permissão para editar timeline
-- EditMasterPartData: Permissão para editar dados de peças

-- Verificar se a tabela foi criada corretamente
SELECT 'Tabela users criada com sucesso!' AS status;
DESCRIBE `users`;
