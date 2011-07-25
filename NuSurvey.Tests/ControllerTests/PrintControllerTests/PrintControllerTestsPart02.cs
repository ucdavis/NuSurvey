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
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.PrintControllerTests
{
    public partial class PrintControllerTests
    {
        #region PickResults Tests

        [TestMethod]
        public void TestPickResultsRedirectsWhenSurveyNotFound()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.PickResults(4, new int[0])
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.FileNotFound());
            #endregion Act

            #region Assert
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestPickResultsRediresIfNoPicks1()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.PickResults(2, null)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("No Survey Responses selected. Click on the rows of the table to select/deselect them.", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestPickResultsRediresIfNoPicks2()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            #endregion Arrange

            #region Act
            Controller.PickResults(2, new int[0])
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("No Survey Responses selected. Click on the rows of the table to select/deselect them.", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestPickResultsRedirectsWhenSurveyResponseNotFound()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            new FakeSurveyResponses(3, SurveyResponseRepository);
            #endregion Arrange

            #region Act
            Controller.PickResults(2, new int[] { 4 })
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Selected Survey Response Not Found.'4'", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestPickResultsRedirectsWhenSurveyResponseNotSameUser()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            new FakeSurveyResponses(3, SurveyResponseRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "nomatch@test.com");
            #endregion Arrange

            #region Act
            Controller.PickResults(2, new int[] { 2 })
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Selected Survey Response not yours.'2'", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestPickResultsThrowsExceptionIfSurveyIdDoesNotMatch1()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeSurveys(3, SurveyRepository);
                new FakeSurveyResponses(3, SurveyResponseRepository);
                Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "nomatch@test.com");
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.PickResults(2, new int[] { 2 });
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("SurveyResponse's survey id does not match 0 -- 2", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestPickResultsThrowsExceptionIfSurveyIdDoesNotMatch2()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                new FakeSurveys(3, SurveyRepository);
                var surveyResponses = new List<SurveyResponse>();
                for (int i = 0; i < 3; i++)
                {
                    surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                    surveyResponses[i].UserId = "match@test.com";
                    surveyResponses[i].Survey.SetIdTo(99);
                }

                new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
                Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User }, "match@test.com");
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.PickResults(2, new int[] { 2, 3 });
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("SurveyResponse's survey id does not match 99 -- 2", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestPickResultsCallsPrintServiceWhenAdmin()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].UserId = "match@test.com";
                surveyResponses[i].Survey.SetIdTo(2);
            }

            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "nomatch@test.com");
            PrintService.Expect(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything)).Return(new FileContentResult(new byte[] { 2, 4, 1 }, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.PickResults(2, new int[] { 2, 3 })
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());
            PrintService.AssertWasCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything));

            var args = PrintService.GetArgumentsForCallsMadeOn(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything))[0]; 
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(" 2 3", (args[4] as int[]).IntArrayToString());
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));            
            #endregion Assert
        }

        [TestMethod]
        public void TestPickResultsCallsPrintServiceWhenUser()
        {
            #region Arrange
            new FakeSurveys(3, SurveyRepository);
            var surveyResponses = new List<SurveyResponse>();
            for (int i = 0; i < 3; i++)
            {
                surveyResponses.Add(CreateValidEntities.SurveyResponse(i + 1));
                surveyResponses[i].UserId = "match@test.com";
                surveyResponses[i].Survey.SetIdTo(2);
            }

            new FakeSurveyResponses(0, SurveyResponseRepository, surveyResponses);
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin }, "match@test.com");
            PrintService.Expect(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything)).Return(new FileContentResult(new byte[] { 2, 4, 1 }, "pdf"));
            #endregion Arrange

            #region Act
            var result = Controller.PickResults(2, new int[] { 1, 2, 3 })
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("pdf", result.ContentType);
            Assert.AreEqual("241", result.FileContents.ByteArrayToString());
            PrintService.AssertWasCalled(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything));

            var args = PrintService.GetArgumentsForCallsMadeOn(a => a.PrintPickList(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<int[]>.Is.Anything))[0];
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(" 1 2 3", (args[4] as int[]).IntArrayToString());
            PrintService.AssertWasNotCalled(a => a.PrintSingle(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything));
            PrintService.AssertWasNotCalled(a => a.PrintMultiple(Arg<int>.Is.Anything, Arg<IRepository>.Is.Anything, Arg<MockHttpRequest>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<DateTime?>.Is.Anything, Arg<DateTime?>.Is.Anything));
            #endregion Assert
        }

        #endregion PickResults Tests
    }
}
