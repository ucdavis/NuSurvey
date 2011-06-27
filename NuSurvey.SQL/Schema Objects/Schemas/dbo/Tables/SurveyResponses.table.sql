CREATE TABLE [dbo].[SurveyResponses] (
    [id]                  INT           IDENTITY (1, 1) NOT NULL,
    [StudentId]           VARCHAR (10)  NOT NULL,
    [DateTaken]           DATETIME      NOT NULL,
    [PositiveCategoryId]  INT           NULL,
    [NegativeCategoryId1] INT           NULL,
    [NegativeCategoryId2] INT           NULL,
    [SurveyId]            INT           NOT NULL,
    [UserId]              VARCHAR (250) NOT NULL,
    [IsPending]           BIT           NOT NULL
);





