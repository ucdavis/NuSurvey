ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [FK_Categories_Surveys] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

