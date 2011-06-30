using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;

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
        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostRedirectsIfSurveyNotFound1()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(4, null, new Question(), new ResponsesParameter[0])
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsIfSurveyNotFound2()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(4, 12, new Question(), new ResponsesParameter[0])
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Survey Not Found", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostWithInvalidDataReturnsView1()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.Category = CategoryRepository.GetNullableById(2);
            var responses = new ResponsesParameter[0];
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 2, questionToCreate, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Responses are required.");
            Assert.AreEqual("Name1", result.Question.Name);
            Assert.AreEqual(0, result.Responses.Count);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostWithInvalidDataReturnsView2()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.Category = CategoryRepository.GetNullableById(2);
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = 3;
            responses[0].Value = "Often";
            responses[0].Remove = true; //This is for a create, so this gets cleaned out.

            questionToCreate.Name = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 2, questionToCreate, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Name: The Name field is required.", "Responses are required.");
            Assert.AreEqual(string.Empty, result.Question.Name);
            Assert.AreEqual(0, result.Responses.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithInvalidDataReturnsView3()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.Category = CategoryRepository.GetNullableById(2);
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = 3;
            responses[0].Value = string.Empty; //Blank value here will be the same as hiding/removing it
            responses[0].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 2, questionToCreate, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Responses are required.");
            Assert.AreEqual("Name1", result.Question.Name);
            Assert.AreEqual(0, result.Responses.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithInvalidDataReturnsView4()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.Category = CategoryRepository.GetNullableById(2);
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = null; //Invalid
            responses[0].Value = "often"; 
            responses[0].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 2, questionToCreate, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("All responses need a score");
            Assert.AreEqual("Name1", result.Question.Name);
            Assert.AreEqual(1, result.Responses.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithInvalidDataReturnsView5()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.Category = null; //Invalid
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = 0;
            responses[0].Value = "often";
            responses[0].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 2, questionToCreate, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Category: The Category field is required.");
            Assert.AreEqual("Name1", result.Question.Name);
            Assert.AreEqual(1, result.Responses.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithInvalidDataReturnsView6()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.Category = CategoryRepository.Queryable.Where(a => !a.IsCurrentVersion).First();
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = 0;
            responses[0].Value = "often";
            responses[0].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 2, questionToCreate, responses)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(Controller.Message);
            Controller.ModelState.AssertErrorsAre("Selected Category is not current.");
            Assert.AreEqual("Name1", result.Question.Name);
            Assert.AreEqual(1, result.Responses.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithValidDataRedirects1()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.Category = CategoryRepository.GetNullableById(5);
            var responses = new ResponsesParameter[5];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 2;
            responses[0].Score = 4;
            responses[0].Value = "Value2";
            responses[0].Remove = false;

            responses[1] = new ResponsesParameter();
            responses[1].Order = 3;
            responses[1].Score = 3;
            responses[1].Value = "Value3";
            responses[1].Remove = false;

            responses[2] = new ResponsesParameter();
            responses[2].Order = 1;
            responses[2].Score = 2;
            responses[2].Value = "Value1";
            responses[2].Remove = false;

            responses[3] = new ResponsesParameter();
            responses[3].Order = 1;
            responses[3].Score = 2;
            responses[3].Value = string.Empty;
            responses[3].Remove = false;

            responses[4] = new ResponsesParameter();
            responses[4].Order = 1;
            responses[4].Score = 2;
            responses[4].Value = "blah";
            responses[4].Remove = true;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 2, questionToCreate, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("Question Created Successfully", Controller.Message);

            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything ));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question) QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual("Name5", args.Category.Name);
            Assert.AreEqual(DateTime.Now.Date, args.CreateDate.Date);
            Assert.AreEqual("Name3", args.Survey.Name);
            Assert.AreEqual(3, args.Responses.Count);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual("Value" + (i+1), args.Responses[i].Value);
            }
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithValidDataRedirects2()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.Category = CategoryRepository.GetNullableById(5);
            var responses = new ResponsesParameter[3];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 2;
            responses[0].Score = 4;
            responses[0].Value = "Value2";
            responses[0].Remove = false;

            responses[1] = new ResponsesParameter();
            responses[1].Order = 3;
            responses[1].Score = 3;
            responses[1].Value = "Value3";
            responses[1].Remove = false;

            responses[2] = new ResponsesParameter();
            responses[2].Order = 1;
            responses[2].Score = 2;
            responses[2].Value = "Value1";
            responses[2].Remove = false;
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, null, questionToCreate, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("Question Created Successfully", Controller.Message);

            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual("Name5", args.Category.Name);
            Assert.AreEqual(DateTime.Now.Date, args.CreateDate.Date);
            Assert.AreEqual("Name3", args.Survey.Name);
            Assert.AreEqual(3, args.Responses.Count);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual("Value" + (i + 1), args.Responses[i].Value);
            }
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostArchivesWhenValidAndExistingAnswers1()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.IsActive = true;
            questionToCreate.Category = CategoryRepository.GetNullableById(1); //This one has related answers
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = 4;
            responses[0].Value = "Value2";
            responses[0].Remove = false;
            ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything))
                .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, null, questionToCreate, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(8, result.RouteValues["id"]);
            Assert.AreEqual("Question Created Successfully, related Category versioned", Controller.Message);

            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            var archiveArgs = ArchiveService
                .GetArgumentsForCallsMadeOn(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything))[0];
            Assert.IsNotNull(archiveArgs);
            Assert.AreEqual(1, archiveArgs[1]);
            Assert.AreEqual("Name1", ((Category) archiveArgs[2]).Name);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual("Name8", args.Category.Name);
            Assert.AreEqual(DateTime.Now.Date, args.CreateDate.Date);
            Assert.AreEqual("Name3", args.Survey.Name);
            Assert.AreEqual(1, args.Responses.Count);
            Assert.AreEqual("Value2", args.Responses[0].Value);
            #endregion Assert	
        }

        [TestMethod]
        public void TestCreatePostArchivesWhenValidAndExistingAnswers2()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.IsActive = true;
            questionToCreate.Category = CategoryRepository.GetNullableById(1); //This one has related answers
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = 4;
            responses[0].Value = "Value2";
            responses[0].Remove = false;
            ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything))
                .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 7, questionToCreate, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(8, result.RouteValues["id"]);
            Assert.AreEqual("Question Created Successfully, related Category versioned", Controller.Message);

            ArchiveService.AssertWasCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            var archiveArgs = ArchiveService
                .GetArgumentsForCallsMadeOn(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything))[0];
            Assert.IsNotNull(archiveArgs);
            Assert.AreEqual(1, archiveArgs[1]);
            Assert.AreEqual("Name1", ((Category) archiveArgs[2]).Name);
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual("Name8", args.Category.Name);
            Assert.AreEqual(DateTime.Now.Date, args.CreateDate.Date);
            Assert.AreEqual("Name3", args.Survey.Name);
            Assert.AreEqual(1, args.Responses.Count);
            Assert.AreEqual("Value2", args.Responses[0].Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotArchiveWhenValidAndExistingAnswersButNotActive1()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.IsActive = false; //Create inactive Question
            questionToCreate.Category = CategoryRepository.GetNullableById(1); //This one has related answers
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = 4;
            responses[0].Value = "Value2";
            responses[0].Remove = false;
            ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything))
                .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, null, questionToCreate, responses)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            Assert.AreEqual("Question Created Successfully", Controller.Message);

            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual("Name1", args.Category.Name);
            Assert.AreEqual(DateTime.Now.Date, args.CreateDate.Date);
            Assert.AreEqual("Name3", args.Survey.Name);
            Assert.AreEqual(1, args.Responses.Count);
            Assert.AreEqual("Value2", args.Responses[0].Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotArchiveWhenValidAndExistingAnswersButNotActive2()
        {
            #region Arrange
            SetupData1();
            SetupData2();
            Question questionToCreate = CreateValidEntities.Question(1);
            questionToCreate.IsActive = false; //Create inactive Question
            questionToCreate.Category = CategoryRepository.GetNullableById(1); //This one has related answers
            var responses = new ResponsesParameter[1];
            responses[0] = new ResponsesParameter();
            responses[0].Order = 1;
            responses[0].Score = 4;
            responses[0].Value = "Value2";
            responses[0].Remove = false;
            ArchiveService.Expect(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything))
                .Return(CategoryRepository.GetNullableById(8)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(3, 1, questionToCreate, responses)
                .AssertActionRedirect()
                .ToAction<CategoryController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            Assert.AreEqual("Question Created Successfully", Controller.Message);

            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Category>.Is.Anything));
            ArchiveService.AssertWasNotCalled(a => a.ArchiveCategory(Arg<IRepository>.Is.Anything, Arg<int>.Is.Anything, Arg<Question>.Is.Anything));
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name1", args.Name);
            Assert.AreEqual("Name1", args.Category.Name);
            Assert.AreEqual(DateTime.Now.Date, args.CreateDate.Date);
            Assert.AreEqual("Name3", args.Survey.Name);
            Assert.AreEqual(1, args.Responses.Count);
            Assert.AreEqual("Value2", args.Responses[0].Value);
            #endregion Assert
        }


        #endregion Create Post Tests
        #endregion Create Tests
    }
}
