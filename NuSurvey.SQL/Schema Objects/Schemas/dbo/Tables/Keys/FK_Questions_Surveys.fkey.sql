ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [FK_Questions_Surveys] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

