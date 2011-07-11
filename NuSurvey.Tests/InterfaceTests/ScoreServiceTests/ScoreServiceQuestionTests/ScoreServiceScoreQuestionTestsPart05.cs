using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceQuestionTests
{
    public partial class ScoreServiceQuestionTests
    {
        #region Open Ended Time (AM/PM) Tests
        [TestMethod]
        public void TestTimeAmPmValueRangeOfNumbers()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 9 };

            var dict = new Dictionary<string, int>(); //answer, expected score
            dict.Add("12:00 AM", 9);
            dict.Add("12:44 AM", 9);
            dict.Add("12:45 AM", 9);
            dict.Add("12:46 AM", 1);
            dict.Add("1:30 AM", 1);
            dict.Add("1:37 AM", 1);
            dict.Add("1:38 AM", 2);
            dict.Add("1:45 AM", 2);
            dict.Add("1:52 AM", 2);
            dict.Add("01:53 AM", 3);
            dict.Add("2:00 AM", 3);
            dict.Add("2:29 AM", 3);
            dict.Add("2:30 AM", 4);
            dict.Add("03:00 AM", 4);
            dict.Add("3:59 AM", 4);
            dict.Add("4:00 AM", 5);
            dict.Add("4:59 AM", 5);
            dict.Add("5:00 AM", 5);
            dict.Add("6:29 AM", 5);
            dict.Add("6:30 AM", 6);
            dict.Add("8:00 AM", 6);
            dict.Add("9:00 AM", 6);
            dict.Add("9:59 AM", 6);
            dict.Add("10:00 AM", 7);
            dict.Add("11:59 AM", 7);
            dict.Add("12:00 PM", 7);
            dict.Add("03:14 PM", 7);
            dict.Add("3:15 PM", 8);
            dict.Add("6:30 PM", 8);
            dict.Add("9:14 PM", 8);
            dict.Add("9:15 PM", 12);
            dict.Add("10:00 PM", 12);
            dict.Add("11:59 PM", 12);
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
        public void TestAnswerNotTimeAmPm1()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = null };
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
        public void TestAnswerNotTimeAmPm2()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = string.Empty };
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
        public void TestAnswerNotTimeAmPm3()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = " " };
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
        public void TestAnswerNotTimeAmPm4()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "3000" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm5()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "three AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm6()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "03:00 XX" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm7()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "14:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm8()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "03.00 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm9()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "03::0 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm10()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "00:00 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm11()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "-1:00 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm12()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "01:60 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm13()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "1:-1 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm14()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 9, Answer = "13:00 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        #endregion Open Ended Time (AM/PM) Tests

        #region Open Ended Time (AM/PM) Tests Not Scored
        [TestMethod]
        public void TestTimeAmPmValueRangeOfNumbersNotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 10 };

            var dict = new Dictionary<string, int>(); //answer, expected score
            dict.Add("12:00 AM", 9);
            dict.Add("12:44 AM", 9);
            dict.Add("12:45 AM", 9);
            dict.Add("12:46 AM", 1);
            dict.Add("1:30 AM", 1);
            dict.Add("1:37 AM", 1);
            dict.Add("1:38 AM", 2);
            dict.Add("1:45 AM", 2);
            dict.Add("1:52 AM", 2);
            dict.Add("01:53 AM", 3);
            dict.Add("2:00 AM", 3);
            dict.Add("2:29 AM", 3);
            dict.Add("2:30 AM", 4);
            dict.Add("03:00 AM", 4);
            dict.Add("3:59 AM", 4);
            dict.Add("4:00 AM", 5);
            dict.Add("4:59 AM", 5);
            dict.Add("5:00 AM", 5);
            dict.Add("6:29 AM", 5);
            dict.Add("6:30 AM", 6);
            dict.Add("8:00 AM", 6);
            dict.Add("9:00 AM", 6);
            dict.Add("9:59 AM", 6);
            dict.Add("10:00 AM", 7);
            dict.Add("11:59 AM", 7);
            dict.Add("12:00 PM", 7);
            dict.Add("03:14 PM", 7);
            dict.Add("3:15 PM", 8);
            dict.Add("6:30 PM", 8);
            dict.Add("9:14 PM", 8);
            dict.Add("9:15 PM", 12);
            dict.Add("10:00 PM", 12);
            dict.Add("11:59 PM", 12);
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
        public void TestAnswerNotTimeAmPm1NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = null };
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
        public void TestAnswerNotTimeAmPm2NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = string.Empty };
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
        public void TestAnswerNotTimeAmPm3NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = " " };
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
        public void TestAnswerNotTimeAmPm4NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "3000" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm5NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "three AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm6NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "03:00 XX" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm7NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "14:45" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm8NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "03.00 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm9NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "03::0 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm10NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "00:00 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm11NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "-1:00 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm12NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "01:60 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm13NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "1:-1 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNotTimeAmPm14NotScored()
        {
            #region Arrange

            var qAndA = new QuestionAnswerParameter { QuestionId = 10, Answer = "13:00 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answer must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        #endregion Open Ended Time (AM/PM) Tests Not Scored
    }
}