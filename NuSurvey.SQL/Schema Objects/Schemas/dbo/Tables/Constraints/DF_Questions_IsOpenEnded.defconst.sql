ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [DF_Questions_IsOpenEnded] DEFAULT ((0)) FOR [IsOpenEnded];

