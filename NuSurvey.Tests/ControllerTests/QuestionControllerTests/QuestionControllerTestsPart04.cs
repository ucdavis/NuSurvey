using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Web.ActionResults;

namespace NuSurvey.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        #region ReOrder Tests
        #region ReOrder Get Tests

        [TestMethod]
        public void TestReOrderGetRedirectsWhenSurveyIdnotFound()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
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
        public void TestReOrderReturnsView()
        {
            #region Arrange
            var categories = new List<Category>();
            categories.Add(CreateValidEntities.Category(1));
            categories.Add(CreateValidEntities.Category(2));
            categories[0].IsCurrentVersion = false;

            var questions = new List<Question>();
            for (int i = 0; i < 5; i++)
            {
                questions.Add(CreateValidEntities.Question(i+1));
                questions[i].Category = categories[i%2];
            }

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i+1));
                surveys[i].Questions = questions;
            }
            new FakeSurveys(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            var result = Controller.ReOrder(2)
                .AssertViewRendered()
                .WithViewData<QuestionListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.Survey.Name);
            Assert.AreEqual(2, result.Questions.Count());
            #endregion Assert		
        }
        #endregion ReOrder Get Tests
        #region ReOrder Post Tests
        [TestMethod]
        public void TestReOrderPostReturnsFalseJSonResultWhenSurveyIdNotFound()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ReOrder(4, new int[0])
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("false", result.JsonResultString);
            #endregion Assert
        }


        [TestMethod]
        public void TestReOrderPostUpdatesQuestionsWhenValid()
        {
            #region Arrange
            var questions = new List<Question>();
            for (int i = 0; i < 5; i++)
            {
                questions.Add(CreateValidEntities.Question(i+1));
            }
            new FakeQuestions(0, QuestionRepository, questions);
            var surveys = new List<Survey>();
            surveys.Add(CreateValidEntities.Survey(1));
            for (int i = 0; i < 5; i++)
            {
                
                surveys[0].Questions.Add(QuestionRepository.GetNullableById(i+1));
            }
            new FakeSurveys(0, SurveyRepository, surveys);
            var newOrder = new int[5];
            newOrder[0] = 3;
            newOrder[1] = 2;
            newOrder[2] = 5;
            newOrder[3] = 1;
            newOrder[4] = 4;
            #endregion Arrange

            #region Act
            var result = Controller.ReOrder(1, newOrder)
                .AssertResultIs<JsonNetResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("true", result.JsonResultString);
            SurveyRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Survey>.Is.Anything));
            var args = (Survey) SurveyRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Survey>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(5, args.Questions.Count);
            var ordered = args.Questions.OrderBy(a => a.Order).ToList();
            Assert.AreEqual("Name3", ordered[0].Name);
            Assert.AreEqual("Name2", ordered[1].Name);
            Assert.AreEqual("Name5", ordered[2].Name);
            Assert.AreEqual("Name1", ordered[3].Name);
            Assert.AreEqual("Name4", ordered[4].Name);
            #endregion Assert		
        }
        #endregion ReOrder Post Tests
        #endregion ReOrder Tests
    }
}
