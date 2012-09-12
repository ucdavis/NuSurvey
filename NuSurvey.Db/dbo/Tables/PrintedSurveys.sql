CREATE TABLE [dbo].[PrintedSurveys]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [UserId] VARCHAR(50) NOT NULL, 
    [DateCreated] DATETIME NOT NULL DEFAULT getdate(), 
    [SurveyId] INT NOT NULL, 
    CONSTRAINT [FK_PrintedSurveys_Surveys] FOREIGN KEY ([SurveyId]) REFERENCES [Surveys]([Id])
)
