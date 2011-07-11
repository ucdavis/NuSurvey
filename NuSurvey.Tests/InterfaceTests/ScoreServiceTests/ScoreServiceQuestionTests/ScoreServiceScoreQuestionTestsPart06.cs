using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuSurvey.Web.Controllers

namespace NuSurvey.Tests.InterfaceTests.ScoreServiceTests.ScoreServiceQuestionTests
{
    public partial class ScoreServiceQuestionTests
    {
        #region Open Ended Time Range
        [TestMethod]
        public void TestTimeRangeValueRangeOfNumbers()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11 };

            var dict = new Dictionary<int, TestScoreParameters>(); //answer, expected score
            #region Range of zero to .49 Hours
            dict.Add(1, new TestScoreParameters { Score = 1, StartTime = "12:00 AM", EndTime = "12:00 AM" });
            dict.Add(2, new TestScoreParameters { Score = 1, StartTime = "12:00 AM", EndTime = "12:01 AM" });
            dict.Add(3, new TestScoreParameters { Score = 1, StartTime = "12:00 AM", EndTime = "12:29 AM" });
            dict.Add(4, new TestScoreParameters { Score = 1, StartTime = "2:01 PM", EndTime = "2:30 PM" });
            dict.Add(5, new TestScoreParameters { Score = 1, StartTime = "3:30 PM", EndTime = "3:59 PM" });
            dict.Add(6, new TestScoreParameters { Score = 1, StartTime = "1:25 PM", EndTime = "1:30 PM" });
            dict.Add(7, new TestScoreParameters { Score = 1, StartTime = "11:59 PM", EndTime = "12:28 AM" });
            dict.Add(8, new TestScoreParameters { Score = 1, StartTime = "11:31 PM", EndTime = "12:00 AM" });
            #endregion Range of zero to .49 Hours

            #region Range of 0.5 to 1.24 Hours (:30 to 1:14)
            dict.Add(9, new TestScoreParameters { Score = 2, StartTime = "12:00 AM", EndTime = "12:30 AM" });
            dict.Add(10, new TestScoreParameters { Score = 2, StartTime = "12:00 AM", EndTime = "1:14 AM" });
            dict.Add(11, new TestScoreParameters { Score = 2, StartTime = "8:16 AM", EndTime = "8:46 AM" });
            dict.Add(12, new TestScoreParameters { Score = 2, StartTime = "11:59 PM", EndTime = "1:13 AM" });
            #endregion Range of 0.5 to 1.24 Hours

            #region Range of 1.25 to 1.74
            dict.Add(13, new TestScoreParameters { Score = 3, StartTime = "12:00 AM", EndTime = "1:15 AM" });
            dict.Add(14, new TestScoreParameters { Score = 3, StartTime = "12:00 AM", EndTime = "1:44 AM" });
            dict.Add(15, new TestScoreParameters { Score = 3, StartTime = "11:59 PM", EndTime = "1:43 AM" });
            #endregion Range of 1.25 to 1.74

            #region Range of 1.75 to 2.49
            dict.Add(16, new TestScoreParameters { Score = 4, StartTime = "12:00 AM", EndTime = "1:45 AM" });
            dict.Add(17, new TestScoreParameters { Score = 4, StartTime = "12:00 AM", EndTime = "2:29 AM" });
            dict.Add(18, new TestScoreParameters { Score = 4, StartTime = "11:59 PM", EndTime = "2:28 AM" });
            #endregion Range of 1.75 to 2.49

            #region Range of 3.50 to 3.99
            dict.Add(19, new TestScoreParameters { Score = 5, StartTime = "12:00 AM", EndTime = "2:30 AM" });
            dict.Add(20, new TestScoreParameters { Score = 5, StartTime = "12:00 AM", EndTime = "3:59 AM" });
            dict.Add(21, new TestScoreParameters { Score = 5, StartTime = "11:59 PM", EndTime = "2:29 AM" });
            dict.Add(22, new TestScoreParameters { Score = 5, StartTime = "11:59 PM", EndTime = "3:58 AM" });
            #endregion Range of 3.50 to 3.99

