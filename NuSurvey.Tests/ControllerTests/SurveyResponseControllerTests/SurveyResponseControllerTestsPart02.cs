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
        public void TestStartSurveyRedirectsIfPendingSurveyResponseLinkedToOlderVersion()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].IsActive = true;
            }
            new FakeCategories(0, CategoryRepository, categories);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion StartSurvey Get Tests
        #endregion StartSurvey Tests
    }
}
