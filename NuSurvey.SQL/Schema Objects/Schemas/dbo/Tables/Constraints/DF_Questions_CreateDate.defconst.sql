ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [DF_Questions_CreateDate] DEFAULT (getdate()) FOR [CreateDate];

