using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    public partial class SurveyResponseControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexReturnView1()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i+1));
            }
            surveys[1].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {RoleNames.Admin, RoleNames.User});
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<ActiveSurveyViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsPublic);
            Assert.AreEqual(2, result.Surveys.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexReturnView2()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            surveys[1].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { RoleNames.Admin });
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<ActiveSurveyViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsPublic);
            Assert.AreEqual(2, result.Surveys.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnView3()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            surveys[1].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { RoleNames.User });
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<ActiveSurveyViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsPublic);
            Assert.AreEqual(2, result.Surveys.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnView4()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            surveys[1].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""});
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<ActiveSurveyViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsPublic);
            Assert.AreEqual(2, result.Surveys.Count());
            #endregion Assert
        }
        #endregion Index Tests

        #region Details Tests

        [TestMethod]
        public void TestDetailsRedirectsWhenSurveyResponseNotFound()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Response Details Not Found.", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDetailsReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {RoleNames.Admin});
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Details(3)
                .AssertViewRendered()
                .WithViewData<SurveyReponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UserId3", result.SurveyResponse.UserId);
            Assert.IsFalse(result.FromYourDetails);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDetailsReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            var survey = CreateValidEntities.Survey(9);
            var categories = new List<Category>();
            for (int i = 0; i < 5; i++)
            {
                categories.Add(CreateValidEntities.Category(i+1));
            }
            categories[4].DoNotUseForCalculations = true;
            new FakeCategories(0, CategoryRepository, categories);
            
            var categoryTotalMaxScores = new List<CategoryTotalMaxScore>();

            var answers = new List<Answer>();
            var count = 0;
            foreach (var category in CategoryRepository.Queryable)
            {                                
                categoryTotalMaxScores.Add(CreateValidEntities.CategoryTotalMaxScore(category.Id));

                for (int i = 0; i < 3; i++)
                {
                    count++;
                    var answer = CreateValidEntities.Answer(count);
                    answer.Category = category;
                    answers.Add(answer);
                }
            }


            survey.Categories = CategoryRepository.Queryable.ToList();
            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].Survey = survey;
            surveyResponses[0].Answers = answers;

            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);

            var scoreMax = 0;
            foreach (var categoryTotalMaxScore in categoryTotalMaxScores)
            {
                scoreMax++;
                categoryTotalMaxScore.TotalMaxScore = scoreMax*25;
                CategoryTotalMaxScore score = categoryTotalMaxScore;
                CategoryTotalMaxScoreRepository.Expect(a => a.GetNullableById(score.Id)).Return(
                    categoryTotalMaxScore);
            }

            #endregion Arrange

            #region Act
            var result = Controller.Details(1)
                .AssertViewRendered()
                .WithViewData<SurveyReponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Scores.Count);
            Assert.AreEqual(24, result.Scores[0].Percent);
            Assert.AreEqual(6, result.Scores[0].TotalScore);
            Assert.AreEqual(25, result.Scores[0].MaxScore);
            Assert.AreEqual(30, result.Scores[1].Percent);
            Assert.AreEqual(32, result.Scores[2].Percent);
            Assert.AreEqual(33, result.Scores[3].Percent);
            Assert.AreEqual("UserId1", result.SurveyResponse.UserId);
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Details(3, true)
                .AssertViewRendered()
                .WithViewData<SurveyReponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UserId3", result.SurveyResponse.UserId);
            Assert.IsTrue(result.FromYourDetails);
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "UserId3");
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Details(3, true)
                .AssertViewRendered()
                .WithViewData<SurveyReponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UserId3", result.SurveyResponse.UserId);
            Assert.IsTrue(result.FromYourDetails);
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsRedirectsWhenNotAuthorized()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "nomatch");
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.Details(3, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert

            #endregion Assert
        }
        #endregion Details Tests
    }
}
