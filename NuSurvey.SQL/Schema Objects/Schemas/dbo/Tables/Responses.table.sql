CREATE TABLE [dbo].[Responses] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [Value]      VARCHAR (MAX) NOT NULL,
    [Score]      INT           NOT NULL,
    [Order]      INT           NOT NULL,
    [IsActive]   BIT           NOT NULL,
    [QuestionId] INT           NOT NULL
);

