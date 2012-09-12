CREATE TABLE [dbo].[Questions] (
    [id]                    INT           IDENTITY (1, 1) NOT NULL,
    [Name]                  VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Questions_IsActive] DEFAULT ((1)) NOT NULL,
    [CategoryId]            INT           NOT NULL,
    [SurveyId]              INT           NOT NULL,
    [Order]                 INT           NOT NULL,
    [IsOpenEnded]           BIT           CONSTRAINT [DF_Questions_IsOpenEnded] DEFAULT ((0)) NOT NULL,
    [CreateDate]            DATETIME      CONSTRAINT [DF_Questions_CreateDate] DEFAULT (getdate()) NOT NULL,
    [OpenEndedQuestionType] INT           CONSTRAINT [DF_Questions_OpenEndedQuestionType] DEFAULT ((0)) NOT NULL,
    [AllowBypass]           BIT           CONSTRAINT [DF_Questions_AllowBypass] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Questions_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([id]),
    CONSTRAINT [FK_Questions_Surveys] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([id])
);

