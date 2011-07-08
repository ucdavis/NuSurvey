using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace NuSurvey.Tests.InterfaceTests
{
    [TestClass]
    public class ScoreServiceTests
    {
        public IScoreService ScoreService;
        public IRepository<Question> QuestionRepository;

        public ScoreServiceTests()
        {
            ScoreService = new ScoreService();
            QuestionRepository = MockRepository.GenerateStub<IRepository<Question>>();
        }

        #region Score Question Tests
        #region Non-Open Ended Tests

        [TestMethod]
        public void TestWhenQuestionHasNoAnswer()
        {
            #region Arrange
            SetupQuestions();
            var qAndA = new QuestionAnswerParameter {QuestionId = 1, ResponseId = 0};
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer is required", result.Message);
            Assert.AreEqual(0, result.Score);
            #endregion Assert		
        }

        [TestMethod]
        public void TestWhenQuestionHasWrongAnswer()
        {
            #region Arrange
            SetupQuestions();
            var qAndA = new QuestionAnswerParameter {QuestionId = 1, ResponseId = 2};
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer is required", result.Message);
            Assert.AreEqual(0, result.Score);
            #endregion Assert
        }


        #endregion Non-Open Ended Tests
        #endregion Score Question Tests


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Do ScoreService Tests");

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }



        private void SetupQuestions()
        {
            var questions = new List<Question>();
            for (int i = 0; i < 5; i++)
            {
                questions.Add(CreateValidEntities.Question(i+1));
            }

            questions[0].Name = "RadionButtons";            
            for (int i = 0; i < 3; i++)
            {
                questions[0].AddResponse(CreateValidEntities.Response(i+1));
                questions[0].Responses[i].SetIdTo(i + 1);
            }
            questions[0].Responses[1].IsActive = false;
            questions[0].IsOpenEnded = false;

            new FakeQuestions(0, QuestionRepository, questions);
        }

    }
}
