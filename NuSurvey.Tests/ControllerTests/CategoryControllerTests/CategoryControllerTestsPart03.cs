using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using NuSurvey.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using NuSurvey.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace NuSurvey.Tests.ControllerTests.CategoryControllerTests
{
    public partial class CategoryControllerTests
    {
        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsWhenCategoryNotFound()
        {
            #region Arrange
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(3, CategoryRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Category not found to edit.", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditGetRedirectsWhenNotCurrentVersion()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i+1));
            }
            categories[1].IsCurrentVersion = false;
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);
            #endregion Arrange

            #region Act
            Controller.Edit(2)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Category is not Current", Controller.Message);
            #endregion Assert	
        }

        [TestMethod]
        public void TestEditGetReturnsView1()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
            }
            categories[1].IsCurrentVersion = false;
            categories[2].Survey = CreateValidEntities.Survey(3);
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);

            var answers = new List<Answer>();
            for (int i = 0; i < 3; i++)
            {
                answers.Add(CreateValidEntities.Answer(i+1));
            }
            answers[1].Category = CategoryRepository.GetNullableById(3);
            var fakeAnswers = new FakeAnswers();
            fakeAnswers.Records(0, AnswerRepository, answers);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3)
                .AssertViewRendered()
                .WithViewData<CategoryViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual("Name3", result.Category.Name);
            Assert.IsTrue(result.HasRelatedAnswers);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView2()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
            }
            categories[1].IsCurrentVersion = false;
            categories[2].Survey = CreateValidEntities.Survey(3);
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);

            var answers = new List<Answer>();
            for (int i = 0; i < 3; i++)
            {
                answers.Add(CreateValidEntities.Answer(i + 1));
            }
            answers[1].Category = CategoryRepository.GetNullableById(1);
            var fakeAnswers = new FakeAnswers();
            fakeAnswers.Records(0, AnswerRepository, answers);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3)
                .AssertViewRendered()
                .WithViewData<CategoryViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual("Name3", result.Category.Name);
            Assert.IsFalse(result.HasRelatedAnswers);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests

        [TestMethod]
        public void TestEditPostRedirectsWhenCategoryNotFound()
        {
            #region Arrange
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(3, CategoryRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4, new Category())
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Category not found to edit.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRedirectsWhenNotCurrentVersion()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
            }
            categories[1].IsCurrentVersion = false;
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);
            #endregion Arrange

            #region Act
            Controller.Edit(2, new Category())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Category is not Current", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
            }
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);

            var categoryToEdit = CreateValidEntities.Category(3);
            categoryToEdit.SetIdTo(3);
            categoryToEdit.Name = string.Empty;

            var answers = new List<Answer>();
            for (int i = 0; i < 3; i++)
            {
                answers.Add(CreateValidEntities.Answer(i + 1));
            }
            answers[1].Category = CategoryRepository.GetNullableById(1);
            var fakeAnswers = new FakeAnswers();
            fakeAnswers.Records(0, AnswerRepository, answers);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, categoryToEdit)
                .AssertViewRendered()
                .WithViewData<CategoryViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Unable to Edit Category", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Name: The Name field is required.");
            Assert.AreEqual(string.Empty, result.Category.Name);
            Assert.IsFalse(result.HasRelatedAnswers);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditPostWithValidDataAndNoVersioningRedirects()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
            }
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);

            var categoryToEdit = CreateValidEntities.Category(3);
            categoryToEdit.SetIdTo(3);
            categoryToEdit.Name = "Updated";

            var answers = new List<Answer>();
            for (int i = 0; i < 3; i++)
            {
                answers.Add(CreateValidEntities.Answer(i + 1));
            }
            answers[1].Category = CategoryRepository.GetNullableById(1);
            var fakeAnswers = new FakeAnswers();
            fakeAnswers.Records(0, AnswerRepository, answers);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, categoryToEdit)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(9));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Category Edited Successfully", Controller.Message);
            Assert.AreEqual(3, result.RouteValues["id"]);
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything));
            var args = (Category) CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0]; 
            Assert.AreEqual("Updated", args.Name);
            #endregion Assert			
        }

        [TestMethod]
        public void TestEditPostWithValidDataAndVersioningRedirects1()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
            }
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);

            var categoryToEdit = CreateValidEntities.Category(3);
            categoryToEdit.SetIdTo(3);
            categoryToEdit.Name = "Updated";

            var answers = new List<Answer>();
            for (int i = 0; i < 3; i++)
            {
                answers.Add(CreateValidEntities.Answer(i + 1));
            }
            answers[1].Category = CategoryRepository.GetNullableById(3);
            var fakeAnswers = new FakeAnswers();
            fakeAnswers.Records(0, AnswerRepository, answers);

            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, categoryToEdit)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(9));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Category Edited Successfully", Controller.Message);
            Assert.AreEqual(3, result.RouteValues["id"]);
            CategoryRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything));
            var args = (Category)CategoryRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Category>.Is.Anything))[0][0];
            Assert.AreEqual("Updated", args.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithValidDataAndVersioningRedirect2()
        {
            #region Arrange
            var categories = new List<Category>();
            for (int i = 0; i < 3; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
            }
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);

            var categoryToEdit = CreateValidEntities.Category(3);
            categoryToEdit.SetIdTo(3);
            categoryToEdit.Name = "Updated";
            categoryToEdit.IsActive = true;

            var answers = new List<Answer>();
            for (int i = 0; i < 3; i++)
            {
                answers.Add(CreateValidEntities.Answer(i + 1));
            }
            answers[1].Category = CategoryRepository.GetNullableById(3);
            var fakeAnswers = new FakeAnswers();
            fakeAnswers.Records(0, AnswerRepository, answers);

            Assert.IsFalse(CategoryRepository.GetNullableById(3).IsActive);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, categoryToEdit)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(9));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Category Edited and Versioned Successfully", Controller.Message);
            Assert.AreEqual(3, result.RouteValues["id"]);
            CategoryRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Category>.Is.Anything));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 3, categoryToEdit));
            #endregion Assert
        }

        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
