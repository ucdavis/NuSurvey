ALTER TABLE [dbo].[Answers]
    ADD CONSTRAINT [FK_Answers_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

