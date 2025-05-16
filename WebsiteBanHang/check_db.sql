-- Simple script to check database connection and server status
SELECT @@VERSION AS SqlServerVersion;
GO

-- Check if the database exists
IF DB_ID('MetacriticDB') IS NOT NULL
BEGIN
    PRINT 'Database MetacriticDB exists.';
    
    -- Try to use the database
    USE MetacriticDB;
    
    -- Check tables (if any)
    SELECT 
        TABLE_SCHEMA,
        TABLE_NAME
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_TYPE = 'BASE TABLE'
    ORDER BY TABLE_SCHEMA, TABLE_NAME;
END
ELSE
BEGIN
    PRINT 'Database MetacriticDB does not exist.';
END
GO 