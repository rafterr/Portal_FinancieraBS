-- Script de verificación y creación de base de datos
-- Sistema de Gestión Financiera

-- Crear base de datos si no existe
CREATE DATABASE IF NOT EXISTS financiera_bs;
USE financiera_bs;

-- Tabla de Usuarios (Identity)
CREATE TABLE IF NOT EXISTS AspNetUsers (
    Id VARCHAR(255) PRIMARY KEY,
    UserName VARCHAR(256),
    NormalizedUserName VARCHAR(256),
    Email VARCHAR(256),
    NormalizedEmail VARCHAR(256),
    EmailConfirmed BOOLEAN,
    PasswordHash TEXT,
    SecurityStamp TEXT,
    ConcurrencyStamp TEXT,
    PhoneNumber VARCHAR(50),
    PhoneNumberConfirmed BOOLEAN,
    TwoFactorEnabled BOOLEAN,
    LockoutEnd DATETIME(6),
    LockoutEnabled BOOLEAN,
    AccessFailedCount INT,
    FechaCreacion DATETIME(6) DEFAULT CURRENT_TIMESTAMP(6)
);

-- Tabla de Clientes
CREATE TABLE IF NOT EXISTS Clientes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    Direccion VARCHAR(255) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PagarePath VARCHAR(500),
    InePath VARCHAR(500),
    ComprobanteDomicilioPath VARCHAR(500),
    Estatus VARCHAR(20) NOT NULL DEFAULT 'Activo',
    UsuarioId VARCHAR(255),
    FOREIGN KEY (UsuarioId) REFERENCES AspNetUsers(Id) ON DELETE SET NULL
);

-- Tabla de Préstamos
CREATE TABLE IF NOT EXISTS Prestamos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MontoSolicitado DECIMAL(18,2) NOT NULL,
    SaldoRestante DECIMAL(18,2) NOT NULL,
    FechaInicio DATETIME(6) NOT NULL,
    Interes DECIMAL(5,2) NOT NULL,
    Estatus INT NOT NULL DEFAULT 2,
    ClienteId INT NOT NULL,
    UsuarioId VARCHAR(255),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE,
    FOREIGN KEY (UsuarioId) REFERENCES AspNetUsers(Id) ON DELETE SET NULL
);

-- Tabla de Pagos
CREATE TABLE IF NOT EXISTS Pagos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FechaPago DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    MontoPago DECIMAL(18,2) NOT NULL,
    PrestamoId INT NOT NULL,
    ClienteId INT NOT NULL,
    UsuarioId VARCHAR(255),
    FOREIGN KEY (PrestamoId) REFERENCES Prestamos(Id) ON DELETE CASCADE,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE,
    FOREIGN KEY (UsuarioId) REFERENCES AspNetUsers(Id) ON DELETE SET NULL
);

-- Índices para mejorar rendimiento
CREATE INDEX idx_clientes_usuario ON Clientes(UsuarioId);
CREATE INDEX idx_clientes_estatus ON Clientes(Estatus);
CREATE INDEX idx_prestamos_cliente ON Prestamos(ClienteId);
CREATE INDEX idx_prestamos_usuario ON Prestamos(UsuarioId);
CREATE INDEX idx_prestamos_estatus ON Prestamos(Estatus);
CREATE INDEX idx_pagos_prestamo ON Pagos(PrestamoId);
CREATE INDEX idx_pagos_cliente ON Pagos(ClienteId);
CREATE INDEX idx_pagos_fecha ON Pagos(FechaPago);

-- Verificar tablas creadas
SHOW TABLES;

-- Mostrar estructura de cada tabla
DESCRIBE AspNetUsers;
DESCRIBE Clientes;
DESCRIBE Prestamos;
DESCRIBE Pagos;

SELECT 'Base de datos configurada correctamente' AS Mensaje;