            #region Range of 4 to 5 to 8.49
            dict.Add(23, new TestScoreParameters { Score = 7, StartTime = "12:00 AM", EndTime = "4:00 AM" });
            dict.Add(24, new TestScoreParameters { Score = 7, StartTime = "12:00 AM", EndTime = "5:00 AM" });
            dict.Add(25, new TestScoreParameters { Score = 7, StartTime = "12:00 AM", EndTime = "8:29 AM" });
            dict.Add(26, new TestScoreParameters { Score = 7, StartTime = "11:59 PM", EndTime = "3:59 AM" });
            dict.Add(27, new TestScoreParameters { Score = 7, StartTime = "11:59 PM", EndTime = "8:28 AM" });
            #endregion Range of 4 to 8.49

            #region Range of 8.5 to 14.99
            dict.Add(28, new TestScoreParameters { Score = 8, StartTime = "12:00 AM", EndTime = "8:30 AM" });
            dict.Add(29, new TestScoreParameters { Score = 8, StartTime = "12:00 AM", EndTime = "2:59 PM" });
            dict.Add(30, new TestScoreParameters { Score = 8, StartTime = "12:00 AM", EndTime = "12:00 PM" });
            dict.Add(31, new TestScoreParameters { Score = 8, StartTime = "12:00 PM", EndTime = "1:00 AM" });
            dict.Add(32, new TestScoreParameters { Score = 8, StartTime = "11:59 PM", EndTime = "8:29 AM" });
            dict.Add(33, new TestScoreParameters { Score = 8, StartTime = "11:59 PM", EndTime = "2:58 PM" });
            #endregion Range of 8.5 to 14.99

            #region Range of 14.5 to 19.99
            dict.Add(34, new TestScoreParameters { Score = 9, StartTime = "12:00 AM", EndTime = "3:00 PM" });
            dict.Add(35, new TestScoreParameters { Score = 9, StartTime = "12:00 AM", EndTime = "7:59 PM" });
            dict.Add(36, new TestScoreParameters { Score = 9, StartTime = "12:00 PM", EndTime = "7:59 AM" });
            dict.Add(38, new TestScoreParameters { Score = 9, StartTime = "11:59 PM", EndTime = "2:59 PM" });
            dict.Add(39, new TestScoreParameters { Score = 9, StartTime = "11:59 PM", EndTime = "7:58 PM" });
            #endregion Range of 14.5 to 19.99

            #region Range of 20 to 22.99
            dict.Add(40, new TestScoreParameters { Score = 10, StartTime = "12:00 AM", EndTime = "8:00 PM" });
            dict.Add(41, new TestScoreParameters { Score = 10, StartTime = "12:00 AM", EndTime = "10:59 PM" });
            dict.Add(42, new TestScoreParameters { Score = 10, StartTime = "12:00 PM", EndTime = "10:59 AM" });
            dict.Add(43, new TestScoreParameters { Score = 10, StartTime = "11:59 PM", EndTime = "7:59 PM" });
            #endregion Range of 20 to 22.99

            #region Range of 22.99 to 23.99
            dict.Add(44, new TestScoreParameters { Score = 12, StartTime = "12:00 AM", EndTime = "11:00 PM" });
            dict.Add(45, new TestScoreParameters { Score = 12, StartTime = "12:00 AM", EndTime = "11:58 PM" });
            dict.Add(46, new TestScoreParameters { Score = 12, StartTime = "12:00 AM", EndTime = "11:59 PM" });
            dict.Add(47, new TestScoreParameters { Score = 12, StartTime = "12:00 PM", EndTime = "11:59 AM" });
            #endregion Range of 22.99 to 23.99

            #endregion Arrange

