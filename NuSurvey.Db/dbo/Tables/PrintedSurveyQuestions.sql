CREATE TABLE [dbo].[PrintedSurveyQuestions]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [PrintedSurveyId] INT NOT NULL, 
    [QuestionId] INT NOT NULL, 
    [PhotoId] INT NOT NULL, 
    [Order] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_PrintedSurveyQuestions_PrintedSurveys] FOREIGN KEY ([PrintedSurveyId]) REFERENCES [PrintedSurveys]([Id]), 
    CONSTRAINT [FK_PrintedSurveyQuestions_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [Questions]([Id]), 
    CONSTRAINT [FK_PrintedSurveyQuestions_Photos] FOREIGN KEY ([PhotoId]) REFERENCES [Photos]([Id])
)
