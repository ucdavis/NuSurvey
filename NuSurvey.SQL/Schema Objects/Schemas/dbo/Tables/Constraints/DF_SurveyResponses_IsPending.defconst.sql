ALTER TABLE [dbo].[SurveyResponses]
    ADD CONSTRAINT [DF_SurveyResponses_IsPending] DEFAULT ((0)) FOR [IsPending];

