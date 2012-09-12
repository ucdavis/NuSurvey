CREATE VIEW dbo.vw_MaxScorePerQuestions
AS
SELECT     dbo.Questions.CategoryId, dbo.Questions.id, MAX(dbo.Responses.Score) AS MaxScore
FROM         dbo.Questions INNER JOIN
                      dbo.Responses ON dbo.Questions.id = dbo.Responses.QuestionId
WHERE     (dbo.Questions.IsActive = 1)
GROUP BY dbo.Questions.id, dbo.Questions.CategoryId, dbo.Responses.IsActive
HAVING      (dbo.Responses.IsActive = 1)