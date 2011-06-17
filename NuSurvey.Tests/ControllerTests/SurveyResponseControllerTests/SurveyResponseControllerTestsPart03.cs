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
        #region AnswerNext Tests
        #region AnswerNext Get Tests

        [TestMethod]
        public void TestAnswerNextGetRedirectsIfPendingSurveyNotFound1()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i+1));
                surveyResponses[i].IsPending = false;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.AnswerNext(3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAnswerNextGetRedirectsIfPendingSurveyNotFound2()
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
            Controller.AnswerNext(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestAnswerNextGetRedirectsIfUserDoesNotMatch()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "noMatch@testy.com");
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
            Controller.AnswerNext(1)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestAnswerNextGetReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
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
            var result = Controller.AnswerNext(1)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PendingSurveyResponseExists);
            Assert.AreEqual(1, result.AnsweredQuestions);
            Assert.AreEqual(5, result.CurrentQuestion.Id);
            Assert.AreEqual(6, result.TotalActiveQuestions);
            #endregion Assert
        }


        [TestMethod]
        public void TestAnswerNextGetRedirectsWhenAllQuestionHaveBeenAnswered()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
            SetupDataForSingleAnswer();

            var questionIds = new int[] {4, 5, 8, 10, 13, 14};
            var answers = new List<Answer>();
            for (int i = 0; i < 6; i++)
            {
                answers.Add(CreateValidEntities.Answer(i+1));
                answers[i].Question = QuestionRepository.GetNullableById(questionIds[i]);
            }

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers = answers;
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.AnswerNext(1)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.FinalizePending(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            #endregion Assert		
        }
        #endregion AnswerNext Get Tests
        #region AnswerNext Post Tests
        [TestMethod]
        public void TestAnswerNextPostRedirectsIfPendingSurveyNotFound1()
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
            Controller.AnswerNext(3, new QuestionAnswerParameter())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerNextPostRedirectsIfPendingSurveyNotFound2()
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
            Controller.AnswerNext(4, new QuestionAnswerParameter())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            #endregion Assert
        }
        [TestMethod]
        public void TestAnswerNextPostRedirectsIfUserDoesNotMatch()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "noMatch@testy.com");
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
            Controller.AnswerNext(1, new QuestionAnswerParameter())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestAnswerNextRedirectsIfQuestionNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
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

            var questionAnswer = new QuestionAnswerParameter();
            questionAnswer.QuestionId = QuestionRepository.Queryable.Max(a => a.Id) + 1;
            #endregion Arrange

            #region Act
            Controller.AnswerNext(1, questionAnswer)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Question survey not found", Controller.Message);
            #endregion Assert		
        }

        /*
         * Answer tests:
         * Test when answer is a radio button:
         *  1) Response ID exists
         *  2) Score matches what the response id's score was.
         *  
         * When answer is open ended:
         *  when answer is an int
         *      1) score is calculated correctly exact match, + value, - value
         *      2) What about if value exists between + and -, but no exact match found?
         *  when answer is not an int
         *      What to do if date? what to do if some other answer type?
         */

        #endregion AnswerNext Post Tests
        #endregion AnswerNext Tests
    }
}
