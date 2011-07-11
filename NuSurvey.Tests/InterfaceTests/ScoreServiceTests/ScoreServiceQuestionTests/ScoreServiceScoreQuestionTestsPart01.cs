using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceQuestionTests
{
    public partial class ScoreServiceQuestionTests
    {
        #region Non-Open Ended Tests

        [TestMethod]
        public void TestWhenQuestionHasNoAnswer()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 1, ResponseId = 0 };
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

            var qAndA = new QuestionAnswerParameter { QuestionId = 1, ResponseId = 2 };
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
    }
}
