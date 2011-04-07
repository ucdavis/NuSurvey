ALTER TABLE [dbo].[SurveyResponses]
    ADD CONSTRAINT [FK_SurveyResponses_Surveys] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

