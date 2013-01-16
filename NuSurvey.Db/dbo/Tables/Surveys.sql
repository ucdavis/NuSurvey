CREATE TABLE [dbo].[Surveys] (
    [id]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (100) NOT NULL,
    [ShortName] NCHAR (10)    NULL,
    [IsActive]  BIT           CONSTRAINT [DF_Surveys_IsActive] DEFAULT ((1)) NOT NULL,
    [QuizType]  VARCHAR (100) CONSTRAINT [DF_Surveys_QuizType] DEFAULT (' ') NOT NULL,
    [OwnerId] VARCHAR(250) NULL, 
    [PhotoId] INT NULL, 
    CONSTRAINT [PK_Surveys] PRIMARY KEY CLUSTERED ([id] ASC)
);

