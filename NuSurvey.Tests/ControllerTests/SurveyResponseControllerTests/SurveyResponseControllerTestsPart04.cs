using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    public partial class SurveyResponseControllerTests
    {
        #region FinalizePending Tests

        [TestMethod]
        public void TestFinalizePendingRedirectsWhenSurveyResponseNotFound()
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
            Controller.FinalizePending(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestFinalizePendingRedirectsWhenSurveyResponseNotPending()
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
            Controller.FinalizePending(3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Pending survey not found", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestFinalizePendingRedirectsWhenSurveyResponseOwnerIsDifferent()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "nomatch");
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].IsPending = true;
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            Controller.FinalizePending(3)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Not your survey", Controller.Message);
            SurveyResponseRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestFinalizePendingRedirectsIfCurrentQuestionIsNotNull()
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
            Controller.FinalizePending(1)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Error finalizing survey.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestFinalizePendingRedirectsWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "test@testy.com");
            SetupDataForSingleAnswer();

            var answers = new List<Answer>();
            for (int i = 0; i < 6; i++)
            {
                answers.Add(CreateValidEntities.Answer(i+1));
            }     
            //Answer All Questions
            answers[0].Question = QuestionRepository.GetNullableById(4);
            answers[1].Question = QuestionRepository.GetNullableById(5);
            answers[2].Question = QuestionRepository.GetNullableById(8);
            answers[3].Question = QuestionRepository.GetNullableById(10);
            answers[4].Question = QuestionRepository.GetNullableById(13);
            answers[5].Question = QuestionRepository.GetNullableById(14);

            var surveyResponses = new List<SurveyResponse>();
            surveyResponses.Add(CreateValidEntities.SurveyResponse(1));
            surveyResponses[0].UserId = "test@testy.com";
            surveyResponses[0].IsPending = true;
            surveyResponses[0].Answers = answers;
            surveyResponses[0].Survey = SurveyRepository.GetNullableById(1);
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            #endregion Arrange

            #region Act
            var result = Controller.FinalizePending(1)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.Results(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);
            ScoreService.AssertWasCalled(a => a.CalculateScores(Controller.Repository, surveyResponses[0]));
            SurveyResponseRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            var args = (SurveyResponse) SurveyResponseRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.IsFalse(args.IsPending);
            #endregion Assert
        }
        #endregion FinalizePending Tests
    }
}
