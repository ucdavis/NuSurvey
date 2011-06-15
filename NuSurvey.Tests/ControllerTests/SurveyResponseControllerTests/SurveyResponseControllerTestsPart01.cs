using System;
using System.Collections.Generic;
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
    }
}
