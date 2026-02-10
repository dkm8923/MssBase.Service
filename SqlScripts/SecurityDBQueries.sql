--CREATE DATABASE SecurityDB;

USE SecurityDB;
GO

DROP TABLE IF EXISTS UserPermission;
DROP TABLE IF EXISTS RolePermission;
DROP TABLE IF EXISTS ApplicationUser;
DROP TABLE IF EXISTS Role;
DROP TABLE IF EXISTS Permission;
DROP TABLE IF EXISTS Application;
GO

-- Application table (created first as it's referenced by other tables)
CREATE TABLE Application (
    ApplicationId INT IDENTITY(1,1) PRIMARY KEY,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(64) NOT NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(64) NULL,
    Active BIT NOT NULL,
    Name VARCHAR(64) NOT NULL UNIQUE,
    Description VARCHAR(256) NULL
);
GO

-- ApplicationUser table
CREATE TABLE ApplicationUser (
    ApplicationUserId INT IDENTITY(1,1) PRIMARY KEY,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(64) NOT NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(64) NULL,
    Active BIT NOT NULL,
    Email VARCHAR(128) NOT NULL,
    FirstName VARCHAR(64) NULL,
    LastName VARCHAR(64) NULL,
    DateOfBirth DATETIME NULL,
    Password VARCHAR(64) NULL,
    LastLoginDate DATETIME NULL,
    LastPasswordChangeDate DATETIME NULL,
    LastLockoutDate DATETIME NULL,
    FailedPasswordAttemptCount SMALLINT NULL,
    ApplicationId INT NOT NULL,
    CONSTRAINT FK_ApplicationUser_Application FOREIGN KEY (ApplicationId) 
        REFERENCES Application(ApplicationId)
);
GO

-- Permission table
CREATE TABLE Permission (
    PermissionId INT IDENTITY(1,1) PRIMARY KEY,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(64) NOT NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(64) NULL,
    Active BIT NOT NULL,
    Name VARCHAR(64) NOT NULL UNIQUE,
    Description VARCHAR(256) NULL,
    ApplicationId INT NOT NULL,
    CONSTRAINT FK_Permission_Application FOREIGN KEY (ApplicationId) 
        REFERENCES Application(ApplicationId)
);
GO

-- Role table
CREATE TABLE Role (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(64) NOT NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(64) NULL,
    Active BIT NOT NULL,
    Name VARCHAR(64) NOT NULL UNIQUE,
    Description VARCHAR(256) NULL,
    ApplicationId INT NOT NULL,
    CONSTRAINT FK_Role_Application FOREIGN KEY (ApplicationId) 
        REFERENCES Application(ApplicationId)
);
GO

-- RolePermission table
CREATE TABLE RolePermission (
    RolePermissionId INT IDENTITY(1,1) PRIMARY KEY,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(64) NOT NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(64) NULL,
    Active BIT NOT NULL,
    ApplicationId INT NOT NULL,
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    CONSTRAINT FK_RolePermission_Application FOREIGN KEY (ApplicationId) 
        REFERENCES Application(ApplicationId),
    CONSTRAINT FK_RolePermission_Role FOREIGN KEY (RoleId) 
        REFERENCES Role(RoleId),
    CONSTRAINT FK_RolePermission_Permission FOREIGN KEY (PermissionId) 
        REFERENCES Permission(PermissionId)
);
GO

-- UserPermission table (now references ApplicationUserId)
CREATE TABLE UserPermission (
    UserPermissionId INT IDENTITY(1,1) PRIMARY KEY,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(64) NOT NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(64) NULL,
    Active BIT NOT NULL,
    ApplicationId INT NOT NULL,
    ApplicationUserId INT NOT NULL,
    PermissionId INT NOT NULL,
    CONSTRAINT FK_UserPermission_Application FOREIGN KEY (ApplicationId) 
        REFERENCES Application(ApplicationId),
    CONSTRAINT FK_UserPermission_ApplicationUser FOREIGN KEY (ApplicationUserId) 
        REFERENCES ApplicationUser(ApplicationUserId),
    CONSTRAINT FK_UserPermission_Permission FOREIGN KEY (PermissionId) 
        REFERENCES Permission(PermissionId)
);
GO