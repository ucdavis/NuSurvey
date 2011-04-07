CREATE TABLE [dbo].[Categories] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (100) NOT NULL,
    [Rank]          INT           NOT NULL,
    [Affirmation]   VARCHAR (MAX) NOT NULL,
    [Encouragement] VARCHAR (MAX) NOT NULL,
    [SurveyId]      INT           NOT NULL,
    [IsActive]      BIT           NOT NULL,
    [LastUpdate]    DATETIME      NOT NULL
);

