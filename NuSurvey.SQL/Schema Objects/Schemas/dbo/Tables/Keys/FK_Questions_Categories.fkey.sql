ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [FK_Questions_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

