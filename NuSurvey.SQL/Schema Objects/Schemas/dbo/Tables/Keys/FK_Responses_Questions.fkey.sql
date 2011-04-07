ALTER TABLE [dbo].[Responses]
    ADD CONSTRAINT [FK_Responses_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

