ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [DF_Categories_IsCurrentVersion] DEFAULT ((1)) FOR [IsCurrentVersion];

