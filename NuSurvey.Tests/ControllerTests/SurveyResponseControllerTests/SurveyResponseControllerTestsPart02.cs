using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web.Helpers;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;

namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    public partial class SurveyResponseControllerTests
    {
        #region StartSurvey Tests
        #region StartSurvey Get Tests

        [TestMethod]
        public void TestStartSurveyRedirectsWhenSurveyNotFoundOrNotActive1()
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
        public void TestStartSurveyRedirectsWhenSurveyNotFoundOrNotActive2()
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
        public void TestStartSurveyRedirectsWhenNotEnoughCategories()
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
        public void TestStartSurveyReturnsViewIfPendingSurveyResponseLinkedToOlderVersion()
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
        public void TestStartSurveyReturnsViewIfPendingSurveyResponse()
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
        public void TestStartSurveyReturnsViewWhenNoPendingSurvey()
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
        public void TestStartSurveyReturnsViewWithExpectedData1()
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
        public void TestStartSurveyReturnsViewWithExpectedData2()
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
        #endregion StartSurvey Tests
    }
}
