CREATE TABLE [dbo].[SurveyResponses] (
    [id]                  INT           IDENTITY (1, 1) NOT NULL,
    [StudentId]           VARCHAR (50)  NOT NULL,
    [DateTaken]           DATETIME      CONSTRAINT [DF_SurveyResponses_DateTaken] DEFAULT (getdate()) NOT NULL,
    [PositiveCategoryId]  INT           NULL,
    [NegativeCategoryId1] INT           NULL,
    [NegativeCategoryId2] INT           NULL,
    [SurveyId]            INT           NOT NULL,
    [UserId]              VARCHAR (250) NOT NULL,
    [IsPending]           BIT           CONSTRAINT [DF_SurveyResponses_IsPending] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SurveyResponses] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_SurveyResponses_Categories] FOREIGN KEY ([PositiveCategoryId]) REFERENCES [dbo].[Categories] ([id]),
    CONSTRAINT [FK_SurveyResponses_Categories1] FOREIGN KEY ([NegativeCategoryId1]) REFERENCES [dbo].[Categories] ([id]),
    CONSTRAINT [FK_SurveyResponses_Categories2] FOREIGN KEY ([NegativeCategoryId2]) REFERENCES [dbo].[Categories] ([id]),
    CONSTRAINT [FK_SurveyResponses_Surveys] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([id])
);

