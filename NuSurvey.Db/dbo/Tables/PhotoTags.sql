CREATE TABLE [dbo].[PhotoTags]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(100) NOT NULL, 
    [PhotoId] INT NOT NULL, 
    CONSTRAINT [FK_PhotoTags_Photos] FOREIGN KEY ([PhotoId]) REFERENCES [Photos]([Id])
)
