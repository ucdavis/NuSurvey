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
            SetupQuestions();
        }

        #region Score Question Tests
        #region Non-Open Ended Tests

        [TestMethod]
        public void TestWhenQuestionHasNoAnswer()
        {
            #region Arrange
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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

        #region Open Ended Whole Number Tests Scored

        [TestMethod]
        public void TestWholeNumberRangeOfNumbers()
        {
            #region Arrange
            //SetupQuestions();
            var qAndA = new QuestionAnswerParameter {QuestionId = 3};

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
            //SetupQuestions();
            var qAndA = new QuestionAnswerParameter {QuestionId = 3, Answer = null};
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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

        #region Open Ended Decimal Tests
        [TestMethod]
        public void TestDecimalNumberRangeOfNumbers()
        {
            #region Arrange
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
            var qAndA = new QuestionAnswerParameter { QuestionId =5, Answer = "3 5" };
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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

        #region Open Ended Time Tests
        [TestMethod]
        public void TestTimeValueRangeOfNumbers()
        {
            #region Arrange
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
            //SetupQuestions();
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
        
        #endregion Open Ended Time Range
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
            for (int i = 0; i < 12; i++)
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

            #region Question 4 (Open Ended Whole Number No Scoreing)
            questions[3].Name = "Whole Number";
            foreach (var response in WholeNumberResponses())
            {
                questions[3].AddResponse(response);
            }
            questions[3].IsOpenEnded = true;
            questions[3].OpenEndedQuestionType = (int)QuestionType.WholeNumber;
            questions[3].Category.DoNotUseForCalculations = true;
            #endregion Question 4 (Open Ended Whole Number No Scoreing)

            #region Question 5 (Open Ended Decimal)
            questions[4].Name = "Decimal";
            foreach (var response in DecimalResponses())
            {
                questions[4].AddResponse(response);
            }
            questions[4].IsOpenEnded = true;
            questions[4].OpenEndedQuestionType = (int)QuestionType.Decimal;
            #endregion Question 5 (Open Ended Decimal)

            #region Question 6 (Open Ended Decimal No Scoring)
            questions[5].Name = "Decimal";
            foreach (var response in DecimalResponses())
            {
                questions[5].AddResponse(response);
            }
            questions[5].IsOpenEnded = true;
            questions[5].OpenEndedQuestionType = (int)QuestionType.Decimal;
            questions[5].Category.DoNotUseForCalculations = true;
            #endregion Question 6 (Open Ended Decimal No Scoring)

            #region Question 7 (Open Ended Time)
            questions[6].Name = "Time";
            foreach (var response in TimeResponses())
            {
                questions[6].AddResponse(response);
            }
            questions[6].IsOpenEnded = true;
            questions[6].OpenEndedQuestionType = (int)QuestionType.Time;
            #endregion Question 7 (Open Ended Time)

            #region Question 8 (Open Ended Time No Scoring)
            questions[7].Name = "Time";
            foreach (var response in TimeResponses())
            {
                questions[7].AddResponse(response);
            }
            questions[7].IsOpenEnded = true;
            questions[7].OpenEndedQuestionType = (int)QuestionType.Time;
            questions[7].Category.DoNotUseForCalculations = true;
            #endregion Question 8 (Open Ended Time No Scoring)

            #region Question 9 (Open Ended Time (AM/PM))
            questions[8].Name = "Time AM/PM";
            foreach (var response in TimeResponsesAmPm())
            {
                questions[8].AddResponse(response);
            }
            questions[8].IsOpenEnded = true;
            questions[8].OpenEndedQuestionType = (int)QuestionType.TimeAmPm;
            #endregion Question 9 (Open Ended Time (AM/PM))

            #region Question 10 (Open Ended Time (AM/PM) No Scoreing)
            questions[9].Name = "Time AM/PM";
            foreach (var response in TimeResponsesAmPm())
            {
                questions[9].AddResponse(response);
            }
            questions[9].IsOpenEnded = true;
            questions[9].OpenEndedQuestionType = (int)QuestionType.TimeAmPm;
            questions[9].Category.DoNotUseForCalculations = true;
            #endregion Question 10 (Open Ended Time (AM/PM) No Scoring)

            #region Question 11 (Open Ended Time Range)
            questions[10].Name = "Time Range";
            foreach (var response in TimeRangeResponses())
            {
                questions[10].AddResponse(response);
            }
            questions[10].IsOpenEnded = true;
            questions[10].OpenEndedQuestionType = (int)QuestionType.TimeRange;
            #endregion Question 11 (Open Ended Time Range)

            new FakeQuestions(0, QuestionRepository, questions);
        }       


        private static IEnumerable<Response> WholeNumberResponses()
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

        private static IEnumerable<Response> DecimalResponses()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 11; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].Score = i + 1;
                responses[i].SetIdTo(i + 1);
            }

            responses[0].Value = "-1.2";
            responses[1].Value = "0";
            responses[2].Value = "1.5";
            responses[3].Value = "2";
            responses[4].Value = "3";
            responses[5].Value = "4.25";
            responses[6].Value = "4.5";
            responses[7].Value = "5";
            responses[7].IsActive = false;
            responses[8].Value = "6";
            responses[9].Value = "6.01";

            responses[10].Value = "TEN";
            responses[10].Score = 99;

            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[6]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[7]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[8]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[10]);
            scrambledResponses.Add(responses[9]);
            return scrambledResponses;
        }

        private static IEnumerable<Response> TimeResponses()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 9; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].Score = i + 1;
                responses[i].SetIdTo(i + 1);
            }

            responses[0].Value = "1:30";
            responses[1].Value = "1:45";
            responses[2].Value = "2:00";
            responses[3].Value = "3:00";
            responses[4].Value = "5:00";
            responses[5].Value = "8:00";
            responses[6].Value = "NINE";
            responses[6].Score = 99;
            responses[7].Value = "10:00";
            responses[7].IsActive = false;
            responses[8].Value = "12:30";


            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[6]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[8]);
            scrambledResponses.Add(responses[7]);
            return scrambledResponses;
        }

        private static IEnumerable<Response> TimeResponsesAmPm()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 12; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].Score = i + 1;
                responses[i].SetIdTo(i + 1);
            }

            responses[0].Value = "1:30 AM";
            responses[1].Value = "1:45 AM";
            responses[2].Value = "2:00 AM";
            responses[3].Value = "3:00 AM";
            responses[4].Value = "5:00 AM";
            responses[5].Value = "8:00 AM";
            responses[6].Value = "12:00 PM";
            responses[7].Value = "6:30 PM";
            responses[8].Value = "12:00 AM"; //Note, this will have a different sort             
            responses[9].Value = "NINE";
            responses[9].Score = 99;
            responses[10].Value = "10:00 PM";
            responses[10].IsActive = false;
            responses[11].Value = "11:59 PM";


            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[11]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[10]);
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[8]);
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[6]);
            scrambledResponses.Add(responses[7]);
            scrambledResponses.Add(responses[9]);
            return scrambledResponses;
        }

        private static IEnumerable<Response> TimeRangeResponses()
        {
            var responses = new List<Response>();
            for (int i = 0; i < 17; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].Score = i + 1;
                responses[i].SetIdTo(i + 1);
            }

            responses[0].Value = "0";
            responses[1].Value = "1";
            responses[2].Value = "1.5";
            responses[3].Value = "2";
            responses[4].Value = "3";
            responses[5].Value = "4";
            responses[5].IsActive = false;
            responses[6].Value = "5";
            responses[7].Value = "12";          
            responses[8].Value = "18";
            responses[9].Value = "22";
            responses[10].Value = "TEN";
            responses[10].Score = 99;
            responses[11].Value = "23.99";
            responses[12].Value = "24"; //Can't hit this one either because the fraction is closer to 23.99
            responses[13].Value = "36"; //None of these higher values should be reachable.
            responses[14].Value = "47";
            responses[15].Value = "47.90";
            responses[16].Value = "48";

            var scrambledResponses = new List<Response>(); //Because the service sorts.
            scrambledResponses.Add(responses[3]);
            scrambledResponses.Add(responses[12]);
            scrambledResponses.Add(responses[5]);
            scrambledResponses.Add(responses[13]);
            scrambledResponses.Add(responses[7]);
            scrambledResponses.Add(responses[2]);
            scrambledResponses.Add(responses[1]);
            scrambledResponses.Add(responses[15]);
            scrambledResponses.Add(responses[6]);
            scrambledResponses.Add(responses[9]);
            scrambledResponses.Add(responses[14]);
            scrambledResponses.Add(responses[8]);
            scrambledResponses.Add(responses[16]);
            scrambledResponses.Add(responses[0]);
            scrambledResponses.Add(responses[11]);
            scrambledResponses.Add(responses[4]);
            scrambledResponses.Add(responses[10]);

            
            
            return scrambledResponses;
        }
        #endregion Helper Methods

        public class TestScoreParameters
        {
            public int Score;
            public string StartTime;
            public string EndTime;
        }

    }
    
}
