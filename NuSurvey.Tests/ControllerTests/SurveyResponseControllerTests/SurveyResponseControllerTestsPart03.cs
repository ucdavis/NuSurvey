using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;

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

            var questionIds = new[] {4, 5, 8, 10, 13, 14};
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
            SurveyResponseRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
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
            SurveyResponseRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
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
            SurveyResponseRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestAnswerNextPostRedirectsIfQuestionNotFound()
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
            SurveyResponseRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestAnswerNextPostReturnsViewIfAnswerNotValid()
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
            questionAnswer.QuestionId = QuestionRepository.GetNullableById(5).Id;

            var scoredQuestionAnswer = new QuestionAnswerParameter();
            scoredQuestionAnswer.QuestionId = questionAnswer.QuestionId;
            scoredQuestionAnswer.Invalid = true;
            scoredQuestionAnswer.Message = "You Made a Mistake";

            ScoreService
                .Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything))
                .Return(scoredQuestionAnswer).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.AnswerNext(1, questionAnswer)
                .AssertViewRendered()
                .WithViewData<SingleAnswerSurveyResponseViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.PendingSurveyResponseExists);
            Assert.AreEqual(1, result.AnsweredQuestions);
            Assert.AreEqual(5, result.CurrentQuestion.Id);
            Assert.AreEqual(6, result.TotalActiveQuestions);

            ScoreService.AssertWasCalled(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything));
            var args =
                ScoreService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(15, (args[0] as IQueryable<Question>).Count());
            Assert.AreEqual(5, ((QuestionAnswerParameter) args[1]).QuestionId);
            Controller.ModelState.AssertErrorsAre("You Made a Mistake");
            SurveyResponseRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            #endregion Assert	
        }


        [TestMethod]
        public void TestAnswerNextPostRedirectsAndSavesWhenValid1()
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

            new FakeResponses(5, ResponseRepository);


            var questionAnswer = new QuestionAnswerParameter();
            questionAnswer.QuestionId = QuestionRepository.GetNullableById(5).Id;

            var scoredQuestionAnswer = new QuestionAnswerParameter();
            scoredQuestionAnswer.QuestionId = questionAnswer.QuestionId;
            scoredQuestionAnswer.Invalid = false;
            scoredQuestionAnswer.ResponseId = 4;
            scoredQuestionAnswer.Score = 89;

            ScoreService
                .Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything))
                .Return(scoredQuestionAnswer).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.AnswerNext(1, questionAnswer)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.AnswerNext(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);

            ScoreService.AssertWasCalled(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything));
            var args =
                ScoreService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(15, (args[0] as IQueryable<Question>).Count());
            Assert.AreEqual(5, ((QuestionAnswerParameter)args[1]).QuestionId);

            SurveyResponseRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            var surveyResponseArgs = (SurveyResponse) SurveyResponseRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything))[0][0]; 
            Assert.IsNotNull(surveyResponseArgs);
            Assert.AreEqual(1, surveyResponseArgs.Id);
            Assert.IsTrue(surveyResponseArgs.IsPending);
            Assert.AreEqual(2, surveyResponseArgs.Answers.Count);
            Assert.AreEqual(89, surveyResponseArgs.Answers[1].Score);
            Assert.AreEqual(4, surveyResponseArgs.Answers[1].Response.Id);
            Assert.AreEqual(null, surveyResponseArgs.Answers[1].OpenEndedAnswer);
            Assert.AreEqual(5, surveyResponseArgs.Answers[1].Question.Id);
            Assert.AreEqual(5, surveyResponseArgs.Answers[1].Category.Id);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAnswerNextPostRedirectsAndSavesWhenValid2()
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

            new FakeResponses(5, ResponseRepository);


            var questionAnswer = new QuestionAnswerParameter();
            questionAnswer.QuestionId = QuestionRepository.GetNullableById(5).Id;
            questionAnswer.Answer = "33";

            var scoredQuestionAnswer = new QuestionAnswerParameter();
            scoredQuestionAnswer.QuestionId = questionAnswer.QuestionId;
            scoredQuestionAnswer.Invalid = false;
            scoredQuestionAnswer.OpenEndedNumericAnswer = 33;
            scoredQuestionAnswer.ResponseId = 0;
            scoredQuestionAnswer.Score = 9;
            scoredQuestionAnswer.Answer = questionAnswer.Answer;

            ScoreService
                .Expect(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything))
                .Return(scoredQuestionAnswer).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.AnswerNext(1, questionAnswer)
                .AssertActionRedirect()
                .ToAction<SurveyResponseController>(a => a.AnswerNext(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["id"]);

            ScoreService.AssertWasCalled(a => a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything));
            var args =
                ScoreService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.ScoreQuestion(Arg<IQueryable<Question>>.Is.Anything, Arg<QuestionAnswerParameter>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(15, (args[0] as IQueryable<Question>).Count());
            Assert.AreEqual(5, ((QuestionAnswerParameter)args[1]).QuestionId);

            SurveyResponseRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything));
            var surveyResponseArgs = (SurveyResponse)SurveyResponseRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<SurveyResponse>.Is.Anything))[0][0];
            Assert.IsNotNull(surveyResponseArgs);
            Assert.AreEqual(1, surveyResponseArgs.Id);
            Assert.IsTrue(surveyResponseArgs.IsPending);
            Assert.AreEqual(2, surveyResponseArgs.Answers.Count);
            Assert.AreEqual(9, surveyResponseArgs.Answers[1].Score);
            Assert.AreEqual(null, surveyResponseArgs.Answers[1].Response);
            Assert.AreEqual(33, surveyResponseArgs.Answers[1].OpenEndedAnswer);
            Assert.AreEqual(5, surveyResponseArgs.Answers[1].Question.Id);
            Assert.AreEqual(5, surveyResponseArgs.Answers[1].Category.Id);
            #endregion Assert
        }

        #endregion AnswerNext Post Tests
        #endregion AnswerNext Tests
    }
}
