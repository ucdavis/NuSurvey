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
                Assert.IsFalse(result.Invalid, string.Format("Unexpected score for {0}", i.Key));
                Assert.AreEqual(string.Empty, result.Message);
                Assert.IsTrue(result.ResponseId > 0);
            }
            #endregion Act
        }

        #endregion Open Ended Decimal Tests

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
            for (int i = 0; i < 10; i++)
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
            questions[4].Name = "Whole Number";
            foreach (var response in DecimalResponses())
            {
                questions[4].AddResponse(response);
            }
            questions[4].IsOpenEnded = true;
            questions[4].OpenEndedQuestionType = (int)QuestionType.Decimal;
            #endregion Question 5 (Open Ended Decimal)

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
        #endregion Helper Methods



    }
}
