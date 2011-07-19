using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing;
namespace NuSurvey.Tests.ControllerTests.CategoryGoalControllerTests
{
    public partial class CategoryGoalControllerTests
    {
        #region Details Tests

        [TestMethod]
        public void TestDetailsRedirectsIfCategoryGoalNotFound()
        {
            #region Arrange
            var fakeCategoryGoals = new FakeCategoryGoals();
            fakeCategoryGoals.Records(3, CategoryGoalRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("CategoryGoal Not Found", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestDetailsReturnsViewWithExpectedValuesWhenCategoryGoalFound()
        {
            #region Arrange
            var category = CreateValidEntities.Category(2);
            category.Survey = CreateValidEntities.Survey(3);
            var categoryGoals = new List<CategoryGoal>();
            for (int i = 0; i < 3; i++)
            {
                categoryGoals.Add(CreateValidEntities.CategoryGoal(i+1));
                categoryGoals[i].Category = category;
            }

            var fakeCategoryGoals = new FakeCategoryGoals();
            fakeCategoryGoals.Records(0, CategoryGoalRepository, categoryGoals);
            #endregion Arrange

            #region Act
            var result = Controller.Details(1)
                .AssertViewRendered()
                .WithViewData<CategoryGoalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name1", result.CategoryGoal.Name);
            Assert.AreEqual("Name2", result.Category.Name);
            Assert.AreEqual("Name3", result.Survey.Name);
            #endregion Assert		
        }
        #endregion Details Tests

        #region Create Tests
        #region Create Get Tests
        [TestMethod]
        public void TestCreateGetRedirectsWhenCategoryNotFound()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(5)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Category Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenCategoryNotCurrent()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Category is not Current", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsView()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3)
                .AssertViewRendered()
                .WithViewData<CategoryGoalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CategoryGoal.IsTransient());
            Assert.AreEqual("Name3", result.Category.Name);
            Assert.AreEqual("Name3", result.Survey.Name);
            #endregion Assert			
        }

        #endregion Create Get Tests

        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostRedirectsWhenCategoryNotFound()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(5, new CategoryGoal())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Category Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenCategoryNotCurrent()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(1, new CategoryGoal())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Category is not Current", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostWithInvalidCategoryGoalReturnsView()
        {
            #region Arrange
            SetupData1();
            var categoryGoalToCreate = CreateValidEntities.CategoryGoal(1);
            categoryGoalToCreate.Name = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, categoryGoalToCreate)
                .AssertViewRendered()
                .WithViewData<CategoryGoalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Category.Name);
            Assert.AreEqual("Name3", result.Survey.Name);
            Controller.ModelState.AssertErrorsAre("Name: The Goal field is required.");
            CategoryGoalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CategoryGoal>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostWithValidCategoryGoalRedirects()
        {
            #region Arrange
            SetupData1();
            var categoryGoalToCreate = CreateValidEntities.CategoryGoal(1);
            categoryGoalToCreate.Name = "New Goal";
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, categoryGoalToCreate)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            CategoryGoalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CategoryGoal>.Is.Anything));
            var args = (CategoryGoal) CategoryGoalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<CategoryGoal>.Is.Anything))[0][0]; 
            Assert.AreEqual("New Goal", args.Name);
            Assert.AreEqual("Name3", args.Category.Name);
            Assert.AreEqual("CategoryGoal Created Successfully", Controller.Message);
            #endregion Assert
        }
        #endregion Create Post Tests
        #endregion Create Tests

        #region Edit Tests
        #region Edit Get Tests
        [TestMethod]
        public void TestEditGetRedirectsWhenCategoryGoalNotFound()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Edit(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("CategoryGoal Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenCategoryNotCurrent()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Edit(1)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Related Category is not Current", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2)
                .AssertViewRendered()
                .WithViewData<CategoryGoalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.CategoryGoal.IsTransient());
            Assert.AreEqual("Name2", result.CategoryGoal.Name);
            Assert.AreEqual("Name3", result.Category.Name);
            Assert.AreEqual("Name3", result.Survey.Name);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostRedirectsWhenCategoryGoalNotFound()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Edit(4, new CategoryGoal())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("CategoryGoal Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsWhenCategoryNotCurrent()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Edit(1, new CategoryGoal())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Related Category is not Current", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostWhenInvalidReturnsView()
        {
            #region Arrange
            SetupData1();
            var editCategoryGoal = CreateValidEntities.CategoryGoal(2);
            editCategoryGoal.SetIdTo(2);
            editCategoryGoal.Name = string.Empty; //Invalid
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, editCategoryGoal)
                .AssertViewRendered()
                .WithViewData<CategoryGoalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.CategoryGoal.Name);
            Assert.AreEqual("Name3", result.Category.Name);
            Assert.AreEqual("Name3", result.Survey.Name);
            Controller.ModelState.AssertErrorsAre("Name: The Goal field is required.");
            CategoryGoalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CategoryGoal>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostWhenValidRedirects()
        {
            #region Arrange
            SetupData1();
            var editCategoryGoal = CreateValidEntities.CategoryGoal(2);
            editCategoryGoal.SetIdTo(2);
            editCategoryGoal.Name = "Updated";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, editCategoryGoal)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            CategoryGoalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CategoryGoal>.Is.Anything));
            var args = (CategoryGoal) CategoryGoalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<CategoryGoal>.Is.Anything))[0][0]; 
            Assert.AreEqual("Updated", args.Name);
            #endregion Assert
        }
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
