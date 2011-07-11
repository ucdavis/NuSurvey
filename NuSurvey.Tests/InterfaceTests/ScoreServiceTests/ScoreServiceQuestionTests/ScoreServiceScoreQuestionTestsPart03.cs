using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceQuestionTests
{
    public partial class ScoreServiceQuestionTests
    {
        #region Open Ended Decimal Tests
        [TestMethod]
        public void TestDecimalNumberRangeOfNumbers()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 5 };

            var dict = new Dictionary<string, int>(); //answer, expected score
            dict.Add("-12", 1);
            dict.Add("-1.3", 1);
            dict.Add("-1.2", 1);
            dict.Add("-1.1", 1);
            dict.Add("-0.2", 2);
            dict.Add("0", 2);
            dict.Add("0.74", 2);
            dict.Add("0.7499", 2);
            dict.Add("0.75", 3);
            dict.Add("1", 3);
            dict.Add("1.5", 3);
            dict.Add("1.7499", 3);
            dict.Add("1.75", 4);
            dict.Add("1.999", 4);
            dict.Add("2", 4);
            dict.Add("2.4999", 4);
            dict.Add("2.5", 5);
            dict.Add("2.999", 5);
            dict.Add("3", 5);
            dict.Add("3.624999", 5);
            dict.Add("3.625", 6);
            dict.Add("4.25", 6);
            dict.Add("4.37499", 6);
            dict.Add("4.375", 7);
            dict.Add("4.5", 7);
            dict.Add("5", 7);
            dict.Add("5.24999", 7);
            dict.Add("5.25", 9);
            dict.Add("6.00", 9);
            dict.Add("6.00499", 9);
            dict.Add("6.005", 10);
            dict.Add("6.01", 10);
            dict.Add("123.123", 10);
            #endregion Arrange

            #region Act
            foreach (var i in dict)
            {
                qAndA.Answer = i.Key;
                var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
                Assert.IsNotNull(result, string.Format("Unexpected value for {0}", i.Key));
                Assert.AreEqual(i.Value, result.Score, string.Format("Unexpected score for {0}", i.Key));
                Assert.IsFalse(result.Invalid, string.Format("Unexpected value for invalid for {0}", i.Key));
                Assert.AreEqual(string.Empty, result.Message);
                Assert.IsTrue(result.ResponseId > 0);
            }
            #endregion Act
        }

        [TestMethod]
        public void TestAnswerNotDecimalNumber1()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 5, Answer = null };
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
        public void TestAnswerNotDecimalNumber2()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 5, Answer = string.Empty };
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
        public void TestAnswerNotDecimalNumber3()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 5, Answer = " " };
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
        public void TestAnswerNotDecimalNumber4()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 5, Answer = "ten" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a number (decimal ok)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }


        [TestMethod]
        public void TestAnswerNotDecimalNumber5()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 5, Answer = "3:5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a number (decimal ok)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotDecimalNumber6()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 5, Answer = "3 5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a number (decimal ok)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        #endregion Open Ended Decimal Tests

        #region Open Ended Decimal Tests Not Scored
        [TestMethod]
        public void TestDecimalNumberRangeOfNumbersNotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 6 };

            var dict = new Dictionary<string, int>(); //answer, expected score
            dict.Add("-12", 1);
            dict.Add("-1.3", 1);
            dict.Add("-1.2", 1);
            dict.Add("-1.1", 1);
            dict.Add("-0.2", 2);
            dict.Add("0", 2);
            dict.Add("0.74", 2);
            dict.Add("0.7499", 2);
            dict.Add("0.75", 3);
            dict.Add("1", 3);
            dict.Add("1.5", 3);
            dict.Add("1.7499", 3);
            dict.Add("1.75", 4);
            dict.Add("1.999", 4);
            dict.Add("2", 4);
            dict.Add("2.4999", 4);
            dict.Add("2.5", 5);
            dict.Add("2.999", 5);
            dict.Add("3", 5);
            dict.Add("3.624999", 5);
            dict.Add("3.625", 6);
            dict.Add("4.25", 6);
            dict.Add("4.37499", 6);
            dict.Add("4.375", 7);
            dict.Add("4.5", 7);
            dict.Add("5", 7);
            dict.Add("5.24999", 7);
            dict.Add("5.25", 9);
            dict.Add("6.00", 9);
            dict.Add("6.00499", 9);
            dict.Add("6.005", 10);
            dict.Add("6.01", 10);
            dict.Add("123.123", 10);
            #endregion Arrange

            #region Act
            foreach (var i in dict)
            {
                qAndA.Answer = i.Key;
                var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
                Assert.IsNotNull(result, string.Format("Unexpected value for {0}", i.Key));
                Assert.AreEqual(0, result.Score, string.Format("Unexpected score for {0}", i.Key));
                Assert.IsFalse(result.Invalid, string.Format("Unexpected value for invalid for {0}", i.Key));
                Assert.AreEqual(string.Empty, result.Message);
                Assert.IsTrue(result.ResponseId > 0);
            }
            #endregion Act
        }

        [TestMethod]
        public void TestAnswerNotDecimalNumber1NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 6, Answer = null };
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
        public void TestAnswerNotDecimalNumber2NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 6, Answer = string.Empty };
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
        public void TestAnswerNotDecimalNumber3NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 6, Answer = " " };
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
        public void TestAnswerNotDecimalNumber4NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 6, Answer = "ten" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a number (decimal ok)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }


        [TestMethod]
        public void TestAnswerNotDecimalNumber5NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 6, Answer = "3:5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a number (decimal ok)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotDecimalNumber6NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 6, Answer = "3 5" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a number (decimal ok)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        #endregion Open Ended Decimal Tests
    }
}