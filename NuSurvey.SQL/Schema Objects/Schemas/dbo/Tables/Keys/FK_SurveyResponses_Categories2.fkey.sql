ALTER TABLE [dbo].[SurveyResponses]
    ADD CONSTRAINT [FK_SurveyResponses_Categories2] FOREIGN KEY ([NegativeCategoryId2]) REFERENCES [dbo].[Categories] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

