ALTER TABLE [dbo].[Categories]
    ADD CONSTRAINT [DF_Categories_UseForCalculations] DEFAULT ((0)) FOR [DoNotUseForCalculations];

