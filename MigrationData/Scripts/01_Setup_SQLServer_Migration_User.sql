USE master;
GO

-- Check SQL Server authentication mode.
-- LoginMode 2 means Mixed Mode is enabled.

DECLARE @LoginMode INT;

EXEC xp_instance_regread
    N'HKEY_LOCAL_MACHINE',
    N'Software\Microsoft\MSSQLServer\MSSQLServer',
    N'LoginMode',
    @LoginMode OUTPUT;

SELECT 
    @LoginMode AS LoginMode,
    CASE 
        WHEN @LoginMode = 1 THEN 'Windows Authentication only'
        WHEN @LoginMode = 2 THEN 'Mixed Mode: SQL Server and Windows Authentication'
        ELSE 'Unknown'
    END AS LoginModeDescription;
GO

-- Create SQL login for migration if it does not already exist.

IF NOT EXISTS (
    SELECT 1
    FROM sys.sql_logins
    WHERE name = 'migration_user'
)
BEGIN
    CREATE LOGIN migration_user
    WITH PASSWORD = '<REPLACE_WITH_SECURE_PASSWORD>',
         CHECK_POLICY = OFF,
         CHECK_EXPIRATION = OFF;
END
GO

USE MusicStore;
GO

-- Create database user in MusicStore if it does not already exist.

IF NOT EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = 'migration_user'
)
BEGIN
    CREATE USER migration_user FOR LOGIN migration_user;
END
GO

-- Grant read-only access for extraction.

ALTER ROLE db_datareader ADD MEMBER migration_user;
GO

SELECT 
    'migration_user setup complete' AS StatusMessage;
GO