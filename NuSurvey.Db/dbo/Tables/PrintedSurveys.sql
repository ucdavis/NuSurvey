CREATE TABLE [dbo].[PrintedSurveys]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [UserId] VARCHAR(50) NOT NULL, 
    [DateCreated] DATETIME NOT NULL DEFAULT getdate(), 
    [SurveyId] INT NOT NULL, 
    [Name] VARCHAR(250) NULL, 
    CONSTRAINT [FK_PrintedSurveys_Surveys] FOREIGN KEY ([SurveyId]) REFERENCES [Surveys]([Id])
)
