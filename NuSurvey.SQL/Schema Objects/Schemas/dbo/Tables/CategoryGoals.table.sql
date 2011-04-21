CREATE TABLE [dbo].[CategoryGoals] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (200) NOT NULL,
    [IsActive]   BIT           NOT NULL,
    [CategoryId] INT           NOT NULL
);



