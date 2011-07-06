using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.PrintControllerTests
{
    public partial class PrintControllerTests
    {
        #region Result Tests (Single)

        [TestMethod]
        public void TestResultRedirectsWhenSurveyResponseNotFound()
        {
            #region Arrange
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.Result(4)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.FileNotFound());
            #endregion Act

            #region Assert
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestResultRedirectsWhenNotAdminAndDidNotCreateResponse()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i+1));
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {RoleNames.User}, "NoMatch");
            #endregion Arrange

            #region Act
            Controller.Result(2)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestResultReturnsFileWhenAdmin()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { RoleNames.Admin }, "NoMatch");
            PrintService.Expect(a => a.PrintSingle(2)).Return(new FileContentResult(new byte[] {2, 4, 1}, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.Result(2)
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            PrintService.AssertWasCalled(a => a.PrintSingle(2));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestResultReturnsFileWhenNotAdminButUserMatches()
        {
            #region Arrange
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
            }
            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { RoleNames.User }, "UserId2");
            PrintService.Expect(a => a.PrintSingle(2)).Return(new FileContentResult(new byte[] { 2, 4, 1 }, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.Result(2)
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            PrintService.AssertWasCalled(a => a.PrintSingle(2));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }
        #endregion Result Tests (Single)

        #region Results Tests (Multiple)

        [TestMethod]
        public void TestResultsRedirectsWhenSurveyNotFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Results(4, null, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.FileNotFound());
            #endregion Act

            #region Assert
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestResultsRedirectsWhenSurveyNotFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Results(4, new DateTime(2011,01,01), null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.FileNotFound());
            #endregion Act

            #region Assert
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestResultsRedirectsWhenSurveyNotFound3()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Results(4, null, new DateTime(2011,10,11))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.FileNotFound());
            #endregion Act

            #region Assert
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestResultsRedirectsWhenSurveyNotFound4()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.Results(4, new DateTime(2011,01,01), new DateTime(2010,01,01))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.FileNotFound());
            #endregion Act

            #region Assert
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestResultsWhenSurveyFound1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            PrintService.Expect(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(new FileContentResult(new byte[] {2, 4, 1}, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.Results(2, null, null)
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());

            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = PrintService.GetArgumentsForCallsMadeOn(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(new DateTime(2000,01,01), args[1]);
            Assert.AreEqual(DateTime.Now.Date.AddYears(1).AddDays(1).AddMinutes(-1), args[2]);
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestResultsWhenSurveyFound2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            PrintService.Expect(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(new FileContentResult(new byte[] { 2, 4, 1 }, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.Results(2, null, new DateTime(2011,02,03))
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());

            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = PrintService.GetArgumentsForCallsMadeOn(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(new DateTime(2000, 01, 01), args[1]);
            Assert.AreEqual(new DateTime(2011,02,03).Date.AddDays(1).AddMinutes(-1), args[2]);
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestResultsWhenSurveyFound3()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            PrintService.Expect(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(new FileContentResult(new byte[] { 2, 4, 1 }, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.Results(2, null, new DateTime(2011, 02, 03, 10,10,10))
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());

            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = PrintService.GetArgumentsForCallsMadeOn(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(new DateTime(2000, 01, 01), args[1]);
            Assert.AreEqual(new DateTime(2011, 02, 03).Date.AddDays(1).AddMinutes(-1), args[2]);
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestResultsWhenSurveyFound4()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            PrintService.Expect(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(new FileContentResult(new byte[] { 2, 4, 1 }, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.Results(2, new DateTime(2011, 02, 03, 10, 10, 10), null)
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());

            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = PrintService.GetArgumentsForCallsMadeOn(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(new DateTime(2011, 02, 03), args[1]);
            Assert.AreEqual(DateTime.Now.Date.AddYears(1).AddDays(1).AddMinutes(-1), args[2]);
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestResultsWhenSurveyFound5()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            PrintService.Expect(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(new FileContentResult(new byte[] { 2, 4, 1 }, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.Results(2, new DateTime(2011, 02, 03, 10, 10, 10), new DateTime(2011, 02, 03, 10, 10, 10)) //begin, end equal
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());

            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = PrintService.GetArgumentsForCallsMadeOn(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(new DateTime(2011, 02, 03), args[1]);
            Assert.AreEqual(new DateTime(2011, 02, 03).AddDays(1).AddMinutes(-1), args[2]);
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestResultsWhenSurveyFound6()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            PrintService.Expect(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything)).Return(new FileContentResult(new byte[] { 2, 4, 1 }, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.Results(2, new DateTime(2011, 02, 03, 10, 10, 10), new DateTime(2010, 02, 03, 10, 10, 10)) //end date less
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());

            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything));
            PrintService.AssertWasCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            var args = PrintService.GetArgumentsForCallsMadeOn(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything))[0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(new DateTime(2011, 02, 03), args[1]);
            Assert.AreEqual(new DateTime(2011, 02, 03).AddDays(1).AddMinutes(-1), args[2]);
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            #endregion Assert
        }
        #endregion Results Tests (Multiple)

    }
}