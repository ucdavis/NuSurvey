ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [DF_Categories_LastUpdate] DEFAULT (getdate()) FOR [LastUpdate];

