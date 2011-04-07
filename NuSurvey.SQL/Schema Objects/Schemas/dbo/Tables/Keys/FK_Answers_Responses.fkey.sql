ALTER TABLE [dbo].[Answers]
    ADD CONSTRAINT [FK_Answers_Responses] FOREIGN KEY ([ResponseId]) REFERENCES [dbo].[Responses] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

