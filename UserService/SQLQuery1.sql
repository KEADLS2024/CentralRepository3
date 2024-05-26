BEGIN
    CREATE DATABASE UserServiceDB;
END;
GO

-- Use the database
USE UserServiceDB;
GO

-- Create Addresses Table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Addresses (
        AddressID INT IDENTITY(1,1) PRIMARY KEY,
        Street NVARCHAR(255),
        City NVARCHAR(255),
        PostalCode NVARCHAR(20),
        Country NVARCHAR(255)
    );
END;
GO

-- Create Customers Table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Customers (
        CustomerID INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(255),
        LastName NVARCHAR(255),
        Email NVARCHAR(255) UNIQUE NOT NULL,
        Phone NVARCHAR(20),
        AddressID INT,
        UserID INT,
        FOREIGN KEY (AddressID) REFERENCES dbo.Addresses(AddressID)
    );
END;
GO

-- Insert sample data into the Addresses table
INSERT INTO dbo.Addresses (Street, City, PostalCode, Country)
VALUES 
('123 Maple Street', 'Springfield', '12345', 'USA'),
('456 Oak Avenue', 'Shelbyville', '67890', 'USA'),
('789 Pine Road', 'Capital City', '10112', 'USA');
GO

-- Insert sample data into the Customers table
INSERT INTO dbo.Customers (FirstName, LastName, Email, Phone, AddressID, UserID)
VALUES 
('John', 'Doe', 'john.doe@example.com', '123-456-7890', 1, 101),
('Jane', 'Smith', 'jane.smith@example.com', '234-567-8901', 2, 102),
('Alice', 'Johnson', 'alice.johnson@example.com', '345-678-9012', 3, 103),
('Desi', 'Khaana', 'desi_khaana@outlook.com', '456-789-0123', 1, 104);
GO