CREATE TABLE [dbo].[Categories] (
    [id]                      INT           IDENTITY (1, 1) NOT NULL,
    [Name]                    VARCHAR (100) NOT NULL,
    [Rank]                    INT           NOT NULL,
    [Affirmation]             VARCHAR (MAX) NOT NULL,
    [Encouragement]           VARCHAR (MAX) NOT NULL,
    [SurveyId]                INT           NOT NULL,
    [IsActive]                BIT           CONSTRAINT [DF_Categories_IsActive] DEFAULT ((1)) NOT NULL,
    [LastUpdate]              DATETIME      CONSTRAINT [DF_Categories_LastUpdate] DEFAULT (getdate()) NOT NULL,
    [CreateDate]              DATETIME      CONSTRAINT [DF_Categories_CreateDate] DEFAULT (getdate()) NOT NULL,
    [DoNotUseForCalculations] BIT           CONSTRAINT [DF_Categories_UseForCalculations] DEFAULT ((0)) NOT NULL,
    [IsCurrentVersion]        BIT           CONSTRAINT [DF_Categories_IsCurrentVersion] DEFAULT ((1)) NOT NULL,
    [PreviousVersion]         INT           NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Categories_Surveys] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([id])
);

