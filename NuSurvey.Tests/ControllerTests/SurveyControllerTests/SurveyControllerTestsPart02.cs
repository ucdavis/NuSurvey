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
        #region PendingDetails Tests

        [TestMethod]
        public void TestPendingDetailsReturnsView1()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.PendingDetails(2)
                .AssertViewRendered()
                .WithViewData<SurveyPendingResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.SurveyResponses.Count());
            Assert.AreEqual("Name2", result.Survey.Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestPendingDetailsReturnsView2()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.PendingDetails(1)
                .AssertViewRendered()
                .WithViewData<SurveyPendingResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.SurveyResponses.Count());
            Assert.AreEqual("Name1", result.Survey.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestPendingDetailsReturnsView3()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.PendingDetails(3)
                .AssertViewRendered()
                .WithViewData<SurveyPendingResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.SurveyResponses.Count());
            Assert.AreEqual("Name3", result.Survey.Name);
            #endregion Assert
        }
        #endregion PendingDetails Tests

        #region Create Tests
        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetReturnsView()
        {
            #region Arrange
            
            #endregion Arrange
            
            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<SurveyViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Survey);
            Assert.IsTrue(result.Survey.IsTransient());
            #endregion Assert		
        }
        #endregion Create Get Tests
        #region Create Post Tests

        [TestMethod]
        public void TestCreatePostWithInvalidSurveyReturnsView()
        {
            #region Arrange
            var survey = CreateValidEntities.Survey(1);
            Controller.ModelState.AddModelError("Faked", @"Faked ModelStateError");
            #endregion Arrange

            #region Act
            var result = Controller.Create(survey)
                .AssertViewRendered()
                .WithViewData<SurveyViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("Faked ModelStateError");
            SurveyRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Survey>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreatePostWithValidSurveySavesAndRedirects()
        {
            #region Arrange
            var survey = CreateValidEntities.Survey(9);
            survey.SetIdTo(9);
            #endregion Arrange

            #region Act
            var result = Controller.Create(survey)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Edit(9));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.RouteValues["id"]);
            SurveyRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Survey>.Is.Anything));
            var args = (Survey) SurveyRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Survey>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual("Survey Created Successfully", Controller.Message);
            #endregion Assert		
        }
        #endregion Create Post Tests
        #endregion Create Tests
    }
}
