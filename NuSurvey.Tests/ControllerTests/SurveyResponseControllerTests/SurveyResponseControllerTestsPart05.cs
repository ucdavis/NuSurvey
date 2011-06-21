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
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using NuSurvey.Tests.Core.Helpers;

namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    public partial class SurveyResponseControllerTests
    {
        #region DeletePending Tests
        #region DeletePending Get Tests

        [TestMethod]
        public void TestDeletePendingGetRedirectsIfSurveyNotFound1()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePending(4, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeletePendingGetRedirectsIfSurveyNotFound2()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePending(4, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingGetRedirectsIfSurveyNotFound3()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = false;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingGetRedirectsIfSurveyNotFound4()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = false;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingGetRedirectsIfNotAdminAndNotOwner1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {RoleNames.User}, "nomatch");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = true;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingGetRedirectsIfNotAdminAndNotOwner2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "nomatch");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = true;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "nomatch");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(1, false)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PendingSurveyResponseExists);
            Assert.AreEqual(1, result.AnsweredQuestions);
            Assert.AreEqual(5, result.CurrentQuestion.Id);
            Assert.AreEqual(6, result.TotalActiveQuestions);
            Assert.IsFalse(result.FromAdmin);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin, RoleNames.User }, "nomatch");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(1, true)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PendingSurveyResponseExists);
            Assert.AreEqual(1, result.AnsweredQuestions);
            Assert.AreEqual(5, result.CurrentQuestion.Id);
            Assert.AreEqual(6, result.TotalActiveQuestions);
            Assert.IsTrue(result.FromAdmin);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingGetReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "test@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(1, false)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PendingSurveyResponseExists);
            Assert.AreEqual(1, result.AnsweredQuestions);
            Assert.AreEqual(5, result.CurrentQuestion.Id);
            Assert.AreEqual(6, result.TotalActiveQuestions);
            Assert.IsFalse(result.FromAdmin);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingGetReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(1, true)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PendingSurveyResponseExists);
            Assert.AreEqual(1, result.AnsweredQuestions);
            Assert.AreEqual(5, result.CurrentQuestion.Id);
            Assert.AreEqual(6, result.TotalActiveQuestions);
            Assert.IsTrue(result.FromAdmin);
            #endregion Assert
        }

        #endregion DeletePending Get Tests
        #region DeletePending Post Tests

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfSurveyNotFound1A()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePending(4, true, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfSurveyNotFound2A()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePending(4, false, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfSurveyNotFound1B()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePending(4, true, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfSurveyNotFound2B()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.DeletePending(4, false, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfSurveyNotFound3A()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = false;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, true, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfSurveyNotFound4A()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = false;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, false, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfSurveyNotFound3B()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = false;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, true, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfSurveyNotFound4B()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = false;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, false, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfNotAdminAndNotOwner1A()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {RoleNames.User}, "nomatch");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = true;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, true, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfNotAdminAndNotOwner2A()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "nomatch");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = true;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, false, false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfNotAdminAndNotOwner1B()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "nomatch");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = true;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, true, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsIfNotAdminAndNotOwner2B()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "nomatch");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = true;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(3, false, true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePendingPostRedirectsWhenConfimIsFalse1A()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);

            surveyResponses.Add(CreateValidEntities.SurveyResponse(2));
            surveyResponses[1].UserId = "test@testy.com";
            surveyResponses[1].IsPending = true;
            surveyResponses[1].Answers.Add(answer);
            surveyResponses[1].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(2, false, true)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.PendingDetails(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsWhenConfimIsFalse1B()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "nomatch@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);

            surveyResponses.Add(CreateValidEntities.SurveyResponse(2));
            surveyResponses[1].UserId = "test@testy.com";
            surveyResponses[1].IsPending = true;
            surveyResponses[1].Answers.Add(answer);
            surveyResponses[1].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(2, false, true)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.PendingDetails(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsWhenConfimIsFalse2A()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);

            surveyResponses.Add(CreateValidEntities.SurveyResponse(2));
            surveyResponses[1].UserId = "test@testy.com";
            surveyResponses[1].IsPending = true;
            surveyResponses[1].Answers.Add(answer);
            surveyResponses[1].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(2, false, false)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.StartSurvey(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostRedirectsWhenConfimIsFalse2B()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "nomatch@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);

            surveyResponses.Add(CreateValidEntities.SurveyResponse(2));
            surveyResponses[1].UserId = "test@testy.com";
            surveyResponses[1].IsPending = true;
            surveyResponses[1].Answers.Add(answer);
            surveyResponses[1].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(2, false, false)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.StartSurvey(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            SurveyResponseRepository.AssertWasNotCalled(a => a.Remove(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestDeletePendingPostWhenConfirm1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "nomatch@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);

            surveyResponses.Add(CreateValidEntities.SurveyResponse(2));
            surveyResponses[1].UserId = "test@testy.com";
            surveyResponses[1].IsPending = true;
            surveyResponses[1].Answers.Add(answer);
            surveyResponses[1].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(2, true, false)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            SurveyResponseRepository.AssertWasCalled(a => a.Remove(surveyResponses[1]));
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeletePendingPostWhenConfirm2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "test@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);

            surveyResponses.Add(CreateValidEntities.SurveyResponse(2));
            surveyResponses[1].UserId = "test@testy.com";
            surveyResponses[1].IsPending = true;
            surveyResponses[1].Answers.Add(answer);
            surveyResponses[1].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.DeletePending(2, true, false)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            SurveyResponseRepository.AssertWasCalled(a => a.Remove(surveyResponses[1]));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostWhenConfirm3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "nomatch@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);

            surveyResponses.Add(CreateValidEntities.SurveyResponse(2));
            surveyResponses[1].UserId = "test@testy.com";
            surveyResponses[1].IsPending = true;
            surveyResponses[1].Answers.Add(answer);
            surveyResponses[1].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(2, true, true)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.PendingDetails(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            SurveyResponseRepository.AssertWasCalled(a => a.Remove(surveyResponses[1]));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePendingPostWhenConfirm4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "test@testy.com");
            SetupDataForSingleAnswer();

            var answer = CreateValidEntities.Answer(1);
            answer.Question = QuestionRepository.GetNullableById(4);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers.Add(answer);
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);

            surveyResponses.Add(CreateValidEntities.SurveyResponse(2));
            surveyResponses[1].UserId = "test@testy.com";
            surveyResponses[1].IsPending = true;
            surveyResponses[1].Answers.Add(answer);
            surveyResponses[1].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.DeletePending(2, true, true)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.PendingDetails(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            SurveyResponseRepository.AssertWasCalled(a => a.Remove(surveyResponses[1]));
            #endregion Assert
        }
        #endregion DeletePending Post Tests
        #endregion DeletePending Tests
    }
}