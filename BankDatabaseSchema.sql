-- 1. Create Database if it does not exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BankSystemDB')
BEGIN
    CREATE DATABASE BankSystemDB;
END
GO

USE BankSystemDB;
GO

-- 2. Create Customers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE Customers (
        CustomerID INT PRIMARY KEY IDENTITY(1,1),
        FullName NVARCHAR(100) NOT NULL,
        NationalID VARCHAR(20) NOT NULL UNIQUE,
        PhoneNumber VARCHAR(15),
        Email VARCHAR(100) UNIQUE,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

-- 3. Create Accounts Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Accounts]') AND type in (N'U'))
BEGIN
    CREATE TABLE Accounts (
        AccountID INT PRIMARY KEY IDENTITY(1001,1),
        CustomerID INT NOT NULL,
        AccountType NVARCHAR(20) CHECK (AccountType IN ('Savings', 'Checking')),
        Balance DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_Accounts_Customers FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE CASCADE
    );
END
GO

-- 4. Create Transactions Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND type in (N'U'))
BEGIN
    CREATE TABLE Transactions (
        TransactionID INT PRIMARY KEY IDENTITY(1,1),
        AccountID INT NOT NULL,
        TransactionType NVARCHAR(20) CHECK (TransactionType IN ('Deposit', 'Withdrawal', 'Transfer')),
        Amount DECIMAL(18, 2) NOT NULL,
        TransactionDate DATETIME DEFAULT GETDATE(),
        Details NVARCHAR(255),
        CONSTRAINT FK_Transactions_Accounts FOREIGN KEY (AccountID) REFERENCES Accounts(AccountID)
    );
END
GO

-- 5. Create Employees Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND type in (N'U'))
BEGIN
    CREATE TABLE Employees (
        EmployeeID INT PRIMARY KEY IDENTITY(1,1),
        FullName NVARCHAR(100) NOT NULL,
        Username VARCHAR(50) UNIQUE NOT NULL,
        PasswordHash VARCHAR(255) NOT NULL,
        Role NVARCHAR(20) CHECK (Role IN ('Admin', 'Teller', 'Manager')) DEFAULT 'Teller',
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

-- 6. Stored Procedure: Transfer Money
CREATE OR ALTER PROCEDURE sp_TransferMoney
    @FromAccountID INT,
    @ToAccountID INT,
    @Amount DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Amount <= 0
    BEGIN
        RAISERROR('Transfer amount must be greater than zero.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @SenderBalance DECIMAL(18, 2);
        SELECT @SenderBalance = Balance FROM Accounts WHERE AccountID = @FromAccountID AND IsActive = 1;

        IF @SenderBalance IS NULL OR @SenderBalance < @Amount
        BEGIN
            RAISERROR('Insufficient balance or account is inactive.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        UPDATE Accounts 
        SET Balance = Balance - @Amount 
        WHERE AccountID = @FromAccountID;

        UPDATE Accounts 
        SET Balance = Balance + @Amount 
        WHERE AccountID = @ToAccountID;

        INSERT INTO Transactions (AccountID, TransactionType, Amount, Details)
        VALUES (@FromAccountID, 'Transfer', @Amount, CONCAT('Transfer to Account ID: ', @ToAccountID));

        INSERT INTO Transactions (AccountID, TransactionType, Amount, Details)
        VALUES (@ToAccountID, 'Transfer', @Amount, CONCAT('Transfer received from Account ID: ', @FromAccountID));

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 7. Seed Initial English Test Data
IF NOT EXISTS (SELECT 1 FROM Employees)
BEGIN
    INSERT INTO Employees (FullName, Username, PasswordHash, Role)
    VALUES 
        ('Ahmed Al-Mahmoud', 'admin_ahmed', 'hashed_pass_123', 'Admin'),
        ('Raghad Ali', 'teller_raghad', 'hashed_pass_456', 'Teller');
END

IF NOT EXISTS (SELECT 1 FROM Customers)
BEGIN
    INSERT INTO Customers (FullName, NationalID, PhoneNumber, Email)
    VALUES 
        ('Sara Khalid', '9981029384', '0791234567', 'sara@example.com'),
        ('Omar Abdullah', '9952019283', '0788765432', 'omar@example.com');
END

IF NOT EXISTS (SELECT 1 FROM Accounts)
BEGIN
    INSERT INTO Accounts (CustomerID, AccountType, Balance)
    VALUES 
        (1, 'Checking', 1500.00), -- AccountID: 1001
        (2, 'Savings', 500.00);    -- AccountID: 1002
END

IF NOT EXISTS (SELECT 1 FROM Transactions)
BEGIN
    INSERT INTO Transactions (AccountID, TransactionType, Amount, Details)
    VALUES 
        (1001, 'Deposit', 1500.00, 'Initial deposit on account opening');
END
GO

-- 8. Verification Queries
SELECT Table_Name FROM INFORMATION_SCHEMA.TABLES WHERE Table_Type = 'BASE TABLE';
SELECT * FROM Customers;
SELECT * FROM Employees;
SELECT * FROM Accounts;
SELECT * FROM Transactions;