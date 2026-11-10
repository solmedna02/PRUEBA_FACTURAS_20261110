-- TABLA CLIENTES
CREATE TABLE Clientes (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    Cedula VARCHAR(13) NOT NULL UNIQUE,
    Correo VARCHAR(150) NOT NULL
);


-- TABLA USUARIO
CREATE TABLE Usuario (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Correo VARCHAR(150) NOT NULL UNIQUE,
    Contrasena VARCHAR(255) NOT NULL,
    Rol VARCHAR(20) NOT NULL
        CHECK (Rol IN ('Administrador', 'Ejecutivo'))
);


-- TABLA FACTURAS
CREATE TABLE Facturas (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    FechaEmision DATETIME2 NOT NULL,
    FechaVencer DATETIME2 NOT NULL,
    EstadoPago BIT NOT NULL DEFAULT 0,
    Monto DECIMAL(18,2) NOT NULL,

    CONSTRAINT FK_Facturas_Clientes
        FOREIGN KEY (IdCliente)
        REFERENCES Clientes(ID),

    CONSTRAINT CK_Facturas_Monto
        CHECK (Monto > 0),

    CONSTRAINT CK_Facturas_Fechas
        CHECK (FechaVencer >= FechaEmision)
);


-- TABLA PAGOS
CREATE TABLE Pagos (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    IdFactura INT NOT NULL,
    MontoPagado DECIMAL(18,2) NOT NULL,
    FechaPago DATETIME2 NOT NULL,

    CONSTRAINT FK_Pagos_Facturas
        FOREIGN KEY (IdFactura)
        REFERENCES Facturas(ID),

    CONSTRAINT CK_Pagos_Monto
        CHECK (MontoPagado > 0)
);