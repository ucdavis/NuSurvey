CREATE TABLE [dbo].[QuestionsXPhotos]
(
    [QuestionId] INT NOT NULL, 
    [PhotoId] INT NOT NULL, 
    CONSTRAINT [PK_QuestionsXPhotos] PRIMARY KEY ([QuestionId], [PhotoId]), 
    CONSTRAINT [FK_QuestionsXPhotos_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [Questions]([Id]), 
    CONSTRAINT [FK_QuestionsXPhotos_Photos] FOREIGN KEY ([PhotoId]) REFERENCES [Photos]([Id])
)
