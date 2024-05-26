-- Opret en ny database, hvis den ikke allerede findes
CREATE DATABASE UserServiceDB;

-- Skift til den nye database
USE UserServiceDB;

-- Opret Address tabel
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Addresses') THEN
    CREATE TABLE Addresses (
        AddressId INT AUTO_INCREMENT PRIMARY KEY,
        Street NVARCHAR(50) NOT NULL,
        City NVARCHAR(50) NOT NULL,
        PostalCode NVARCHAR(10) NOT NULL,
        Country NVARCHAR(56) NOT NULL
    );
END IF;

-- Opret Customer tabel
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers') THEN
    CREATE TABLE Customers (
        CustomerId INT AUTO_INCREMENT PRIMARY KEY,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        Phone NVARCHAR(20) NOT NULL,
        AddressId INT NOT NULL,
        UserId INT NOT NULL,
        FOREIGN KEY (AddressId) REFERENCES Addresses(AddressId)
    );
END IF;

-- Indsæt nogle eksemplerækker i Address
INSERT INTO Addresses (Street, City, PostalCode, Country) VALUES
('123 Main St', 'Anytown', '12345', 'USA'),
('456 Maple Ave', 'Othertown', '67890', 'Canada');

-- Indsæt nogle eksemplerækker i Customer
INSERT INTO Customers (FirstName, LastName, Email, Phone, AddressId, UserId) VALUES
('Alice', 'Smith', 'alice.smith@example.com', '12345678', 1, 1),
('Bob', 'Johnson', 'bob.johnson@example.com', '98765432', 2, 2);
