using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using Rhino.Mocks;
using UCDArch.Testing;

namespace NuSurvey.Tests.ControllerTests.SurveyControllerTests
{

    public partial class SurveyControllerTests
    {

        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsWhenSurveyNotFound()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditGetReturnsViewWhenSurveyFound()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3)
                .AssertViewRendered()
                .WithViewData<Survey>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Name);
            #endregion Assert		
        }
        #endregion Edit Get Tests
        #region Edit Post Tests

        [TestMethod]
        public void TestEditPostRedirectsIfSurveyNotFound()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4, new Survey())
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            var survey = CreateValidEntities.Survey(9);
            survey.Name = null;
            survey.ShortName = "upd";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, survey)
                .AssertViewRendered()
                .WithViewData<Survey>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Name);
            Assert.AreEqual("upd", result.ShortName);
            Controller.ModelState.AssertErrorsAre("Name: The Name field is required.");
            SurveyRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Survey>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            var survey = CreateValidEntities.Survey(9);
            survey.Name = null;
            survey.ShortName = "upd";
            for (int i = 0; i < 5; i++)
            {
                survey.Questions.Add(CreateValidEntities.Question(i+1));
                survey.SurveyResponses.Add(CreateValidEntities.SurveyResponse(i+1));
                survey.Categories.Add(CreateValidEntities.Category(i+1));
            }
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, survey)
                .AssertViewRendered()
                .WithViewData<Survey>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Name);
            Assert.AreEqual("upd", result.ShortName);
            Assert.AreEqual(0, result.Questions.Count);
            Assert.AreEqual(0, result.SurveyResponses.Count);
            Assert.AreEqual(0, result.Categories.Count);
            Controller.ModelState.AssertErrorsAre("Name: The Name field is required.");
            SurveyRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Survey>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostWithValidDataRedirectsAndSaves()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            var survey = CreateValidEntities.Survey(9);
            survey.Name = "updated";
            survey.ShortName = "upd";
            survey.IsActive = false;
            survey.QuizType = "Qupdate";
            survey.SetIdTo(9);
            for (int i = 0; i < 5; i++)
            {
                survey.Questions.Add(CreateValidEntities.Question(i + 1));
                survey.SurveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                survey.Categories.Add(CreateValidEntities.Category(i + 1));
            }
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, survey)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.RouteValues["id"]); //The id is mapped
            Assert.AreEqual("Survey Edited Successfully", Controller.Message);
            SurveyRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Survey>.Is.Anything));
            var args = (Survey) SurveyRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Survey>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("updated", args.Name);
            Assert.AreEqual("upd", args.ShortName);
            Assert.IsFalse(args.IsActive);
            Assert.AreEqual("Qupdate", args.QuizType);
            Assert.AreEqual(9, args.Id);
            Assert.AreEqual(0, args.Questions.Count);
            Assert.AreEqual(0, args.SurveyResponses.Count);
            Assert.AreEqual(0, args.Categories.Count);
            #endregion Assert		
        }
        #endregion Edit Post Tests
        #endregion Edit Tests

        #region Review Tests

        [TestMethod]
        public void TestReviewReturnsViewWithExpectedValues()
        {
            #region Arrange
            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i+1));
            }
            surveys[1].IsActive = false;
            new FakeSurveys(0, SurveyRepository, surveys);
            #endregion Arrange

            #region Act
            var result = Controller.Review()
                .AssertViewRendered()
                .WithViewData<IList<Survey>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            #endregion Assert		
        }
        #endregion Review Tests
    }
}