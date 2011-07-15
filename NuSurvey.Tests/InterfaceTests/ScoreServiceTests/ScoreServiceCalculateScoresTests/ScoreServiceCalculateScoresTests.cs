using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceCalculateScoresTests
{
    [TestClass]
    public class ScoreServiceCalculateScoresTests
    {
        public IScoreService ScoreService;
        public IRepository<CategoryTotalMaxScore> CategoryTotalMaxScoreRepository { get; set; }
        public IRepository Repository { get; set; }

        public ScoreServiceCalculateScoresTests()
        {
            ScoreService = new ScoreService();
            CategoryTotalMaxScoreRepository = MockRepository.GenerateStub<IRepository<CategoryTotalMaxScore>>();
            Repository = MockRepository.GenerateStub<IRepository>();
            Repository.Expect(a => a.OfType<CategoryTotalMaxScore>()).Return(CategoryTotalMaxScoreRepository).Repeat.Any();
            SetupTotalMaxScores();
        }


        [TestMethod]
        public void TestCalculateScoreIgnoredBypassedQuestions()
        {
            #region Arrange
            var surveyResponse = GetSurveyResponse();

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[0]).ElementAt(0).Score = 999;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[1]).ElementAt(0).Score = 998;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[2]).ElementAt(0).Score = 997;

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[3]).ElementAt(0).Score = 1;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[4]).ElementAt(0).Score = 2;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[5]).ElementAt(0).Score = 3;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(0).Score = 0;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(0).BypassScore = true;
            #endregion Arrange

            #region Act
            ScoreService.CalculateScores(Repository, surveyResponse);
            #endregion Act

            #region Assert
            Assert.IsNotNull(surveyResponse.PositiveCategory);
            Assert.AreEqual("Name7", surveyResponse.PositiveCategory.Name);  //Score 0 was bypassed
            Assert.IsNotNull(surveyResponse.NegativeCategory1);
            Assert.AreEqual("Name4", surveyResponse.NegativeCategory1.Name);
            Assert.IsNotNull(surveyResponse.NegativeCategory2);
            Assert.AreEqual("Name5", surveyResponse.NegativeCategory2.Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCalculateScoreNotIgnoredBypassedQuestions() //Same as test above except no ignore
        {
            #region Arrange
            var surveyResponse = GetSurveyResponse();

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[0]).ElementAt(0).Score = 999;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[1]).ElementAt(0).Score = 998;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[2]).ElementAt(0).Score = 997;

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[3]).ElementAt(0).Score = 1;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[4]).ElementAt(0).Score = 2;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[5]).ElementAt(0).Score = 3;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(0).Score = 0;
            //surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(0).BypassScore = true;
            #endregion Arrange

            #region Act
            ScoreService.CalculateScores(Repository, surveyResponse);
            #endregion Act

            #region Assert
            Assert.IsNotNull(surveyResponse.PositiveCategory);
            Assert.AreEqual("Name6", surveyResponse.PositiveCategory.Name);  //Score 0 was bypassed
            Assert.IsNotNull(surveyResponse.NegativeCategory1);
            Assert.AreEqual("Name7", surveyResponse.NegativeCategory1.Name);
            Assert.IsNotNull(surveyResponse.NegativeCategory2);
            Assert.AreEqual("Name4", surveyResponse.NegativeCategory2.Name);
            #endregion Assert
        }

        /// <summary>
        /// Ok, this one skips Name7 because all of the answers have been bypassed
        /// </summary>
        [TestMethod]
        public void TestCalculateScoreIgnoredBypassedQuestionsAllInCategory()
        {
            #region Arrange
            var surveyResponse = GetSurveyResponse();

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[0]).ElementAt(0).Score = 999;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[1]).ElementAt(0).Score = 998;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[2]).ElementAt(0).Score = 997;

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[3]).ElementAt(0).Score = 1;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[4]).ElementAt(0).Score = 2;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[5]).ElementAt(0).Score = 3;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(0).Score = 0;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(0).BypassScore = true;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(1).Score = 0;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(1).BypassScore = true;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(2).Score = 0;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(2).BypassScore = true;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(3).Score = 0;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(3).BypassScore = true;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(4).Score = 0;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[6]).ElementAt(4).BypassScore = true;
            #endregion Arrange

            #region Act
            ScoreService.CalculateScores(Repository, surveyResponse);
            #endregion Act

            #region Assert
            Assert.IsNotNull(surveyResponse.PositiveCategory);
            Assert.AreEqual("Name6", surveyResponse.PositiveCategory.Name);  
            Assert.IsNotNull(surveyResponse.NegativeCategory1);
            Assert.AreEqual("Name4", surveyResponse.NegativeCategory1.Name);
            Assert.IsNotNull(surveyResponse.NegativeCategory2);
            Assert.AreEqual("Name5", surveyResponse.NegativeCategory2.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestCalculateScoreUsesRankWhenScoresAreSame1()
        {
            #region Arrange
            var surveyResponse = GetSurveyResponse();

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[0]).ElementAt(0).Score = 999;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[1]).ElementAt(0).Score = 998;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[2]).ElementAt(0).Score = 997;

            surveyResponse.Survey.Categories[6].Rank = 1;
            surveyResponse.Survey.Categories[5].Rank = 2;
            surveyResponse.Survey.Categories[4].Rank = 3;
            surveyResponse.Survey.Categories[3].Rank = 4;

            #endregion Arrange

            #region Act
            ScoreService.CalculateScores(Repository, surveyResponse);
            #endregion Act

            #region Assert
            Assert.IsNotNull(surveyResponse.PositiveCategory);
            Assert.AreEqual("Name7", surveyResponse.PositiveCategory.Name);  
            Assert.IsNotNull(surveyResponse.NegativeCategory1);
            Assert.AreEqual("Name6", surveyResponse.NegativeCategory1.Name);
            Assert.IsNotNull(surveyResponse.NegativeCategory2);
            Assert.AreEqual("Name5", surveyResponse.NegativeCategory2.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestCalculateScoreUsesRankWhenScoresAreSame2() 
        {
            #region Arrange
            var surveyResponse = GetSurveyResponse();

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[0]).ElementAt(0).Score = 999;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[1]).ElementAt(0).Score = 998;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[2]).ElementAt(0).Score = 997;

            surveyResponse.Survey.Categories[6].Rank = 2;
            surveyResponse.Survey.Categories[5].Rank = 4;
            surveyResponse.Survey.Categories[4].Rank = 1;
            surveyResponse.Survey.Categories[3].Rank = 3;

            #endregion Arrange

            #region Act
            ScoreService.CalculateScores(Repository, surveyResponse);
            #endregion Act

            #region Assert
            Assert.IsNotNull(surveyResponse.PositiveCategory);
            Assert.AreEqual("Name5", surveyResponse.PositiveCategory.Name);
            Assert.IsNotNull(surveyResponse.NegativeCategory1);
            Assert.AreEqual("Name7", surveyResponse.NegativeCategory1.Name);
            Assert.IsNotNull(surveyResponse.NegativeCategory2);
            Assert.AreEqual("Name4", surveyResponse.NegativeCategory2.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestCalculateScoreAllAnswersByPassed()
        {
            #region Arrange
            var surveyResponse = GetSurveyResponse();

            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[0]).ElementAt(0).Score = 999;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[1]).ElementAt(0).Score = 998;
            surveyResponse.Answers.Where(a => a.Category == surveyResponse.Survey.Categories[2]).ElementAt(0).Score = 997;

            foreach (var answer in surveyResponse.Answers)
            {
                answer.Score = 0;
                answer.BypassScore = true;
            }
            #endregion Arrange

            #region Act
            ScoreService.CalculateScores(Repository, surveyResponse);
            #endregion Act

            #region Assert
            Assert.IsNotNull(surveyResponse.PositiveCategory);
            Assert.AreEqual("Name4", surveyResponse.PositiveCategory.Name);
            Assert.IsNotNull(surveyResponse.NegativeCategory1);
            Assert.AreEqual("Name5", surveyResponse.NegativeCategory1.Name);
            Assert.IsNotNull(surveyResponse.NegativeCategory2);
            Assert.AreEqual("Name6", surveyResponse.NegativeCategory2.Name);
            #endregion Assert
        }


        #region Helper Methods
        public void SetupTotalMaxScores()
        {
            var categoryTotalMaxScores = new List<CategoryTotalMaxScore>();
            for (int i = 0; i < 7; i++)
            {
                categoryTotalMaxScores.Add(CreateValidEntities.CategoryTotalMaxScore(i+1));
                categoryTotalMaxScores[i].TotalMaxScore = 25;
            }

            new FakeCategoryTotalMaxScore(0, CategoryTotalMaxScoreRepository, categoryTotalMaxScores);
        }

        public static SurveyResponse GetSurveyResponse()
        {
            var surveyResponse = CreateValidEntities.SurveyResponse(1);
            var answerCount = 0;
            var answers = new List<Answer>();
            var categories = new List<Category>();
            for (int i = 0; i < 8; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].IsActive = true;
                categories[i].Rank = i + 1;
                categories[i].SetIdTo(i + 1);
                for (int j = 0; j < 5; j++)
                {
                    answerCount++;
                    answers.Add(CreateValidEntities.Answer(answerCount));
                    answers[answerCount - 1].Category = categories[i];
                    answers[answerCount - 1].Question.Responses = new List<Response>();
                    answers[answerCount - 1].Question.Responses.Add(CreateValidEntities.Response(1));
                    answers[answerCount - 1].Question.Responses.Add(CreateValidEntities.Response(2));
                    answers[answerCount - 1].Question.Responses[0].Score = 5;
                    answers[answerCount - 1].Question.Responses[1].Score = 6;
                    answers[answerCount - 1].Question.Responses[1].IsActive = false;
                    answers[answerCount - 1].Score = 5;
                }
            }
            categories[0].IsActive = false;
            categories[1].IsCurrentVersion = false;
            categories[2].DoNotUseForCalculations = true;


            surveyResponse.Survey.Categories = categories;

            surveyResponse.Answers = answers;
            return surveyResponse;
        }
        #endregion Helper Methods
    }
}
