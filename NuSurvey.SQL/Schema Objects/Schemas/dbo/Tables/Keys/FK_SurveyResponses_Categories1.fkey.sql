ALTER TABLE [dbo].[SurveyResponses]
    ADD CONSTRAINT [FK_SurveyResponses_Categories1] FOREIGN KEY ([NegativeCategoryId1]) REFERENCES [dbo].[Categories] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

