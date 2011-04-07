ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [DF_Categories_IsActive] DEFAULT ((1)) FOR [IsActive];

