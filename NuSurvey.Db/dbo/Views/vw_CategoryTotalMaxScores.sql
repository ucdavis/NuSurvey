CREATE VIEW [dbo].[vw_CategoryTotalMaxScores]
AS
SELECT     dbo.Categories.id, dbo.Categories.Name, SUM(dbo.vw_MaxScorePerQuestions.MaxScore) AS TotalMaxScore
FROM         dbo.Categories INNER JOIN
                      dbo.vw_MaxScorePerQuestions ON dbo.Categories.id = dbo.vw_MaxScorePerQuestions.CategoryId
GROUP BY dbo.Categories.id, dbo.Categories.Name, dbo.Categories.DoNotUseForCalculations
HAVING      (dbo.Categories.DoNotUseForCalculations = 0)