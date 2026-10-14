IF DB_ID('SmartBusinessPro') IS NULL
    CREATE DATABASE SmartBusinessPro;
GO
USE SmartBusinessPro;
GO

CREATE TABLE Categorias (
    CategoriaId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL
);

CREATE TABLE Productos (
    ProductoId  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(150) NOT NULL,
    Precio      DECIMAL(18,2) NOT NULL,
    Existencia  INT NOT NULL,
    CategoriaId INT NOT NULL,
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId)
        REFERENCES Categorias(CategoriaId)
);
GO

INSERT INTO Categorias (Nombre) VALUES (N'Electrónica'), (N'Papelería'), (N'Hogar');

INSERT INTO Productos (Nombre, Precio, Existencia, CategoriaId) VALUES
(N'Laptop 15"',        649.99, 12, 1),
(N'Mouse inalámbrico',  15.50, 80, 1),
(N'Cuaderno profesional', 3.25, 200, 2),
(N'Caja de lapiceros',   5.75, 150, 2),
(N'Lámpara de escritorio', 22.00, 35, 3);
GO
