ALTER TABLE [dbo].[Responses]
    ADD CONSTRAINT [DF_Responses_IsActive] DEFAULT ((1)) FOR [IsActive];

