ALTER TABLE [dbo].[Answers]
    ADD CONSTRAINT [FK_Answers_SurveyResponses] FOREIGN KEY ([SurveyResponseId]) REFERENCES [dbo].[SurveyResponses] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

