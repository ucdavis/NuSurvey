ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [DF_Categories_CreateDate] DEFAULT (getdate()) FOR [CreateDate];

