-- Creates all databases referenced across appsettings*.json files for dev and integration test environments.
IF DB_ID('CommonDb') IS NULL
    CREATE DATABASE [CommonDb];
GO

IF DB_ID('CommonDb_UT') IS NULL
    CREATE DATABASE [CommonDb_UT];
GO

IF DB_ID('SecurityDB_DEV') IS NULL
    CREATE DATABASE [SecurityDB_DEV];
GO

IF DB_ID('SecurityDB_UT') IS NULL
    CREATE DATABASE [SecurityDB_UT];
GO
