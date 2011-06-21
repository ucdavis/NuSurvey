using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
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
using UCDArch.Web.Attributes;
using NuSurvey.Tests.Core.Helpers;

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
        
        #endregion Create Post Tests
        #endregion Create Tests
    }
}