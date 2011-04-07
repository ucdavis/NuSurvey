CREATE TABLE [dbo].[Answers] (
    [id]               INT IDENTITY (1, 1) NOT NULL,
    [SurveyResponseId] INT NOT NULL,
    [CategoryId]       INT NOT NULL,
    [QuestionId]       INT NOT NULL,
    [ResponseId]       INT NOT NULL,
    [Score]            INT NOT NULL
);

