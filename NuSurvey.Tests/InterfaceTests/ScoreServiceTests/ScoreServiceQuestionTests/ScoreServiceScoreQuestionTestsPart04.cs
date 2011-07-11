using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceQuestionTests
{
    public partial class ScoreServiceQuestionTests
    {
        #region Open Ended Time Tests
        [TestMethod]
        public void TestTimeValueRangeOfNumbers()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7 };

            var dict = new Dictionary<string, int>(); //answer, expected score
            dict.Add("1:00", 1);
            dict.Add("1:15", 1);
            dict.Add("1:30", 1);
            dict.Add("1:37", 1);
            dict.Add("1:38", 2);
            dict.Add("1:45", 2);
            dict.Add("1:52", 2);
            dict.Add("01:53", 3);
            dict.Add("2:00", 3);
            dict.Add("2:29", 3);
            dict.Add("2:30", 4);
            dict.Add("03:00", 4);
            dict.Add("3:59", 4);
            dict.Add("4:00", 5);
            dict.Add("4:59", 5);
            dict.Add("5:00", 5);
            dict.Add("6:29", 5);
            dict.Add("6:30", 6);
            dict.Add("8:00", 6);
            dict.Add("9:00", 6);
            dict.Add("10:00", 6);
            dict.Add("10:14", 6);
            dict.Add("10:15", 9);
            dict.Add("12:30", 9);
            dict.Add("12:31", 9);
            dict.Add("12:59", 9);
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
        public void TestAnswerNotTime1()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = null };
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
        public void TestAnswerNotTime2()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = string.Empty };
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
        public void TestAnswerNotTime3()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = " " };
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
        public void TestAnswerNotTime4()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "ten" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime5()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "3.45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime6()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "3 45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime7()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "3:4" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime8()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = ":456" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime9()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "1.2:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }
        [TestMethod]
        public void TestAnswerNotTime10()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "0:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime11()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "-1.2:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime12()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "13:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime13()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "12:60" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime14()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 7, Answer = "12:61" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }



        #endregion Open Ended Time Tests

        #region Open Ended Time Tests Not Scored
        [TestMethod]
        public void TestTimeValueRangeOfNumbersNotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8 };

            var dict = new Dictionary<string, int>(); //answer, expected score
            dict.Add("1:00", 1);
            dict.Add("1:15", 1);
            dict.Add("1:30", 1);
            dict.Add("1:37", 1);
            dict.Add("1:38", 2);
            dict.Add("1:45", 2);
            dict.Add("1:52", 2);
            dict.Add("01:53", 3);
            dict.Add("2:00", 3);
            dict.Add("2:29", 3);
            dict.Add("2:30", 4);
            dict.Add("03:00", 4);
            dict.Add("3:59", 4);
            dict.Add("4:00", 5);
            dict.Add("4:59", 5);
            dict.Add("5:00", 5);
            dict.Add("6:29", 5);
            dict.Add("6:30", 6);
            dict.Add("8:00", 6);
            dict.Add("9:00", 6);
            dict.Add("10:00", 6);
            dict.Add("10:14", 6);
            dict.Add("10:15", 9);
            dict.Add("12:30", 9);
            dict.Add("12:31", 9);
            dict.Add("12:59", 9);
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
        public void TestAnswerNotTime1NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = null };
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
        public void TestAnswerNotTime2NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = string.Empty };
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
        public void TestAnswerNotTime3NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = " " };
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
        public void TestAnswerNotTime4NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "ten" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime5NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "3.45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime6NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "3 45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime7NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "3:4" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime8NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = ":456" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime9NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "1.2:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }
        [TestMethod]
        public void TestAnswerNotTime10NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "0:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime11NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "-1.2:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime12NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "13:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime13NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "12:60" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTime14NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 8, Answer = "12:61" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }



        #endregion Open Ended Time Tests
    }
}