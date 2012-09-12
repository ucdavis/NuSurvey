CREATE TABLE [dbo].[CategoryGoals] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (200) NOT NULL,
    [IsActive]   BIT           CONSTRAINT [DF_CategoryGoals_IsActive] DEFAULT ((1)) NOT NULL,
    [CategoryId] INT           NOT NULL,
    CONSTRAINT [PK_CategoryGoals] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_CategoryGoals_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([id])
);

