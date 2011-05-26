ALTER TABLE [dbo].[Surveys]
    ADD CONSTRAINT [DF_Surveys_QuizType] DEFAULT (' ') FOR [QuizType];

