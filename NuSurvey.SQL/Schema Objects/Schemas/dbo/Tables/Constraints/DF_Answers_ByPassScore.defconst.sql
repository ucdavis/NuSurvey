ALTER TABLE [dbo].[Answers]
    ADD CONSTRAINT [DF_Answers_ByPassScore] DEFAULT ((0)) FOR [BypassScore];

