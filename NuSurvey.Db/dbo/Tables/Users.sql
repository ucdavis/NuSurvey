CREATE TABLE [dbo].[Users]
(
	[Id] VARCHAR(256) NOT NULL PRIMARY KEY, 
    [FirstName] VARCHAR(100) NOT NULL, 
    [LastName] VARCHAR(100) NOT NULL, 
    [Title] VARCHAR(100) NULL, 
    [Agency] VARCHAR(250) NULL, 
    [City] VARCHAR(100) NOT NULL, 
    [State] VARCHAR(50) NOT NULL
)
