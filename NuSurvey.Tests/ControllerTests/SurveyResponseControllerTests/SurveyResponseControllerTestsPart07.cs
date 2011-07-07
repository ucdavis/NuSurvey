using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    public partial class SurveyResponseControllerTests
    {
        #region Results Tests

        [TestMethod]
        public void TestResultsRedirectsIfSurveyResponseNotFound()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.Results(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not Found", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestResultsRedirectsWhenNotAdminAndNotAuthorized1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "nomatch@test.com");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i+1));
                surveyResponses[i].UserId = "match@test.com";
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.Results(2)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert

            #endregion Assert
        }

        [TestMethod]
        public void TestResultsRedirectsWhenNotAdminAndNotAuthorized2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "nomatch@test.com");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].UserId = "match@test.com";
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.Results(2)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert

            #endregion Assert
        }

        [TestMethod]
        public void TestResultsReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "nomatch@test.com");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].UserId = "match@test.com";
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.Results(2)
                .AssertViewRendered()
                .WithViewData<ResultsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SID2", result.SurveyResponse.StudentId);
            Assert.IsTrue(result.ShowPdfPrint);
            #endregion Assert
        }

        [TestMethod]
        public void TestResultsReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].UserId = "match@test.com";
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.Results(2)
                .AssertViewRendered()
                .WithViewData<ResultsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SID2", result.SurveyResponse.StudentId);
            Assert.IsTrue(result.ShowPdfPrint);
            #endregion Assert
        }

        [TestMethod]
        public void TestResultsReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "match@test.com");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].UserId = "match@test.com";
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.Results(2)
                .AssertViewRendered()
                .WithViewData<ResultsViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SID2", result.SurveyResponse.StudentId);
            Assert.IsFalse(result.ShowPdfPrint);
            #endregion Assert
        }
        #endregion Results Tests
    }
}