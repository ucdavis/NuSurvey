using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
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
        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsIfSurveyNotFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(1, 4, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Not Found", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetRedirectsIfSurveyNotFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(1, 4, 7)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsIfQuestionNotFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            new FakeQuestions(3, QuestionRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, 3, null)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question Not Found.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsIfQuestionNotFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            new FakeQuestions(3, QuestionRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, 3, 7)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question Not Found.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditGetWhenQuestionSurveyIdDoesNotMatchPassedSurveyId()
        {
            #region Arrange
            new FakeCategories(3, CategoryRepository);
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
                for (int j = 0; j < 3; j++)
                {
                    surveys[i].Categories.Add(CreateValidEntities.Category(j + 4));
                }
                surveys[i].Categories[1].IsCurrentVersion = false;
            }
            new FakeSurveys(0, SurveyRepository, surveys);
            var responses = new List<Response>();
            for (int i = 0; i < 5; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].IsActive = true;
            }
            responses[2].IsActive = false;
            var questions = new List<Question>();
            for (int i = 0; i < 3; i++)
            {
                questions.Add(CreateValidEntities.Question(i + 1));
                questions[i].Responses = responses;
                questions[i].Survey = SurveyRepository.GetNullableById(1); //No Match
            }
            new FakeQuestions(0, QuestionRepository, questions);
            #endregion Arrange

            #region Act
            Controller.Edit(2, 2, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Question not related to current survey", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetWhenQuestionsCategoryIsNotCurrentVersion()
        {
            #region Arrange
            new FakeCategories(3, CategoryRepository);
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
                for (int j = 0; j < 3; j++)
                {
                    surveys[i].Categories.Add(CreateValidEntities.Category(j + 4));
                }
                surveys[i].Categories[1].IsCurrentVersion = false;
            }
            new FakeSurveys(0, SurveyRepository, surveys);
            var responses = new List<Response>();
            for (int i = 0; i < 5; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].IsActive = true;
            }
            responses[2].IsActive = false;
            var questions = new List<Question>();
            for (int i = 0; i < 3; i++)
            {
                questions.Add(CreateValidEntities.Question(i + 1));
                questions[i].Responses = responses;
                questions[i].Survey = SurveyRepository.GetNullableById(2);
                questions[i].Category.IsCurrentVersion = false;
            }
            new FakeQuestions(0, QuestionRepository, questions);
            #endregion Arrange

            #region Act
            Controller.Edit(2, 2, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Question's related category is not current version", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditGetReturnsView1()
        {
            #region Arrange
            new FakeCategories(3, CategoryRepository);
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i+1));
                for (int j = 0; j < 3; j++)
                {
                    surveys[i].Categories.Add(CreateValidEntities.Category(j+4));                    
                }
                surveys[i].Categories[1].IsCurrentVersion = false;
            }
            new FakeSurveys(0, SurveyRepository, surveys);
            var responses = new List<Response>();
            for (int i = 0; i < 5; i++)
            {
                responses.Add(CreateValidEntities.Response(i+1));
                responses[i].IsActive = true;
            }
            responses[2].IsActive = false;
            var questions = new List<Question>();            
            for (int i = 0; i < 3; i++)
            {
                questions.Add(CreateValidEntities.Question(i+1));
                questions[i].Responses = responses;
                questions[i].Survey = SurveyRepository.GetNullableById(3);
            }
            new FakeQuestions(0, QuestionRepository, questions);
            
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, 3, null)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.Question.Name);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual(2, result.Categories.Count());
            Assert.AreEqual(5, result.Responses.Count);
            Assert.IsNull(result.Category);
            Assert.IsTrue(result.Responses[2].Remove);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetReturnsView2()
        {
            #region Arrange
            new FakeCategories(3, CategoryRepository);
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
                for (int j = 0; j < 3; j++)
                {
                    surveys[i].Categories.Add(CreateValidEntities.Category(j + 4));
                }
                surveys[i].Categories[1].IsCurrentVersion = false;
            }
            new FakeSurveys(0, SurveyRepository, surveys);
            var responses = new List<Response>();
            for (int i = 0; i < 5; i++)
            {
                responses.Add(CreateValidEntities.Response(i + 1));
                responses[i].IsActive = true;
            }
            responses[2].IsActive = false;
            var questions = new List<Question>();
            for (int i = 0; i < 3; i++)
            {
                questions.Add(CreateValidEntities.Question(i + 1));
                questions[i].Responses = responses;
                questions[i].Survey = SurveyRepository.GetNullableById(3);
            }
            new FakeQuestions(0, QuestionRepository, questions);

            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, 3, 1)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.Question.Name);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual(2, result.Categories.Count());
            Assert.AreEqual(5, result.Responses.Count);
            Assert.AreEqual("Name1", result.Category.Name);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostRedirectsIfSurveyNotFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            var questionToEdit = CreateValidEntities.Question(9);
            var reponses = new ResponsesParameter[0]; 
            #endregion Arrange

            #region Act
            Controller.Edit(1, 4, null, questionToEdit, reponses)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsIfSurveyNotFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            var questionToEdit = CreateValidEntities.Question(9);
            var reponses = new ResponsesParameter[0]; 
            #endregion Arrange

            #region Act
            Controller.Edit(1, 4, 7, questionToEdit, reponses)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsIfQuestionNotFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            new FakeQuestions(3, QuestionRepository);
            var questionToEdit = CreateValidEntities.Question(9);
            var reponses = new ResponsesParameter[0]; 
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, 3, null, questionToEdit, reponses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question Not Found.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsIfQuestionNotFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            new FakeQuestions(3, QuestionRepository);
            var questionToEdit = CreateValidEntities.Question(9);
            var reponses = new ResponsesParameter[0]; 
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, 3, 7, questionToEdit, reponses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question Not Found.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView()
        {
            #region Arrange
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(1);
            questionToEdit.SetIdTo(1);
            questionToEdit.Name = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, 2, null, questionToEdit, new ResponsesParameter[0])
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("", "");
            #endregion Assert		
        }
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
