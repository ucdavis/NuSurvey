using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    public partial class SurveyResponseControllerTests
    {
        #region Create Tests
        #region Create Get Tests
        [TestMethod]
        public void TestCreateGetRedirectsWhenSurveyNotFoundOrNotActive1()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            surveys[2].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            Controller.Create(3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey not found or not active.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenSurveyNotFoundOrNotActive2()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            surveys[2].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            Controller.Create(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey not found or not active.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetRedirectsWhenNotEnoughCategories()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 5; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].IsActive = true;
            }
            categories[0].IsActive = false;
            categories[1].DoNotUseForCalculations = true;
            categories[2].IsCurrentVersion = false;
            new FakeCategories(0, CategoryRepository, categories);

            var surveys = new List<Survey>();
            surveys.Add(CreateValidEntities.Survey(1));
            surveys[0].Categories = CategoryRepository.Queryable.ToList();
            new FakeSurveys(0, SurveyRepository, surveys);

            new FakeCategoryTotalMaxScore(5, CategoryTotalMaxScoreRepository);
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey does not have enough active categories to complete survey.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsView()
        {
            #region Arrange
            SetupDataForSingleAnswer();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<SurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Questions.Count);
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.IsNotNull(result.SurveyResponse);
            Assert.IsTrue(result.SurveyResponse.IsTransient());
            #endregion Assert		
        }
        #endregion Create Get Tests
        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostRedirectsWhenSurveyNotFoundOrNotActive1()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            surveys[2].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            Controller.Create(3, new SurveyResponse(),new QuestionAnswerParameter[0] )
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey not found or not active.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenSurveyNotFoundOrNotActive2()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            surveys[2].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            Controller.Create(4, new SurveyResponse(), new QuestionAnswerParameter[0])
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey not found or not active.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestCreatePostThrowsExceptionIfPassedQuestionIdNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
                var qAndA = new QuestionAnswerParameter[1];
                qAndA[0] = new QuestionAnswerParameter();
                qAndA[0].QuestionId = 99;
                new FakeSurveys(3, SurveyRepository);
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Question not found.\n SurveyId: 2\n QuestionId: 99\n Question #: 0", ex.Message);
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestCreatePostThrowsExceptionIfRelatedCategoryNotActive()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
                var qAndA = new QuestionAnswerParameter[1];
                qAndA[0] = new QuestionAnswerParameter();
                qAndA[0].QuestionId = 1;
                new FakeSurveys(3, SurveyRepository);
                var questions = new List<Question>();
                questions.Add(CreateValidEntities.Question(1));
                questions[0].Category = CreateValidEntities.Category(1);
                questions[0].Category.IsActive = false;
                new FakeQuestions(0, QuestionRepository, questions);
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Related Category is not active for question Id 1", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestCreatePostThrowsExceptionIfRelatedCategoryNotCurrent()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
                var qAndA = new QuestionAnswerParameter[1];
                qAndA[0] = new QuestionAnswerParameter();
                qAndA[0].QuestionId = 1;
                new FakeSurveys(3, SurveyRepository);
                var questions = new List<Question>();
                questions.Add(CreateValidEntities.Question(1));
                questions[0].Category = CreateValidEntities.Category(1);
                questions[0].Category.IsCurrentVersion = false;
                questions[0].Category.IsActive = true;
                new FakeQuestions(0, QuestionRepository, questions);
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Related Category is not current version for question Id 1", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestCreatePostThrowsExceptionIfRelatedSurveyDoesNotMatch()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
                var qAndA = new QuestionAnswerParameter[1];
                qAndA[0] = new QuestionAnswerParameter();
                qAndA[0].QuestionId = 1;
                new FakeSurveys(3, SurveyRepository);
                var questions = new List<Question>();
                questions.Add(CreateValidEntities.Question(1));
                questions[0].Category = CreateValidEntities.Category(1);
                questions[0].Survey = SurveyRepository.GetNullableById(3);
                questions[0].Category.IsActive = true;
                new FakeQuestions(0, QuestionRepository, questions);
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Related Survey does not match passed survey 3--2", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestCreatePostReturnsViewWhenAnswerIsInvalid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
            var qAndA = new QuestionAnswerParameter[1];
            qAndA[0] = new QuestionAnswerParameter();
            qAndA[0].QuestionId = 1;
            qAndA[0].BypassQuestion = false;
            
            var questions = new List<Question>();
            questions.Add(CreateValidEntities.Question(1));
            questions[0].Category = CreateValidEntities.Category(1);
            questions[0].Survey = CreateValidEntities.Survey(2);
            questions[0].Survey.SetIdTo(2);
            questions[0].Category.IsActive = true;
            questions[0].IsOpenEnded = false;
            new FakeQuestions(0, QuestionRepository, questions);

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i+1));
            }

            surveys[1].Questions.Add(QuestionRepository.GetNullableById(1));
            new FakeSurveys(0, SurveyRepository, surveys);


            var qAndAWithError = new QuestionAnswerParameter[1];
            qAndAWithError[0] = new QuestionAnswerParameter();
            qAndAWithError[0].QuestionId = 1;
            qAndAWithError[0].BypassQuestion = false;
            qAndAWithError[0].Invalid = true;
            qAndAWithError[0].Message = "Faked Error";

            ScoreService.Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything)).Return(qAndAWithError[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA)
                .AssertViewRendered()
                .WithViewData<SurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Faked Error");
            Assert.AreEqual("Please correct all errors and submit.", Controller.Message);
            Assert.IsNotNull(result);

            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostReturnsViewWhenAnswerCountDifferentFromQuestionCount()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
            var qAndA = new QuestionAnswerParameter[1];
            qAndA[0] = new QuestionAnswerParameter();
            qAndA[0].QuestionId = 1;
            qAndA[0].BypassQuestion = false;

            var questions = new List<Question>();
            questions.Add(CreateValidEntities.Question(1));
            questions[0].Category = CreateValidEntities.Category(1);
            questions[0].Survey = CreateValidEntities.Survey(2);
            questions[0].Survey.SetIdTo(2);
            questions[0].Category.IsActive = true;
            questions[0].IsOpenEnded = false;
            questions.Add(CreateValidEntities.Question(2));
            questions[1].Category = CreateValidEntities.Category(1);
            questions[1].Survey = CreateValidEntities.Survey(2);
            questions[1].Survey.SetIdTo(2);
            questions[1].Category.IsActive = true;
            questions[1].IsOpenEnded = false;
            new FakeQuestions(0, QuestionRepository, questions);

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }

            surveys[1].Questions.Add(QuestionRepository.GetNullableById(1));
            surveys[1].Questions.Add(QuestionRepository.GetNullableById(2));
            new FakeSurveys(0, SurveyRepository, surveys);


            var qAndAWithError = new QuestionAnswerParameter[1];
            qAndAWithError[0] = new QuestionAnswerParameter();
            qAndAWithError[0].QuestionId = 1;
            qAndAWithError[0].BypassQuestion = false;
            qAndAWithError[0].Invalid = false;

            ScoreService.Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything)).Return(qAndAWithError[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA)
                .AssertViewRendered()
                .WithViewData<SurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("You must answer all survey questions.");
            Assert.AreEqual("You must answer all survey questions.", Controller.Message);
            Assert.IsNotNull(result);

            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostCanBypassQuestions()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
            var qAndA = new QuestionAnswerParameter[1];
            qAndA[0] = new QuestionAnswerParameter();
            qAndA[0].QuestionId = 1;
            qAndA[0].BypassQuestion = true;

            var questions = new List<Question>();
            questions.Add(CreateValidEntities.Question(1));
            questions[0].Category = CreateValidEntities.Category(1);
            questions[0].Survey = CreateValidEntities.Survey(2);
            questions[0].Survey.SetIdTo(2);
            questions[0].Category.IsActive = true;
            questions[0].IsOpenEnded = false;
            new FakeQuestions(0, QuestionRepository, questions);

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }

            surveys[1].Questions.Add(QuestionRepository.GetNullableById(1));
            new FakeSurveys(0, SurveyRepository, surveys);


            var qAndAWithError = new QuestionAnswerParameter[1];
            qAndAWithError[0] = new QuestionAnswerParameter();
            qAndAWithError[0].QuestionId = 1;
            qAndAWithError[0].BypassQuestion = true;
            qAndAWithError[0].Invalid = true;
            qAndAWithError[0].Message = "Faked Error";

            ScoreService.Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything)).Return(qAndAWithError[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.Results(1));
            #endregion Act

            #region Assert

            Assert.AreEqual("SurveyResponse Created Successfully", Controller.Message);
            ScoreService.AssertWasCalled(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything));
            ScoreService.AssertWasCalled(a => a.CalculateScores(Arg<IRepository>.Is.Anything, Arg<SurveyResponse>.Is.Anything));
            SurveyResponseRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            var args = (SurveyResponse) SurveyResponseRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything))[0][0];
            Assert.AreEqual("SID1", args.StudentId);
            Assert.AreEqual("UserName", args.UserId);
            Assert.AreEqual(1, args.Answers.Count);

            Assert.AreEqual(null, args.Answers[0].OpenEndedAnswer);
            Assert.AreEqual(null, args.Answers[0].Response);
            Assert.AreEqual(0, args.Answers[0].Score);
            Assert.AreEqual(true, args.Answers[0].BypassScore);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWhenQuestionsAnswered1()
        {
            #region Arrange
            new FakeResponses(3, ResponseRepository);

            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
            var qAndA = new QuestionAnswerParameter[1];
            qAndA[0] = new QuestionAnswerParameter();
            qAndA[0].QuestionId = 1;
            qAndA[0].BypassQuestion = false;

            var questions = new List<Question>();
            questions.Add(CreateValidEntities.Question(1));
            questions[0].Category = CreateValidEntities.Category(1);
            questions[0].Survey = CreateValidEntities.Survey(2);
            questions[0].Survey.SetIdTo(2);
            questions[0].Category.IsActive = true;
            questions[0].IsOpenEnded = false;
            new FakeQuestions(0, QuestionRepository, questions);

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }

            surveys[1].Questions.Add(QuestionRepository.GetNullableById(1));
            new FakeSurveys(0, SurveyRepository, surveys);


            var qAndAWithError = new QuestionAnswerParameter[1];
            qAndAWithError[0] = new QuestionAnswerParameter();
            qAndAWithError[0].QuestionId = 1;
            qAndAWithError[0].BypassQuestion = false;
            qAndAWithError[0].Invalid = false;
            qAndAWithError[0].Message = null;
            qAndAWithError[0].OpenEndedNumericAnswer = null;
            qAndAWithError[0].ResponseId = 2;
            qAndAWithError[0].Score = 99;

            ScoreService.Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything)).Return(qAndAWithError[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.Results(1));
            #endregion Act

            #region Assert

            Assert.AreEqual("SurveyResponse Created Successfully", Controller.Message);
            ScoreService.AssertWasCalled(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything));
            ScoreService.AssertWasCalled(a => a.CalculateScores(Arg<IRepository>.Is.Anything, Arg<SurveyResponse>.Is.Anything));
            SurveyResponseRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            var args = (SurveyResponse)SurveyResponseRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything))[0][0];
            Assert.AreEqual("SID1", args.StudentId);
            Assert.AreEqual("UserName", args.UserId);
            Assert.AreEqual(1, args.Answers.Count);

            Assert.AreEqual(null, args.Answers[0].OpenEndedAnswer);
            Assert.AreEqual(2, args.Answers[0].Response.Id);
            Assert.AreEqual(99, args.Answers[0].Score);
            Assert.AreEqual(false, args.Answers[0].BypassScore);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWhenQuestionsAnswered2()
        {
            #region Arrange
            new FakeResponses(3, ResponseRepository);

            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
            var qAndA = new QuestionAnswerParameter[1];
            qAndA[0] = new QuestionAnswerParameter();
            qAndA[0].QuestionId = 1;
            qAndA[0].BypassQuestion = false;

            var questions = new List<Question>();
            questions.Add(CreateValidEntities.Question(1));
            questions[0].Category = CreateValidEntities.Category(1);
            questions[0].Survey = CreateValidEntities.Survey(2);
            questions[0].Survey.SetIdTo(2);
            questions[0].Category.IsActive = true;
            questions[0].IsOpenEnded = true;
            new FakeQuestions(0, QuestionRepository, questions);

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }

            surveys[1].Questions.Add(QuestionRepository.GetNullableById(1));
            new FakeSurveys(0, SurveyRepository, surveys);


            var qAndAWithError = new QuestionAnswerParameter[1];
            qAndAWithError[0] = new QuestionAnswerParameter();
            qAndAWithError[0].QuestionId = 1;
            qAndAWithError[0].BypassQuestion = false;
            qAndAWithError[0].Invalid = false;
            qAndAWithError[0].Message = null;
            qAndAWithError[0].OpenEndedNumericAnswer = null;
            qAndAWithError[0].ResponseId = 2;
            qAndAWithError[0].Score = 99;
            qAndAWithError[0].Answer = "87654";

            ScoreService.Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything)).Return(qAndAWithError[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.Results(1));
            #endregion Act

            #region Assert

            Assert.AreEqual("SurveyResponse Created Successfully", Controller.Message);
            ScoreService.AssertWasCalled(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything));
            ScoreService.AssertWasCalled(a => a.CalculateScores(Arg<IRepository>.Is.Anything, Arg<SurveyResponse>.Is.Anything));
            SurveyResponseRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            var args = (SurveyResponse)SurveyResponseRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything))[0][0];
            Assert.AreEqual("SID1", args.StudentId);
            Assert.AreEqual("UserName", args.UserId);
            Assert.AreEqual(1, args.Answers.Count);

            Assert.AreEqual(null, args.Answers[0].OpenEndedAnswer);
            Assert.AreEqual(2, args.Answers[0].Response.Id);
            Assert.AreEqual(99, args.Answers[0].Score);
            Assert.AreEqual(false, args.Answers[0].BypassScore);
            Assert.AreEqual("87654", args.Answers[0].OpenEndedStringAnswer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWhenQuestionsAnswered3()
        {
            #region Arrange
            new FakeResponses(3, ResponseRepository);

            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
            var qAndA = new QuestionAnswerParameter[1];
            qAndA[0] = new QuestionAnswerParameter();
            qAndA[0].QuestionId = 1;
            qAndA[0].BypassQuestion = false;

            var questions = new List<Question>();
            questions.Add(CreateValidEntities.Question(1));
            questions[0].Category = CreateValidEntities.Category(1);
            questions[0].Survey = CreateValidEntities.Survey(2);
            questions[0].Survey.SetIdTo(2);
            questions[0].Category.IsActive = true;
            questions[0].IsOpenEnded = true;
            questions[0].OpenEndedQuestionType = (int) QuestionType.TimeRange;
            new FakeQuestions(0, QuestionRepository, questions);

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }

            surveys[1].Questions.Add(QuestionRepository.GetNullableById(1));
            new FakeSurveys(0, SurveyRepository, surveys);


            var qAndAWithError = new QuestionAnswerParameter[1];
            qAndAWithError[0] = new QuestionAnswerParameter();
            qAndAWithError[0].QuestionId = 1;
            qAndAWithError[0].BypassQuestion = false;
            qAndAWithError[0].Invalid = false;
            qAndAWithError[0].Message = null;
            qAndAWithError[0].OpenEndedNumericAnswer = null;
            qAndAWithError[0].ResponseId = 2;
            qAndAWithError[0].Score = 99;
            qAndAWithError[0].Answer = "Now";
            qAndAWithError[0].AnswerRange = "Then";

            ScoreService.Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything)).Return(qAndAWithError[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.Results(1));
            #endregion Act

            #region Assert

            Assert.AreEqual("SurveyResponse Created Successfully", Controller.Message);
            ScoreService.AssertWasCalled(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything));
            ScoreService.AssertWasCalled(a => a.CalculateScores(Arg<IRepository>.Is.Anything, Arg<SurveyResponse>.Is.Anything));
            SurveyResponseRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            var args = (SurveyResponse)SurveyResponseRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything))[0][0];
            Assert.AreEqual("SID1", args.StudentId);
            Assert.AreEqual("UserName", args.UserId);
            Assert.AreEqual(1, args.Answers.Count);

            Assert.AreEqual(null, args.Answers[0].OpenEndedAnswer);
            Assert.AreEqual(2, args.Answers[0].Response.Id);
            Assert.AreEqual(99, args.Answers[0].Score);
            Assert.AreEqual(false, args.Answers[0].BypassScore);
            Assert.AreEqual("Now_Then", args.Answers[0].OpenEndedStringAnswer);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostServiceParameterCheck()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
            var qAndA = new QuestionAnswerParameter[1];
            qAndA[0] = new QuestionAnswerParameter();
            qAndA[0].QuestionId = 1;
            qAndA[0].BypassQuestion = false;

            var questions = new List<Question>();
            questions.Add(CreateValidEntities.Question(1));
            questions[0].Category = CreateValidEntities.Category(1);
            questions[0].Survey = CreateValidEntities.Survey(2);
            questions[0].Survey.SetIdTo(2);
            questions[0].Category.IsActive = true;
            questions[0].IsOpenEnded = false;
            questions.Add(CreateValidEntities.Question(2));
            questions[1].Category = CreateValidEntities.Category(1);
            questions[1].Survey = CreateValidEntities.Survey(2);
            questions[1].Survey.SetIdTo(2);
            questions[1].Category.IsActive = true;
            questions[1].IsOpenEnded = false;
            new FakeQuestions(0, QuestionRepository, questions);

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }

            surveys[1].Questions.Add(QuestionRepository.GetNullableById(1));
            surveys[1].Questions.Add(QuestionRepository.GetNullableById(2));
            new FakeSurveys(0, SurveyRepository, surveys);


            var qAndAWithError = new QuestionAnswerParameter[1];
            qAndAWithError[0] = new QuestionAnswerParameter();
            qAndAWithError[0].QuestionId = 1;
            qAndAWithError[0].BypassQuestion = false;
            qAndAWithError[0].Invalid = false;

            ScoreService.Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything)).Return(qAndAWithError[0]).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(2, CreateValidEntities.SurveyResponse(1), qAndA)
                .AssertViewRendered()
                .WithViewData<SurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("You must answer all survey questions.");
            Assert.AreEqual("You must answer all survey questions.", Controller.Message);
            var args =
                ScoreService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, (args[0] as IQueryable<Question>).Count());
            Assert.AreEqual("Name1", (args[0] as IQueryable<Question>).ElementAt(0).Name);
            Assert.AreEqual("Name2", (args[0] as IQueryable<Question>).ElementAt(1).Name);
            Assert.IsNotNull(result);

            #endregion Assert
        }
        #endregion Create Post Tests
        #endregion Create Tests
    }
}