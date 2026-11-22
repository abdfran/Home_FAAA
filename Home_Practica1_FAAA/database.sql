CREATE DATABASE IF NOT EXISTS catalogo_videojuegos;
USE catalogo_videojuegos;

DROP TABLE IF EXISTS Usuarios;

CREATE TABLE Usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre_usuario VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL
);

-- Usuarios de prueba requeridos
INSERT INTO Usuarios (nombre_usuario, password) VALUES ('admin', '1234');
INSERT INTO Usuarios (nombre_usuario, password) VALUES ('franklin', 'pws2026');
INSERT INTO Usuarios (nombre_usuario, password) VALUES ('player1', 'gamer');