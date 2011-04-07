ALTER TABLE [dbo].[CategoryGoals]
    ADD CONSTRAINT [DF_CategoryGoals_IsActive] DEFAULT ((1)) FOR [IsActive];

