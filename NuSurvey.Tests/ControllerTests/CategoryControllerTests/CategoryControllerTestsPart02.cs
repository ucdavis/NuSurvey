using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;

namespace NuSurvey.Tests.ControllerTests.CategoryControllerTests
{
    public partial class CategoryControllerTests
    {
        #region Create Tests
        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetRedirectsToIndexIfSurveyIdNotFound()
        {
            #region Arrange
            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Create(4)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        
        [TestMethod]
        public void TestCreateGetReturnsViewWithExpectedValues1()
        {
            #region Arrange
            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(3, SurveyRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Create(3)
                .AssertViewRendered()
                .WithViewData<CategoryViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Survey.Id);
            Assert.IsTrue(result.Category.IsTransient());
            Assert.AreEqual(1, result.Category.Rank);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetReturnsViewWithExpectedValues2()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            var categories = new List<Category>();
            for (int i = 0; i < 4; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].Survey = surveys[2];
                categories[i].Rank = 10 - i;
            }
            categories[1].IsCurrentVersion = false;
            surveys[2].Categories = categories;

            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            var result = Controller.Create(3)
                .AssertViewRendered()
                .WithViewData<CategoryViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Survey.Id);
            Assert.IsTrue(result.Category.IsTransient());
            Assert.AreEqual(11, result.Category.Rank);
            #endregion Assert
        }
        #endregion Create Get Tests
        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostRedirectsToIndexIfSurveyIdNotFound()
        {
            #region Arrange
            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Create(4, new Category())
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostReturnsViewIfCategoryInvalid()
        {
            #region Arrange
            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(3, SurveyRepository);
            var category = CreateValidEntities.Category(2);
            category.Encouragement = null; //Force Fail
            category.Survey = null;
            category.Rank = 999;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, category)
                .AssertViewRendered()
                .WithViewData<CategoryViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("The Encouragement field is required.");
            Assert.AreEqual(DateTime.Now.Date, result.Category.CreateDate.Date); 
            Assert.AreEqual(DateTime.Now.Date, result.Category.LastUpdate.Date);
            Assert.AreEqual(3, result.Category.Survey.Id);
            Assert.AreNotEqual(999, result.Category.Rank);
            CategoryRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreatePostRedirectsToEditWhenSuccessful()
        {
            #region Arrange
            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(3, SurveyRepository);
            var category = CreateValidEntities.Category(2);
            category.Survey = null;
            category.Rank = 999;

            
            #endregion Arrange

            #region Act
            Controller.Create(3, category)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything));
            var args = (Category) CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Affirmation2", args.Affirmation);
            Assert.AreEqual("Encouragement2", args.Encouragement);
            Assert.AreEqual("Name2", args.Name);
            Assert.AreEqual(3, args.Survey.Id);
            Assert.IsFalse(args.IsActive);
            Assert.IsTrue(args.IsCurrentVersion);
            Assert.IsNull(args.PreviousVersion);
            Assert.AreEqual(DateTime.Now.Date, args.CreateDate.Date);
            Assert.AreEqual(DateTime.Now.Date, args.LastUpdate.Date);
            Assert.IsFalse(args.DoNotUseForCalculations);
            Assert.AreEqual(1, args.Rank);
            #endregion Assert		
        }
        #endregion Create Post Tests
        #endregion Create Tests
    }
}
