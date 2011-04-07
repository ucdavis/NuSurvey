CREATE TABLE [dbo].[Questions] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (100) NOT NULL,
    [IsActive]    BIT           NOT NULL,
    [CategoryId]  INT           NOT NULL,
    [SurveyId]    INT           NOT NULL,
    [Order]       INT           NOT NULL,
    [IsOpenEnded] BIT           NOT NULL
);

