using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceQuestionTests
{
    public partial class ScoreServiceQuestionTests
    {
        #region Open Ended Whole Number Tests Scored

        [TestMethod]
        public void TestWholeNumberRangeOfNumbers()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 3 };

            var dict = new Dictionary<string, int>(); //answer, expected score
            dict.Add("-12", 2);
            dict.Add("-1", 2);
            dict.Add("0", 2);
            dict.Add("1", 2);
            dict.Add("2", 2);
            dict.Add("3", 2);
            dict.Add("4", 3);
            dict.Add("5", 3);
            dict.Add("6", 3);
            dict.Add("7", 4);
            dict.Add("8", 4);
            dict.Add("9", 4);
            dict.Add("10", 5);
            dict.Add("11", 5);
            dict.Add("12", 5);
            dict.Add("13", 7);
            dict.Add("14", 7);
            dict.Add("15", 7);
            dict.Add("16", 7);

            #endregion Arrange

            #region Act
            foreach (var i in dict)
            {
                qAndA.Answer = i.Key;
                var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
                Assert.IsNotNull(result, string.Format("Unexpected value for {0}", i.Key));
                Assert.AreEqual(i.Value, result.Score, string.Format("Unexpected score for {0}", i.Key));
                Assert.IsFalse(result.Invalid);
                Assert.AreEqual(string.Empty, result.Message);
                Assert.IsTrue(result.ResponseId > 0);
            }
            #endregion Act
        }


        [TestMethod]
        public void TestAnswerNotWholeNumber1()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 3, Answer = null };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer is required", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber2()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 3, Answer = string.Empty };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer is required", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber3()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 3, Answer = " " };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer is required", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber4()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 3, Answer = "ten" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a whole number", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber5()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 3, Answer = "3.5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a whole number", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber6()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 3, Answer = "3:5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a whole number", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber7()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 3, Answer = "3 5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a whole number", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        #endregion Open Ended Whole Number Tests Scored

        #region Open Ended Whole Number Tests Not Scored

        [TestMethod]
        public void TestWholeNumberRangeOfNumbersNotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 4 };

            var dict = new Dictionary<string, int>(); //answer, expected score
            dict.Add("-12", 2);
            dict.Add("-1", 2);
            dict.Add("0", 2);
            dict.Add("1", 2);
            dict.Add("2", 2);
            dict.Add("3", 2);
            dict.Add("4", 3);
            dict.Add("5", 3);
            dict.Add("6", 3);
            dict.Add("7", 4);
            dict.Add("8", 4);
            dict.Add("9", 4);
            dict.Add("10", 5);
            dict.Add("11", 5);
            dict.Add("12", 5);
            dict.Add("13", 7);
            dict.Add("14", 7);
            dict.Add("15", 7);
            dict.Add("16", 7);

            #endregion Arrange

            #region Act
            foreach (var i in dict)
            {
                qAndA.Answer = i.Key;
                var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
                Assert.IsNotNull(result, string.Format("Unexpected value for {0}", i.Key));
                Assert.AreEqual(0, result.Score, string.Format("Unexpected score for {0}", i.Key));
                Assert.IsFalse(result.Invalid);
                Assert.AreEqual(string.Empty, result.Message);
                Assert.IsTrue(result.ResponseId > 0);
            }
            #endregion Act
        }


        [TestMethod]
        public void TestAnswerNotWholeNumber1NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 4, Answer = null };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer is required", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber2NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 4, Answer = string.Empty };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer is required", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber3NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 4, Answer = " " };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer is required", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber4NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 4, Answer = "ten" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a whole number", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber5NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 4, Answer = "3.5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a whole number", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber6NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 4, Answer = "3:5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a whole number", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotWholeNumber7NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 4, Answer = "3 5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a whole number", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        #endregion Open Ended Whole Number Tests Not Scored
    }
}