            #region Act
            foreach (var i in dict)
            {
                qAndA.Answer = i.Value.StartTime;
                qAndA.AnswerRange = i.Value.EndTime;
                var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
                Assert.IsNotNull(result, string.Format("Unexpected value for {0}", i.Key));
                Assert.AreEqual(i.Value.Score, result.Score, string.Format("Unexpected score for {0}", i.Key));
                Assert.IsFalse(result.Invalid, string.Format("Unexpected value for invalid for {0}", i.Key));
                Assert.AreEqual(string.Empty, result.Message);
                Assert.IsTrue(result.ResponseId > 0);
            }
            #endregion Act
        }

        [TestMethod]
        public void TestAnswerRangeTime1()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = null, AnswerRange = null };
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
        public void TestAnswerRangeTime2()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = null, AnswerRange = "12:00 PM" };
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
        public void TestAnswerRangeTime3()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:00 PM", AnswerRange = null };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime4()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = string.Empty, AnswerRange = string.Empty };
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
        public void TestAnswerRangeTime5()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = string.Empty, AnswerRange = "12:00 PM" };
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
        public void TestAnswerRangeTime6()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:00 PM", AnswerRange = string.Empty };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime7()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = " ", AnswerRange = " " };
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
        public void TestAnswerRangeTime8()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = " ", AnswerRange = "12:00 PM" };
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
        public void TestAnswerRangeTime9()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:00 PM", AnswerRange = " " };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime10()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12.34 PM", AnswerRange = "12.34 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime11()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12.34 PM", AnswerRange = "12:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime12()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:00 PM", AnswerRange = "12.34 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime13()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:34 XX", AnswerRange = "12:34 XX" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime14()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:34 XX", AnswerRange = "12:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime15()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:00 PM", AnswerRange = "12:34 XX" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime16()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12.3456", AnswerRange = "12.3456" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime17()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12.3456", AnswerRange = "12:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime18()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:00 PM", AnswerRange = "12.3456" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime19()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:60 AM", AnswerRange = "12:60 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime20()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "0:00 AM", AnswerRange = "12:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime21()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 11, Answer = "12:00 PM", AnswerRange = "13:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }
        #endregion Open Ended Time Range

        #region Open Ended Time Range Not Scored
        [TestMethod]
        public void TestTimeRangeValueRangeOfNumbersNotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12 };

            var dict = new Dictionary<int, TestScoreParameters>(); //answer, expected score
            #region Range of zero to .49 Hours
            dict.Add(1, new TestScoreParameters { Score = 1, StartTime = "12:00 AM", EndTime = "12:00 AM" });
            dict.Add(2, new TestScoreParameters { Score = 1, StartTime = "12:00 AM", EndTime = "12:01 AM" });
            dict.Add(3, new TestScoreParameters { Score = 1, StartTime = "12:00 AM", EndTime = "12:29 AM" });
            dict.Add(4, new TestScoreParameters { Score = 1, StartTime = "2:01 PM", EndTime = "2:30 PM" });
            dict.Add(5, new TestScoreParameters { Score = 1, StartTime = "3:30 PM", EndTime = "3:59 PM" });
            dict.Add(6, new TestScoreParameters { Score = 1, StartTime = "1:25 PM", EndTime = "1:30 PM" });
            dict.Add(7, new TestScoreParameters { Score = 1, StartTime = "11:59 PM", EndTime = "12:28 AM" });
            dict.Add(8, new TestScoreParameters { Score = 1, StartTime = "11:31 PM", EndTime = "12:00 AM" });
            #endregion Range of zero to .49 Hours

            #region Range of 0.5 to 1.24 Hours (:30 to 1:14)
            dict.Add(9, new TestScoreParameters { Score = 2, StartTime = "12:00 AM", EndTime = "12:30 AM" });
            dict.Add(10, new TestScoreParameters { Score = 2, StartTime = "12:00 AM", EndTime = "1:14 AM" });
            dict.Add(11, new TestScoreParameters { Score = 2, StartTime = "8:16 AM", EndTime = "8:46 AM" });
            dict.Add(12, new TestScoreParameters { Score = 2, StartTime = "11:59 PM", EndTime = "1:13 AM" });
            #endregion Range of 0.5 to 1.24 Hours

            #region Range of 1.25 to 1.74
            dict.Add(13, new TestScoreParameters { Score = 3, StartTime = "12:00 AM", EndTime = "1:15 AM" });
            dict.Add(14, new TestScoreParameters { Score = 3, StartTime = "12:00 AM", EndTime = "1:44 AM" });
            dict.Add(15, new TestScoreParameters { Score = 3, StartTime = "11:59 PM", EndTime = "1:43 AM" });
            #endregion Range of 1.25 to 1.74

            #region Range of 1.75 to 2.49
            dict.Add(16, new TestScoreParameters { Score = 4, StartTime = "12:00 AM", EndTime = "1:45 AM" });
            dict.Add(17, new TestScoreParameters { Score = 4, StartTime = "12:00 AM", EndTime = "2:29 AM" });
            dict.Add(18, new TestScoreParameters { Score = 4, StartTime = "11:59 PM", EndTime = "2:28 AM" });
            #endregion Range of 1.75 to 2.49

            #region Range of 3.50 to 3.99
            dict.Add(19, new TestScoreParameters { Score = 5, StartTime = "12:00 AM", EndTime = "2:30 AM" });
            dict.Add(20, new TestScoreParameters { Score = 5, StartTime = "12:00 AM", EndTime = "3:59 AM" });
            dict.Add(21, new TestScoreParameters { Score = 5, StartTime = "11:59 PM", EndTime = "2:29 AM" });
            dict.Add(22, new TestScoreParameters { Score = 5, StartTime = "11:59 PM", EndTime = "3:58 AM" });
            #endregion Range of 3.50 to 3.99

            #region Range of 4 to 5 to 8.49
            dict.Add(23, new TestScoreParameters { Score = 7, StartTime = "12:00 AM", EndTime = "4:00 AM" });
            dict.Add(24, new TestScoreParameters { Score = 7, StartTime = "12:00 AM", EndTime = "5:00 AM" });
            dict.Add(25, new TestScoreParameters { Score = 7, StartTime = "12:00 AM", EndTime = "8:29 AM" });
            dict.Add(26, new TestScoreParameters { Score = 7, StartTime = "11:59 PM", EndTime = "3:59 AM" });
            dict.Add(27, new TestScoreParameters { Score = 7, StartTime = "11:59 PM", EndTime = "8:28 AM" });
            #endregion Range of 4 to 8.49

            #region Range of 8.5 to 14.99
            dict.Add(28, new TestScoreParameters { Score = 8, StartTime = "12:00 AM", EndTime = "8:30 AM" });
            dict.Add(29, new TestScoreParameters { Score = 8, StartTime = "12:00 AM", EndTime = "2:59 PM" });
            dict.Add(30, new TestScoreParameters { Score = 8, StartTime = "12:00 AM", EndTime = "12:00 PM" });
            dict.Add(31, new TestScoreParameters { Score = 8, StartTime = "12:00 PM", EndTime = "1:00 AM" });
            dict.Add(32, new TestScoreParameters { Score = 8, StartTime = "11:59 PM", EndTime = "8:29 AM" });
            dict.Add(33, new TestScoreParameters { Score = 8, StartTime = "11:59 PM", EndTime = "2:58 PM" });
            #endregion Range of 8.5 to 14.99

            #region Range of 14.5 to 19.99
            dict.Add(34, new TestScoreParameters { Score = 9, StartTime = "12:00 AM", EndTime = "3:00 PM" });
            dict.Add(35, new TestScoreParameters { Score = 9, StartTime = "12:00 AM", EndTime = "7:59 PM" });
            dict.Add(36, new TestScoreParameters { Score = 9, StartTime = "12:00 PM", EndTime = "7:59 AM" });
            dict.Add(38, new TestScoreParameters { Score = 9, StartTime = "11:59 PM", EndTime = "2:59 PM" });
            dict.Add(39, new TestScoreParameters { Score = 9, StartTime = "11:59 PM", EndTime = "7:58 PM" });
            #endregion Range of 14.5 to 19.99

            #region Range of 20 to 22.99
            dict.Add(40, new TestScoreParameters { Score = 10, StartTime = "12:00 AM", EndTime = "8:00 PM" });
            dict.Add(41, new TestScoreParameters { Score = 10, StartTime = "12:00 AM", EndTime = "10:59 PM" });
            dict.Add(42, new TestScoreParameters { Score = 10, StartTime = "12:00 PM", EndTime = "10:59 AM" });
            dict.Add(43, new TestScoreParameters { Score = 10, StartTime = "11:59 PM", EndTime = "7:59 PM" });
            #endregion Range of 20 to 22.99

            #region Range of 22.99 to 23.99
            dict.Add(44, new TestScoreParameters { Score = 12, StartTime = "12:00 AM", EndTime = "11:00 PM" });
            dict.Add(45, new TestScoreParameters { Score = 12, StartTime = "12:00 AM", EndTime = "11:58 PM" });
            dict.Add(46, new TestScoreParameters { Score = 12, StartTime = "12:00 AM", EndTime = "11:59 PM" });
            dict.Add(47, new TestScoreParameters { Score = 12, StartTime = "12:00 PM", EndTime = "11:59 AM" });
            #endregion Range of 22.99 to 23.99

            #endregion Arrange

            #region Act
            foreach (var i in dict)
            {
                qAndA.Answer = i.Value.StartTime;
                qAndA.AnswerRange = i.Value.EndTime;
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
        public void TestAnswerRangeTime1NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = null, AnswerRange = null };
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
        public void TestAnswerRangeTime2NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = null, AnswerRange = "12:00 PM" };
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
        public void TestAnswerRangeTime3NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:00 PM", AnswerRange = null };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime4NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = string.Empty, AnswerRange = string.Empty };
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
        public void TestAnswerRangeTime5NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = string.Empty, AnswerRange = "12:00 PM" };
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
        public void TestAnswerRangeTime6NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:00 PM", AnswerRange = string.Empty };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime7NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = " ", AnswerRange = " " };
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
        public void TestAnswerRangeTime8NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = " ", AnswerRange = "12:00 PM" };
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
        public void TestAnswerRangeTime9NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:00 PM", AnswerRange = " " };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime10NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12.34 PM", AnswerRange = "12.34 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime11NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12.34 PM", AnswerRange = "12:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime12NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:00 PM", AnswerRange = "12.34 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime13NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:34 XX", AnswerRange = "12:34 XX" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime14NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:34 XX", AnswerRange = "12:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime15NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:00 PM", AnswerRange = "12:34 XX" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime16NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12.3456", AnswerRange = "12.3456" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime17NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12.3456", AnswerRange = "12:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime18NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:00 PM", AnswerRange = "12.3456" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime19NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:60 AM", AnswerRange = "12:60 AM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime20NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "0:00 AM", AnswerRange = "12:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerRangeTime21NotScored()
        {
            #region Arrange
            var qAndA = new QuestionAnswerParameter { QuestionId = 12, Answer = "12:00 PM", AnswerRange = "13:00 PM" };
            #endregion Arrange

            #region Act
            var result = ScoreService.ScoreQuestion(QuestionRepository.Queryable, qAndA);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Invalid);
            Assert.AreEqual("Answers must be a Time (hh:mm AM/PM)", result.Message);
            Assert.AreEqual(0, result.Score);
            Assert.AreEqual(0, result.ResponseId);
            #endregion Assert
        }
        #endregion Open Ended Time Range No Score
    }
}
