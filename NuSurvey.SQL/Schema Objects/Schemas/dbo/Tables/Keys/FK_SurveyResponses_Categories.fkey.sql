ALTER TABLE [dbo].[SurveyResponses]
    ADD CONSTRAINT [FK_SurveyResponses_Categories] FOREIGN KEY ([PositiveCategoryId]) REFERENCES [dbo].[Categories] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

