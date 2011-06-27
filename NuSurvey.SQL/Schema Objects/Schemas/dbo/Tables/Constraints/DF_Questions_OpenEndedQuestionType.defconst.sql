ALTER TABLE [dbo].[Questions]
    ADD CONSTRAINT [DF_Questions_OpenEndedQuestionType] DEFAULT ((0)) FOR [OpenEndedQuestionType];

