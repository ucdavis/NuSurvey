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
using UCDArch.Testing;
using UCDArch.Web.ActionResults;


namespace NuSurvey.Tests.ControllerTests.CategoryControllerTests
{
    public partial class CategoryControllerTests
    {
        #region ReOrder Tests
        #region ReOrder Get Tests

        [TestMethod]
        public void TestReOrderGetRedirectsIfSurveyNotFound()
        {
            #region Arrange
            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.ReOrder(4)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert		
        }


        [TestMethod]
        public void TestReOrderGetReturnsViewWhenSurveyFound()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i+1));  
            }
            var categories = new List<Category>();
            for (int i = 0; i < 4; i++)
            {
                categories.Add(CreateValidEntities.Category(i+1));
                categories[i].Survey = surveys[2];
                categories[i].Rank = 10 - i;
            }
            categories[1].IsCurrentVersion = false;
            surveys[2].Categories = categories;

            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            var result = Controller.ReOrder(3)
                .AssertViewRendered()
                .WithViewData<CategoryListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Categories.Count());
            Assert.AreEqual(3, result.Survey.Id);
            Assert.AreEqual("Name4", result.Categories.ElementAt(0).Name);
            Assert.AreEqual("Name3", result.Categories.ElementAt(1).Name);
            Assert.AreEqual("Name1", result.Categories.ElementAt(2).Name);
            #endregion Assert		
        }
        #endregion ReOrder Get Tests
        #region ReOrder Post Tests
        [TestMethod]
        public void TestReOrderPostReturnsJsonIfSurveyNotFound()
        {
            #region Arrange
            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(3, SurveyRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ReOrder(4, new int[0])
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Data);
            #endregion Assert
        }


        [TestMethod]
        public void TestReOrderPostReturnsFalseIfTheSurveyHasErrors()
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
                categories[i].SetIdTo(i + 1);
            }
            categories[1].IsCurrentVersion = false;
            surveys[2].Categories = categories;
            surveys[2].QuizType = null; //Force Error

            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            var result = Controller.ReOrder(3, new []{1,3,4})
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.Data);
            Controller.ModelState.AssertErrorsAre("QuizType: The QuizType field is required.");
            #endregion Assert		
        }

        /// <summary>
        /// Tests the FieldToTest with A value of TestValue does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReOrderThorowsExceptionIfTableOrderIdIsNotFound()
        {
            var thisFar = false;
            try
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
                    categories[i].SetIdTo(i + 1);
                }
                categories[1].IsCurrentVersion = false;
                surveys[2].Categories = categories;

                var fakeSurveys = new FakeSurveys();
                fakeSurveys.Records(0, SurveyRepository, surveys);
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.ReOrder(3, new  []{ 1, 99, 4 });
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                Assert.IsTrue(thisFar);
                Assert.AreEqual("Sequence contains no elements", ex.Message);
                throw;
            }	
        }

        [TestMethod]
        public void TestReOrderPostReturnsTrueAndSavesNewRankIfTheSurveyIsValid()
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
                categories[i].SetIdTo(i + 1);
            }
            categories[1].IsCurrentVersion = false;
            surveys[2].Categories = categories;

            var fakeSurveys = new FakeSurveys();
            fakeSurveys.Records(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            var result = Controller.ReOrder(3, new []{ 3, 1, 4 })
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.Data);
            SurveyRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Survey>.Is.Anything));
            var args = (Survey)SurveyRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Survey>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(1, args.Categories.Where(a => a.Id == 3).Single().Rank);
            Assert.AreEqual(2, args.Categories.Where(a => a.Id == 1).Single().Rank);
            Assert.AreEqual(3, args.Categories.Where(a => a.Id == 4).Single().Rank);
            #endregion Assert
        }

        #endregion ReOrder Post Tests
        #endregion ReOrder Tests
    }
}
