CREATE DATABASE OrderServiceDB;
GO
USE OrderServiceDB;
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderTables')
BEGIN
    CREATE TABLE OrderTables (
        OrderID INT PRIMARY KEY IDENTITY(1,1),
        OrderDate DATETIME,
        TotalAmount DECIMAL(10, 2),
        CustomerID INT,
        DeliveryAddressID INT,
        DeletedAt DATETIME
    );
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        OrderItemID INT PRIMARY KEY IDENTITY(1,1),
        Quantity INT,
        Price DECIMAL(10, 2),
        OrderID INT FOREIGN KEY REFERENCES OrderTables (OrderID),
        ProductID INT,
        DeletedAt DATETIME
    );
END
GO

-- Insert data into OrderTables
-- Assuming the `OrderDate` is the current date for simplicity
-- Insert data into OrderTables only if it's empty
-- By using the IF NOT EXISTS we can make the script idempotent, by making sure there won't be inserted duplicates
-- whenever it gets deployed.
IF NOT EXISTS (SELECT 1 FROM OrderTables)
BEGIN
    INSERT INTO OrderTables (OrderDate, TotalAmount, CustomerID, DeliveryAddressID, DeletedAt) VALUES
    (GETDATE(), 1000.00, 1, 1, null),
    (GETDATE(), 50.00, 2, 2, null),
    (GETDATE(), 23.99, 3, 3, null),
    (GETDATE(), 450.00, 4, 4, null),
    (GETDATE(), 29.99, 5, 5, null),
    (GETDATE(), 89.99, 6, 6, null),
    (GETDATE(), 15.99, 7, 7, null),
    (GETDATE(), 99.99, 8, 8, null),
    (GETDATE(), 3.99, 9, 9, null),
    (GETDATE(), 500.00, 10, 10, null);
END

-- Insert data into OrderItems
-- Assuming one product per order item for simplicity
-- Insert data into OrderItems only if it's empty
-- By using the IF NOT EXISTS we can make the script idempotent, by making sure there won't be inserted duplicates
-- whenever it gets deployed.
IF NOT EXISTS (SELECT 1 FROM OrderItems)
BEGIN
    INSERT INTO OrderItems (Quantity, Price, OrderID, ProductID, DeletedAt) VALUES
    (1, 999.99, 1, 1, null),
    (3, 19.99, 2, 2, null),
    (5, 9.99, 3, 3, null),
    (1, 499.99, 4, 4, null),
    (2, 29.99, 5, 5, null),
    (2, 39.99, 6, 6, null),
    (10, 12.99, 7, 7, null),
    (4, 79.99, 8, 8, null),
    (100, 2.99, 9, 9, null),
    (10, 19.99, 10, 10, null);
END