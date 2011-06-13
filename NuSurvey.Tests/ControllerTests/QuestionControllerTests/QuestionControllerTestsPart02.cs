using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace NuSurvey.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        #region Create Tests
        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetRedirectsIfSurveyNotFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Create(4, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Not Found", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsIfSurveyNotFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Create(4, 3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Not Found", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsView1()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, null)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual(3, result.Categories.Count());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 2).Any());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 4).Any());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 5).Any());
            Assert.AreEqual(0, result.Categories.Where(a => !a.IsCurrentVersion).Count());
            Assert.IsNotNull(result.Responses);
            Assert.AreEqual(0, result.Responses.Count);
            Assert.IsNull(result.Category);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetReturnsView2()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 11)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual(3, result.Categories.Count());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 2).Any());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 4).Any());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 5).Any());
            Assert.AreEqual(0, result.Categories.Where(a => !a.IsCurrentVersion).Count());
            Assert.IsNotNull(result.Responses);
            Assert.AreEqual(0, result.Responses.Count);
            Assert.IsNull(result.Category);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsView3()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 10)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual(3, result.Categories.Count());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 2).Any());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 4).Any());
            Assert.IsTrue(result.Categories.Where(a => a.Id == 5).Any());
            Assert.AreEqual(0, result.Categories.Where(a => !a.IsCurrentVersion).Count());
            Assert.IsNotNull(result.Responses);
            Assert.AreEqual(0, result.Responses.Count);
            Assert.IsNotNull(result.Category);
            Assert.AreEqual("Name10", result.Category.Name);
            #endregion Assert
        }

        #endregion Create Get Tests
        #endregion Create Tests
    }
}
