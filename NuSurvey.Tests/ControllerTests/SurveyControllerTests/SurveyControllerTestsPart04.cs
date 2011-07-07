using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.SurveyControllerTests
{

    public partial class SurveyControllerTests
    {

        #region YourDetails Tests

        [TestMethod]
        public void TestYourDetailsRedirectsToReviewWhenSurveyNotFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.YourDetails(4, null, null)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Review());
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsRedirectsToReviewWhenSurveyNotFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.YourDetails(4, new DateTime(2011, 01, 01), new DateTime(2011, 02, 01))
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Review());
            #endregion Act

            #region Assert
            #endregion Assert
        }


        [TestMethod]
        public void TestYourDetailsReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {RoleNames.User}, "match@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, null, null)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(5, result.SurveyResponses.Count());
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.IsNull(result.FilterBeginDate);
            Assert.IsNull(result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, new DateTime(2011, 01, 03), null)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(3, result.SurveyResponses.Count());
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.AreEqual(new DateTime(2011, 01, 03), result.FilterBeginDate);
            Assert.IsNull(result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, new DateTime(2011, 01, 03), new DateTime(2011, 01, 03))
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(1, result.SurveyResponses.Count());
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.AreEqual(new DateTime(2011, 01, 03), result.FilterBeginDate);
            Assert.AreEqual(new DateTime(2011, 01, 03), result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, null, new DateTime(2011, 01, 03))
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(3, result.SurveyResponses.Count());
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.AreEqual(null, result.FilterBeginDate);
            Assert.AreEqual(new DateTime(2011, 01, 03), result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            var singleDate = new DateTime(2011, 01, 01);
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, singleDate, singleDate)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(1, result.SurveyResponses.Count());
            Assert.AreEqual(singleDate, result.SurveyResponses.ElementAtOrDefault(0).DateTaken.Date);
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.AreEqual(singleDate, result.FilterBeginDate);
            Assert.AreEqual(singleDate, result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            var singleDate = new DateTime(2011, 01, 02);
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, singleDate, singleDate)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(1, result.SurveyResponses.Count());
            Assert.AreEqual(singleDate, result.SurveyResponses.ElementAtOrDefault(0).DateTaken.Date);
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.AreEqual(singleDate, result.FilterBeginDate);
            Assert.AreEqual(singleDate, result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            var singleDate = new DateTime(2011, 01, 03);
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, singleDate, singleDate)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(1, result.SurveyResponses.Count());
            Assert.AreEqual(singleDate, result.SurveyResponses.ElementAtOrDefault(0).DateTaken.Date);
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.AreEqual(singleDate, result.FilterBeginDate);
            Assert.AreEqual(singleDate, result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView8()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            var singleDate = new DateTime(2011, 01, 04);
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, singleDate, singleDate)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(1, result.SurveyResponses.Count());
            Assert.AreEqual(singleDate, result.SurveyResponses.ElementAtOrDefault(0).DateTaken.Date);
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.AreEqual(singleDate, result.FilterBeginDate);
            Assert.AreEqual(singleDate, result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView9()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            var singleDate = new DateTime(2011, 01, 05);
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(1, singleDate, singleDate)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasPendingResponses);
            Assert.AreEqual(1, result.SurveyResponses.Count());
            Assert.AreEqual(singleDate, result.SurveyResponses.ElementAtOrDefault(0).DateTaken.Date);
            Assert.AreEqual("Name1", result.Survey.Name);
            Assert.AreEqual(singleDate, result.FilterBeginDate);
            Assert.AreEqual(singleDate, result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView10()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(2, null, null)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasPendingResponses);
            Assert.AreEqual(2, result.SurveyResponses.Count());
            Assert.IsFalse(result.SurveyResponses.ElementAtOrDefault(0).IsPending);
            Assert.IsFalse(result.SurveyResponses.ElementAtOrDefault(1).IsPending);
            Assert.AreEqual("Name2", result.Survey.Name);
            Assert.AreEqual(null, result.FilterBeginDate);
            Assert.AreEqual(null, result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView11()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(3, null, null)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasPendingResponses);
            Assert.AreEqual(3, result.SurveyResponses.Count());
            Assert.IsFalse(result.SurveyResponses.ElementAtOrDefault(0).IsPending);
            Assert.IsFalse(result.SurveyResponses.ElementAtOrDefault(1).IsPending);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual(null, result.FilterBeginDate);
            Assert.AreEqual(null, result.FilterEndDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsReturnsView12()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            var singleDate = new DateTime(2011, 05, 01);
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(3, singleDate, singleDate)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasPendingResponses);
            Assert.AreEqual(3, result.SurveyResponses.Count());
            Assert.IsFalse(result.SurveyResponses.ElementAtOrDefault(0).IsPending);
            Assert.IsFalse(result.SurveyResponses.ElementAtOrDefault(1).IsPending);
            Assert.AreEqual("Name3", result.Survey.Name);
            Assert.AreEqual(singleDate, result.FilterBeginDate);
            Assert.AreEqual(singleDate, result.FilterEndDate);
            #endregion Assert
        }


        [TestMethod]
        public void TestYourDetailsChecksUserId1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(4, null, null)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.SurveyResponses.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestYourDetailsChecksUserId2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "nomatch@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(4, null, null)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.SurveyResponses.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestYourDetailsChecksUserId3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "nonematch@test.com");
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.YourDetails(4, null, null)
                .AssertViewRendered()
                .WithViewData<SurveyResponseDetailViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.SurveyResponses.Count());
            #endregion Assert
        }
        #endregion Details Tests

    }
}
