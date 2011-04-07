ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [DF_Questions_IsActive] DEFAULT ((1)) FOR [IsActive];

