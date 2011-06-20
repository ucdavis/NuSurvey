using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Helpers;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace NuSurvey.Tests.ControllerTests
{
    [TestClass]
    public class ErrorControllerTests : ControllerTestBase<ErrorController>
    {
        private readonly Type _controllerClass = typeof(ErrorController);

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            Controller = new TestControllerBuilder().CreateController<ErrorController>();
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        #endregion Init

        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Error/Index/".ShouldMapTo<ErrorController>(a => a.Index());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestNotAuthorizedMapping()
        {
            "~/Error/NotAuthorized/".ShouldMapTo<ErrorController>(a => a.NotAuthorized());
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestFileNotFoundMapping()
        {
            "~/Error/FileNotFound/".ShouldMapTo<ErrorController>(a => a.FileNotFound());
        }
        #endregion Mapping Tests

        #region Method Tests

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexReturnsView()
        {
            Controller.Index()
                .AssertViewRendered();
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestNotAuthorizedReturnsView()
        {
            Controller.NotAuthorized()
                .AssertViewRendered();
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestFileNotFoundReturnsView()
        {
            Controller.FileNotFound()
                .AssertViewRendered();
        }
        #endregion Method Tests

        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from application controller.
        /// </summary>
        [TestMethod]
        public void TestControllerInheritsFromApplicationController()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            Assert.IsNotNull(controllerClass.BaseType);
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("ApplicationController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has four attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasFourAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasVersionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<LocVersionAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "VersionAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasLocServiceMessageAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<ServiceMessageAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "LocServiceMessageAttribute not found.");
            #endregion Assert
        }

        #endregion Controller Class Tests

        #region Controller Method Tests

        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestControllerMethodNotAuthorizedContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("NotAuthorized");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestControllerMethodFileNotFoundContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("FileNotFound");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
