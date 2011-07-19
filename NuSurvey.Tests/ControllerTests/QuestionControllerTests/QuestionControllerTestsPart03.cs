using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

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
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
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
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView1()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Name = string.Empty;
            questionToEdit.Survey = null; //this is ignored
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            foreach (var responsesParameter in responses)
            {
                responsesParameter.Remove = true;
            }
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("The Question field is required.", "Active Responses are required.");
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Responses.Count);
            Assert.IsTrue(result.Responses[0].Remove);
            Assert.AreEqual(2, result.Responses[0].ResponseId); //Reversed
            Assert.IsTrue(result.Responses[1].Remove);
            Assert.AreEqual(1, result.Responses[1].ResponseId);
            Assert.AreEqual(2, result.Question.Responses.Count);
            Assert.AreEqual("Name2", result.Question.Survey.Name);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView2()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Name = string.Empty;
            questionToEdit.Survey = null; //this is ignored
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("The Question field is required.");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Responses.Count); //Reversed
            Assert.AreEqual(0, result.Responses[0].ResponseId); //Added
            Assert.AreEqual(2, result.Responses[1].ResponseId); 
            Assert.AreEqual(1, result.Responses[2].ResponseId);
            Assert.AreEqual(3, result.Question.Responses.Count);
            Assert.AreEqual("Name2", result.Question.Survey.Name);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView3()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[1].Score = null;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("All responses need a score");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Responses.Count); //Reversed
            Assert.AreEqual(0, result.Responses[0].ResponseId); //Added
            Assert.AreEqual(2, result.Responses[1].ResponseId);
            Assert.AreEqual(1, result.Responses[2].ResponseId);
            Assert.AreEqual(3, result.Question.Responses.Count);
            Assert.AreEqual("Name2", result.Question.Survey.Name);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView4()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[1].Value = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Response 3 must have a choice.");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Responses.Count); //Reversed
            Assert.AreEqual(0, result.Responses[0].ResponseId); //Added
            Assert.AreEqual(2, result.Responses[1].ResponseId);
            Assert.AreEqual(1, result.Responses[2].ResponseId);
            Assert.AreEqual(3, result.Question.Responses.Count);
            Assert.AreEqual("Name2", result.Question.Survey.Name);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView5()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CreateValidEntities.Category(99);
            questionToEdit.Category.IsCurrentVersion = false;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);

            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Selected Category is not current.");
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Responses.Count); //Reversed
            Assert.AreEqual(0, result.Responses[0].ResponseId); //Added
            Assert.AreEqual(2, result.Responses[1].ResponseId);
            Assert.AreEqual(1, result.Responses[2].ResponseId);
            Assert.AreEqual(3, result.Question.Responses.Count);
            Assert.AreEqual("Name2", result.Question.Survey.Name);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        /// <summary>
        /// Saving to same category (no answers)
        /// </summary>
        [TestMethod]
        public void TestEditPostWithValidDataAndNoVersioningSavesAndRedirects1()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = QuestionRepository.GetNullableById(questionId).Category;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question) QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert		
        }

        /// <summary>
        /// Saving to same category (no answers)
        /// </summary>
        [TestMethod]
        public void TestEditPostWithValidDataAndNoVersioningSavesAndRedirects2()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = QuestionRepository.GetNullableById(questionId).Category;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, 1, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Saving to different category (no answers)
        /// </summary>
        [TestMethod]
        public void TestEditPostWithValidDataAndNoVersioningSavesAndRedirects3()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(2);
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name2", args.Category.Name);
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Saving to different category (no answers)
        /// </summary>
        [TestMethod]
        public void TestEditPostWithValidDataAndNoVersioningSavesAndRedirects4()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(2);
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, 1, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name2", args.Category.Name);
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (active)/Category has no answers, move to category With answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects1A()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(7);
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            ArchiveService.Expect(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit))
                .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name8", args.Category.Name);
            Assert.AreEqual("Question Edited Successfully Previously Related Category Not Versioned Newly Related category Versioned", Controller.Message);
            #endregion Assert
        }


        /// <summary>
        /// Question (active)/Category has no answers, move to category With answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects1B()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(7);
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            ArchiveService.Expect(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit))
                .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, 1, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name8", args.Category.Name);
            Assert.AreEqual("Question Edited Successfully Previously Related Category Not Versioned Newly Related category Versioned", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (Inactive)/Category has no answers, move to category With answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects2A()
        {
            #region Arrange
            const int questionId = 1;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(7);
            questionToEdit.IsActive = false;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            //ArchiveService.Expect(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit))
            //    .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name7", args.Category.Name);
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (active)/Category has answers, move to category With no answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects3A()
        {
            #region Arrange
            const int questionId = 13;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(1);
            //questionToEdit.IsActive = false;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything))
                .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            //ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));            
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name1", args.Category.Name);
            Assert.AreEqual("Question Edited Successfully Previously Related Category Versioned Newly Related Category Not Versioned", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (active)/Category has answers, move to category With no answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects3B()
        {
            #region Arrange
            const int questionId = 13;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(1);
            //questionToEdit.IsActive = false;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything))
                .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, 7, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(8, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            //ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name1", args.Category.Name);
            Assert.AreEqual("Question Edited Successfully Previously Related Category Versioned Newly Related Category Not Versioned", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (active)/Category has answers, move to category With answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects4A()
        {
            #region Arrange
            const int questionId = 13;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(8);

            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything))
                .Return(CategoryRepository.GetNullableById(4)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            //ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 8, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name4", args.Category.Name); //It is 4, because this is what we faked the new category to
            Assert.AreEqual("Question Edited Successfully Previously Related Category Versioned Newly Related Category Versioned", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (active)/Category has answers, move to category With answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects4B()
        {
            #region Arrange
            const int questionId = 13;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(8);

            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything))
                .Return(CategoryRepository.GetNullableById(4)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, 8, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues["id"]); //Redirected here because this is the new version I faked.
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            //ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 8, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name4", args.Category.Name); //It is 4, because this is what we faked the new category to
            Assert.AreEqual("Question Edited Successfully Previously Related Category Versioned Newly Related Category Versioned", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (Inactive)/Category has answers, move to category With answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects5A()
        {
            #region Arrange
            const int questionId = 14;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(8);
            questionToEdit.IsActive = false;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            //ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything))
            //    .Return(CategoryRepository.GetNullableById(4)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 8, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name8", args.Category.Name); //It is 4, because this is what we faked the new category to
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (Inactive)/Category has answers, move to category With answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects5B()
        {
            #region Arrange
            const int questionId = 14;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(8);
            questionToEdit.IsActive = false;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            //ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything))
            //    .Return(CategoryRepository.GetNullableById(4)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, 7, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(8, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 8, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name8", args.Category.Name); //It is 4, because this is what we faked the new category to
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (Inactive)/Category has answers, move to category With No answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects6A()
        {
            #region Arrange
            const int questionId = 14;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(1);
            questionToEdit.IsActive = false;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            //ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything))
            //    .Return(CategoryRepository.GetNullableById(4)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, null, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 8, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name1", args.Category.Name); //It is 4, because this is what we faked the new category to
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert
        }

        /// <summary>
        /// Question (Inactive)/Category has answers, move to category With no answers
        /// </summary>
        [TestMethod]
        public void TestEditPostRedirects6B()
        {
            #region Arrange
            const int questionId = 14;
            SetupData3();
            var questionToEdit = CreateValidEntities.Question(questionId);
            questionToEdit.SetIdTo(questionId);
            questionToEdit.Category = CategoryRepository.GetNullableById(1);
            questionToEdit.IsActive = false;
            var responses = new ResponsesParameter[4];
            SetupResponses(responses, questionId);
            responses[0].Value = "updated";
            responses[0].Score = 99;
            responses[1].Remove = false;

            //ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything))
            //    .Return(CategoryRepository.GetNullableById(4)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(questionId, 2, 7, questionToEdit, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 7, questionToEdit));
            //ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Controller.Repository, 8, questionToEdit));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.Responses.Count);
            Assert.AreEqual("added", args.Responses[0].Value);
            Assert.IsTrue(args.Responses[1].IsActive);
            Assert.AreEqual("updated", args.Responses[2].Value);
            Assert.AreEqual(99, args.Responses[2].Score);
            Assert.AreEqual("Name1", args.Category.Name); //It is 4, because this is what we faked the new category to
            Assert.AreEqual("Question Edited Successfully", Controller.Message);
            #endregion Assert
        }
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
