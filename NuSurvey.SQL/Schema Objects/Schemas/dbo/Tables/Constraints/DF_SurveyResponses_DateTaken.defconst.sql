ALTER TABLE [dbo].[SurveyResponses]
    ADD CONSTRAINT [DF_SurveyResponses_DateTaken] DEFAULT (getdate()) FOR [DateTaken];

