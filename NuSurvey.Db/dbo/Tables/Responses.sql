CREATE TABLE [dbo].[Responses] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [Value]      VARCHAR (MAX) NOT NULL,
    [Score]      INT           NOT NULL,
    [Order]      INT           NOT NULL,
    [IsActive]   BIT           CONSTRAINT [DF_Responses_IsActive] DEFAULT ((1)) NOT NULL,
    [QuestionId] INT           NOT NULL,
    CONSTRAINT [PK_Responses] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Responses_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id])
);

