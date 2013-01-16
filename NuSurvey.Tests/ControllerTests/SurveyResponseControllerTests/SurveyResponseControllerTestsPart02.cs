using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    public partial class SurveyResponseControllerTests
    {
        #region StartSurvey Tests
        #region StartSurvey Get Tests

        [TestMethod]
        public void TestStartSurveyGetRedirectsWhenSurveyNotFoundOrNotActive1()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i+1));
            }
            surveys[2].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            Controller.StartSurvey(3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey not found or not active.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestStartSurveyGetRedirectsWhenSurveyNotFoundOrNotActive2()
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
            Controller.StartSurvey(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey not found or not active.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestStartSurveyGetRedirectsWhenNotEnoughCategories()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 5; i++)
            {
                categories.Add(CreateValidEntities.Category(i+1));
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
            Controller.StartSurvey(1)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey does not have enough active categories to complete survey.", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestStartSurveyGetReturnsViewIfPendingSurveyResponseLinkedToOlderVersion()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
            var categories = new List<Category>();
            for (int i = 0; i < 4; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].IsActive = true;
            }
            categories[0].IsCurrentVersion = false;
            new FakeCategories(0, CategoryRepository, categories);

            var answer = CreateValidEntities.Answer(1); //Not current answer
            answer.Category = CategoryRepository.GetNullableById(1);
            var surveys = new List<Survey>();
            surveys.Add(CreateValidEntities.Survey(1));
            surveys[0].Categories = CategoryRepository.Queryable.ToList();
            new FakeSurveys(0, SurveyRepository, surveys);

            var surveyReponses = new List<SurveyResponse>();
            surveyReponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyReponses[0].UserId = "test@testy.com";
            surveyReponses[0].IsPending = true;
            surveyReponses[0].Answers.Add(answer);
            surveyReponses[0].Survey = SurveyRepository.GetNullableById(1);

            new FakeSurveyResponses(0, SurveyResponseRepository, surveyReponses);
            new FakeCategoryTotalMaxScore(5, CategoryTotalMaxScoreRepository);
            #endregion Arrange

            #region Act
            var result = Controller.StartSurvey(1)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("The unfinished survey's questions have been modifed. Unable to continue. Delete survey and start again.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CannotContinue);
            #endregion Assert		
        }

        [TestMethod]
        public void TestStartSurveyGetReturnsViewIfPendingSurveyResponse()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
            var categories = new List<Category>();
            for (int i = 0; i < 4; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].IsActive = true;
            }
            //categories[0].IsCurrentVersion = false;
            new FakeCategories(0, CategoryRepository, categories);

            var answer = CreateValidEntities.Answer(1); //Not current answer
            answer.Category = CategoryRepository.GetNullableById(1);
            var surveys = new List<Survey>();
            surveys.Add(CreateValidEntities.Survey(1));
            surveys[0].Categories = CategoryRepository.Queryable.ToList();
            new FakeSurveys(0, SurveyRepository, surveys);

            var surveyReponses = new List<SurveyResponse>();
            surveyReponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyReponses[0].UserId = "test@testy.com";
            surveyReponses[0].IsPending = true;
            surveyReponses[0].Answers.Add(answer);
            surveyReponses[0].Survey = SurveyRepository.GetNullableById(1);

            new FakeSurveyResponses(0, SurveyResponseRepository, surveyReponses);
            new FakeCategoryTotalMaxScore(5, CategoryTotalMaxScoreRepository);
            #endregion Arrange

            #region Act
            var result = Controller.StartSurvey(1)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Unfinished survey found.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.CannotContinue);
            #endregion Assert
        }


        [TestMethod]
        public void TestStartSurveyGetReturnsViewWhenNoPendingSurvey()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
            var categories = new List<Category>();
            for (int i = 0; i < 4; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].IsActive = true;
            }
            //categories[0].IsCurrentVersion = false;
            new FakeCategories(0, CategoryRepository, categories);

            var answer = CreateValidEntities.Answer(1); //Not current answer
            answer.Category = CategoryRepository.GetNullableById(1);
            var surveys = new List<Survey>();
            surveys.Add(CreateValidEntities.Survey(1));
            surveys[0].Categories = CategoryRepository.Queryable.ToList();
            new FakeSurveys(0, SurveyRepository, surveys);

            var surveyReponses = new List<SurveyResponse>();
            surveyReponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyReponses[0].UserId = "test@testy.com";
            surveyReponses[0].IsPending = false;
            surveyReponses[0].Answers.Add(answer);
            surveyReponses[0].Survey = SurveyRepository.GetNullableById(1);

            new FakeSurveyResponses(0, SurveyResponseRepository, surveyReponses);
            new FakeCategoryTotalMaxScore(5, CategoryTotalMaxScoreRepository);
            #endregion Arrange

            #region Act
            var result = Controller.StartSurvey(1)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.CannotContinue);
            #endregion Assert	
        }


        [TestMethod]
        public void TestStartSurveyGetReturnsViewWithExpectedData1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
            SetupDataForSingleAnswer();

            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            var result = Controller.StartSurvey(1)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.PendingSurveyResponseExists);
            Assert.AreEqual(0, result.AnsweredQuestions);
            Assert.AreEqual(4, result.CurrentQuestion.Id);
            Assert.AreEqual(6, result.TotalActiveQuestions);
            #endregion Assert		
        }

        [TestMethod]
        public void TestStartSurveyGetReturnsViewWithExpectedData2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1); //Not current answer
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.StartSurvey(1)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PendingSurveyResponseExists);
            Assert.AreEqual(1, result.AnsweredQuestions);
            Assert.AreEqual(5, result.CurrentQuestion.Id);
            Assert.AreEqual(6, result.TotalActiveQuestions);
            #endregion Assert
        }
        #endregion StartSurvey Get Tests
        #region StartSurvey Post Tests

        [TestMethod]
        public void TestStartSurveyPostRedirectsIfSurveyNotFoundOrActive1()
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
            Controller.StartSurvey(3, new SurveyResponse(), null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey not found or not active.", Controller.Message);
            #endregion Assert			
        }

        [TestMethod]
        public void TestStartSurveyPostRedirectsWhenSurveyNotFoundOrNotActive2()
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
            Controller.StartSurvey(4, new SurveyResponse(), null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey not found or not active.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestStartSurveyPostReturnsViewWhenInvalid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me@test.com");
            new FakeSurveys(3, SurveyRepository);
            var surveyResponse = CreateValidEntities.SurveyResponse(1);
            surveyResponse.StudentId = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.StartSurvey(2, surveyResponse, null)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("The Name field is required."); 
            Assert.AreEqual("Please correct errors to continue", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestStartSurveyPostWithValidDataRedirectsAndSaves()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me@test.com");
            SetupDataForSingleAnswer();
            var surveyResponse = CreateValidEntities.SurveyResponse(99);
            surveyResponse.SetIdTo(99);
            #endregion Arrange

            #region Act
            var result = Controller.StartSurvey(1, surveyResponse, null)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.AnswerNext(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(99, result.RouteValues["id"]);
            SurveyResponseRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            var args = (SurveyResponse) SurveyResponseRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Survey.Name);
            Assert.AreEqual("me@test.com", args.UserId);
            Assert.IsTrue(args.IsPending);
            Assert.AreEqual(0, args.Answers.Count);
            Assert.AreEqual("SID99", args.StudentId);
            #endregion Assert		
        }
        #endregion StartSurvey Post Tests
        #endregion StartSurvey Tests
    }
}
