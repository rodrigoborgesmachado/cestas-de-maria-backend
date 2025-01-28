-- Check if the database exists before creating it
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CESTAS-MARIA')
BEGIN
    CREATE DATABASE [CESTAS-MARIA];
    PRINT 'Database created successfully!';
END
ELSE
BEGIN
    PRINT 'Database already exists.';
END
GO

-- Switch to the new database
USE [CESTAS-MARIA];
GO

-- Check if the login already exists before creating it
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'cestas')
BEGIN
    -- Create the 'cestas' login
    CREATE LOGIN cestas WITH PASSWORD = '}u+LhDa-Tc_%26V=';
    PRINT 'Login created successfully!';
END
ELSE
BEGIN
    PRINT 'Login already exists.';
END
GO

-- Check if the user already exists before creating it
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'cestas')
BEGIN
    -- Create the 'cestas' user
    CREATE USER cestas FOR LOGIN cestas;
    PRINT 'User created successfully!';
    
    -- Grant 'cestas' user the 'db_owner' role in the CESTAS-MARIA database
    EXEC sp_addrolemember 'db_owner', 'cestas';
    PRINT 'User granted db_owner role.';
END
ELSE
BEGIN
    PRINT 'User already exists.';
END
GO
