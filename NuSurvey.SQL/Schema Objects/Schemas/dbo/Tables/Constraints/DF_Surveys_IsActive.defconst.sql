ALTER TABLE [dbo].[Surveys]
    ADD CONSTRAINT [DF_Surveys_IsActive] DEFAULT ((1)) FOR [IsActive];

