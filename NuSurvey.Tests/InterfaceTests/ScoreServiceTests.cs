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

        [TestMethod]
        public void TestWhenQuestionHasCorrectAnswer1()
        {
            #region Arrange
            SetupQuestions();
            var qAndA = new QuestionAnswerParameter { QuestionId = 1, ResponseId = 3 };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Invalid);
            Assert.AreEqual(string.Empty, result.Message);
            Assert.AreEqual(3, result.Score);
            #endregion Assert
        }

        [TestMethod]
        public void TestWhenQuestionHasCorrectAnswer2()
        {
            #region Arrange
            SetupQuestions();
            var qAndA = new QuestionAnswerParameter { QuestionId = 1, ResponseId = 4 };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Invalid);
            Assert.AreEqual(string.Empty, result.Message);
            Assert.AreEqual(9, result.Score);
            #endregion Assert
        }

        [TestMethod]
        public void TestWhenQuestionHasCorrectAnswer3()
        {
            #region Arrange
            SetupQuestions();
            var qAndA = new QuestionAnswerParameter { QuestionId = 2, ResponseId = 4 };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Invalid);
            Assert.AreEqual(string.Empty, result.Message);
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


        #region Helper Methods
        private void SetupQuestions()
        {
            var questions = new List<Question>();
            for (int i = 0; i < 5; i++)
            {
                questions.Add(CreateValidEntities.Question(i + 1));
            }

            #region Question 1 (Non-Open Ended)
            questions[0].Name = "RadionButtons";
            for (int i = 0; i < 4; i++)
            {
                questions[0].AddResponse(CreateValidEntities.Response(i + 1));
                questions[0].Responses[i].SetIdTo(i + 1);
            }
            questions[0].Responses[1].IsActive = false;
            questions[0].Responses[3].Score = 9;
            questions[0].IsOpenEnded = false;
            #endregion Question 1 (Non-Open Ended)

            #region Question 2 (Non-Open Ended and Not Scoreing)
            questions[1].Name = "RadionButtons";
            for (int i = 0; i < 4; i++)
            {
                questions[1].AddResponse(CreateValidEntities.Response(i + 1));
                questions[1].Responses[i].SetIdTo(i + 1);
            }
            questions[1].Responses[1].IsActive = false;
            questions[1].Responses[3].Score = 9;
            questions[1].IsOpenEnded = false;
            questions[1].Category.DoNotUseForCalculations = true;
            #endregion Question 2 (Non-Open Ended and Not Scoreing)

            #region Question 3 (Open Ended Whole Number)
            questions[2].Name = "Whole Number";
            foreach (var response in WholeNumberResponses())
            {
                questions[2].AddResponse(response);
            }
            questions[2].IsOpenEnded = true;
            questions[2].OpenEndedQuestionType = (int)QuestionType.WholeNumber;
            #endregion Question 3 (Open Ended Whole Number)

            new FakeQuestions(0, QuestionRepository, questions);
        }       


        private static IList<Response> WholeNumberResponses()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 7; i++)
            {
                responses.Add(CreateValidEntities.Response(i+1));
                responses[i].Score = i + 2;
                responses[i].SetIdTo(i + 1);
            }
            responses[0].Value = "3";
            responses[1].Value = "5";
            responses[2].Value = "8";
            responses[3].Value = "12";
            responses[4].Value = "13";
            responses[4].IsActive = false;
            responses[5].Value = "14";
            
            responses[6].Value = "15.2";
            responses[6].Score = 99;

            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[6]);

            return scrambledResponses;
        }
        #endregion Helper Methods



    }
}
