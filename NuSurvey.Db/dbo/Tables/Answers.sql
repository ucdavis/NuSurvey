CREATE TABLE [dbo].[Answers] (
    [id]                    INT          IDENTITY (1, 1) NOT NULL,
    [SurveyResponseId]      INT          NOT NULL,
    [CategoryId]            INT          NOT NULL,
    [QuestionId]            INT          NOT NULL,
    [ResponseId]            INT          NULL,
    [Score]                 INT          NOT NULL,
    [OpenEndedAnswer]       INT          NULL,
    [BypassScore]           BIT          CONSTRAINT [DF_Answers_ByPassScore] DEFAULT ((0)) NOT NULL,
    [OpenEndedStringAnswer] VARCHAR (50) NULL,
    CONSTRAINT [PK_Answers] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Answers_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([id]),
    CONSTRAINT [FK_Answers_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id]),
    CONSTRAINT [FK_Answers_Responses] FOREIGN KEY ([ResponseId]) REFERENCES [dbo].[Responses] ([id]),
    CONSTRAINT [FK_Answers_SurveyResponses] FOREIGN KEY ([SurveyResponseId]) REFERENCES [dbo].[SurveyResponses] ([id])
);

