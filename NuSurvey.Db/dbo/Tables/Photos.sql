CREATE TABLE [dbo].[Photos]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(100) NOT NULL, 
	[Filename] varchar(100) not null,
    [ContentType] VARCHAR(50) NOT NULL, 
    [FileContents] VARBINARY(MAX) NOT NULL, 
    [DateCreated] DATETIME NOT NULL DEFAULT getdate(), 
    [ThumbNail] VARBINARY(MAX) NOT NULL
)
