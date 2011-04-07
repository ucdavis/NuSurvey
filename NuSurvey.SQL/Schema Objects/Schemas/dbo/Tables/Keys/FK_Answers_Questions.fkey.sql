ALTER TABLE [dbo].[Answers]
    ADD CONSTRAINT [FK_Answers_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

