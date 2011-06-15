using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;

namespace NuSurvey.Tests.ControllerTests.SurveyControllerTests
{

    public partial class SurveyControllerTests
    {

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<IEnumerable<Survey>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            #endregion Assert		
        }
        #endregion Index Tests

        #region Details Tests

        [TestMethod]
        public void TestDetailsRedirectsToIndexWhenSurvyNotFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4, null, null)
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        [TestMethod]
        public void TestDetailsRedirectsToIndexWhenSurvyNotFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4, new DateTime(2011,01,01), new DateTime(2011,02,01))
                .AssertActionRedirect()
                .ToAction<SurveyController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert
        }


        [TestMethod]
        public void TestDetailReturnsView1()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, null, null)
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
        public void TestDetailReturnsView2()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, new DateTime(2011,01,03), null)
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
        public void TestDetailReturnsView3()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, new DateTime(2011, 01, 03), new DateTime(2011, 01, 03))
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
        public void TestDetailReturnsView4()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, null, new DateTime(2011, 01, 03))
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
        public void TestDetailReturnsView5()
        {
            #region Arrange
            SetupData1();
            var singleDate = new DateTime(2011, 01, 01);
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, singleDate, singleDate)
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
        public void TestDetailReturnsView6()
        {
            #region Arrange
            SetupData1();
            var singleDate = new DateTime(2011, 01, 02);
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, singleDate, singleDate)
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
        public void TestDetailReturnsView7()
        {
            #region Arrange
            SetupData1();
            var singleDate = new DateTime(2011, 01, 03);
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, singleDate, singleDate)
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
        public void TestDetailReturnsView8()
        {
            #region Arrange
            SetupData1();
            var singleDate = new DateTime(2011, 01, 04);
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, singleDate, singleDate)
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
        public void TestDetailReturnsView9()
        {
            #region Arrange
            SetupData1();
            var singleDate = new DateTime(2011, 01, 05);
            #endregion Arrange

            #region Act
            var result = Controller.Details(1, singleDate, singleDate)
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
        public void TestDetailReturnsView10()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(2, null, null)
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
        public void TestDetailReturnsView11()
        {
            #region Arrange
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Details(3, null, null)
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
        public void TestDetailReturnsView12()
        {
            #region Arrange
            SetupData1();
            var singleDate = new DateTime(2011, 05, 01);
            #endregion Arrange

            #region Act
            var result = Controller.Details(3, singleDate, singleDate)
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
        #endregion Details Tests

    }
}
