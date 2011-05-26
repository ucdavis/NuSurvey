CREATE TABLE [dbo].[Surveys] (
    [id]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (100) NOT NULL,
    [ShortName] NCHAR (10)    NULL,
    [IsActive]  BIT           NOT NULL,
    [QuizType]  VARCHAR (100) NOT NULL
);





