ALTER TABLE [dbo].[CategoryGoals]
    ADD CONSTRAINT [FK_CategoryGoals_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

