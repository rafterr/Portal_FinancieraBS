-- Script para crear todas las tablas de Identity en financiera_bs
USE financiera_bs;

-- Eliminar tablas existentes si hay conflictos
DROP TABLE IF EXISTS AspNetUserTokens;
DROP TABLE IF EXISTS AspNetUserRoles;
DROP TABLE IF EXISTS AspNetUserLogins;
DROP TABLE IF EXISTS AspNetUserClaims;
DROP TABLE IF EXISTS AspNetRoleClaims;
DROP TABLE IF EXISTS AspNetRoles;
DROP TABLE IF EXISTS Pagos;
DROP TABLE IF EXISTS Prestamos;
DROP TABLE IF EXISTS Clientes;
DROP TABLE IF EXISTS AspNetUsers;

-- Tabla AspNetRoles
CREATE TABLE AspNetRoles (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Name VARCHAR(256),
    NormalizedName VARCHAR(256),
    ConcurrencyStamp TEXT
);

-- Tabla AspNetUsers
CREATE TABLE AspNetUsers (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    UserName VARCHAR(256),
    NormalizedUserName VARCHAR(256),
    Email VARCHAR(256),
    NormalizedEmail VARCHAR(256),
    EmailConfirmed TINYINT(1) NOT NULL DEFAULT 0,
    PasswordHash TEXT,
    SecurityStamp TEXT,
    ConcurrencyStamp TEXT,
    PhoneNumber VARCHAR(50),
    PhoneNumberConfirmed TINYINT(1) NOT NULL DEFAULT 0,
    TwoFactorEnabled TINYINT(1) NOT NULL DEFAULT 0,
    LockoutEnd DATETIME(6),
    LockoutEnabled TINYINT(1) NOT NULL DEFAULT 0,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6)
);

-- Tabla AspNetUserClaims
CREATE TABLE AspNetUserClaims (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Tabla AspNetUserLogins
CREATE TABLE AspNetUserLogins (
    LoginProvider VARCHAR(255) NOT NULL,
    ProviderKey VARCHAR(255) NOT NULL,
    ProviderDisplayName TEXT,
    UserId VARCHAR(255) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Tabla AspNetUserRoles
CREATE TABLE AspNetUserRoles (
    UserId VARCHAR(255) NOT NULL,
    RoleId VARCHAR(255) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

-- Tabla AspNetUserTokens
CREATE TABLE AspNetUserTokens (
    UserId VARCHAR(255) NOT NULL,
    LoginProvider VARCHAR(255) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Value TEXT,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Tabla AspNetRoleClaims
CREATE TABLE AspNetRoleClaims (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    RoleId VARCHAR(255) NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

-- Tabla Clientes
CREATE TABLE Clientes (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
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

-- Tabla Prestamos
CREATE TABLE Prestamos (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
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

-- Tabla Pagos
CREATE TABLE Pagos (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    FechaPago DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    MontoPago DECIMAL(18,2) NOT NULL,
    PrestamoId INT NOT NULL,
    ClienteId INT NOT NULL,
    UsuarioId VARCHAR(255),
    FOREIGN KEY (PrestamoId) REFERENCES Prestamos(Id) ON DELETE CASCADE,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE,
    FOREIGN KEY (UsuarioId) REFERENCES AspNetUsers(Id) ON DELETE SET NULL
);

-- Índices para AspNetUsers
CREATE INDEX IX_AspNetUsers_NormalizedUserName ON AspNetUsers(NormalizedUserName);
CREATE INDEX IX_AspNetUsers_NormalizedEmail ON AspNetUsers(NormalizedEmail);

-- Índices para AspNetRoles
CREATE INDEX IX_AspNetRoles_NormalizedName ON AspNetRoles(NormalizedName);

-- Índices para AspNetUserClaims
CREATE INDEX IX_AspNetUserClaims_UserId ON AspNetUserClaims(UserId);

-- Índices para AspNetUserLogins
CREATE INDEX IX_AspNetUserLogins_UserId ON AspNetUserLogins(UserId);

-- Índices para AspNetUserRoles
CREATE INDEX IX_AspNetUserRoles_RoleId ON AspNetUserRoles(RoleId);

-- Índices para AspNetRoleClaims
CREATE INDEX IX_AspNetRoleClaims_RoleId ON AspNetRoleClaims(RoleId);

-- Índices para Clientes
CREATE INDEX IX_Clientes_UsuarioId ON Clientes(UsuarioId);
CREATE INDEX IX_Clientes_Estatus ON Clientes(Estatus);

-- Índices para Prestamos
CREATE INDEX IX_Prestamos_ClienteId ON Prestamos(ClienteId);
CREATE INDEX IX_Prestamos_UsuarioId ON Prestamos(UsuarioId);
CREATE INDEX IX_Prestamos_Estatus ON Prestamos(Estatus);

-- Índices para Pagos
CREATE INDEX IX_Pagos_PrestamoId ON Pagos(PrestamoId);
CREATE INDEX IX_Pagos_ClienteId ON Pagos(ClienteId);
CREATE INDEX IX_Pagos_UsuarioId ON Pagos(UsuarioId);
CREATE INDEX IX_Pagos_FechaPago ON Pagos(FechaPago);

SELECT 'Tablas de Identity creadas correctamente' AS Mensaje;
