using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Helpers;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;


namespace NuSurvey.Tests.ControllerTests
{
    [TestClass]
    public class HomeControllerTests : ControllerTestBase<HomeController>
    {
        private readonly Type _controllerClass = typeof(HomeController);

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            Controller = new TestControllerBuilder().CreateController<HomeController>();
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
            "~/Home/Index/".ShouldMapTo<HomeController>(a => a.Index());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestAdministrationMapping()
        {
            "~/Home/Administration/".ShouldMapTo<HomeController>(a => a.Administration());
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestAboutMapping()
        {
            "~/Home/About/".ShouldMapTo<HomeController>(a => a.About());
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestSampleMapping()
        {
            "~/Home/Sample/".ShouldMapTo<HomeController>(a => a.Sample());
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestResetCacheMapping()
        {
            "~/Home/ResetCache/".ShouldMapTo<HomeController>(a => a.ResetCache());
        }
        #endregion Mapping Tests

        #region Method Tests
        [TestMethod]
        public void TestIndexReturnsView1()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {RoleNames.Admin});
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<HomeController.HomeViewModel>();
            Assert.IsTrue(result.Admin);
            Assert.IsFalse(result.User);
        }

        [TestMethod]
        public void TestIndexReturnsView2()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.User });
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<HomeController.HomeViewModel>();
            Assert.IsTrue(result.User);
            Assert.IsFalse(result.Admin);
        }

        [TestMethod]
        public void TestIndexReturnsView3()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin, RoleNames.User });
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<HomeController.HomeViewModel>();
            Assert.IsTrue(result.Admin);
            Assert.IsTrue(result.User);
        }

        [TestMethod]
        public void TestIndexReturnsView4()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" });
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<HomeController.HomeViewModel>();
            Assert.IsFalse(result.Admin);
            Assert.IsFalse(result.User);
        }

        [TestMethod]
        public void TestAboutReturnsView()
        {
            Controller.About()
                .AssertViewRendered();
        }

        [TestMethod]
        public void TestAdministrationReturnsView()
        {
            Controller.Administration()
                .AssertViewRendered();
        }

        [TestMethod]
        public void TestSampleReturnsView()
        {
            Controller.Sample()
                .AssertViewRendered();
        }


        [TestMethod]
        public void TestResetCacheRedirectsToIndex()
        {
            Controller.ResetCache()
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
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
        /// Tests the controller has six attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasSixAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(6, result.Count());
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
            Assert.IsTrue(result.Count() > 0, "LocVersionAttribute not found.");
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

        [TestMethod]
        public void TestControllerHasHandleTransactionsManuallyAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<HandleTransactionsManuallyAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "HandleTransactionsManuallyAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasAuthorizeAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<AuthorizeAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "AuthorizeAttribute not found.");
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
            Assert.AreEqual(5, result.Count(), "It looks like a method was added or removed from the controller.");
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
        public void TestControllerMethodAboutContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("About");
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
        public void TestControllerMethodAdministrationContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Administration");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AdminAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodSampleContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Sample");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AdminAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestControllerMethodResetCacheContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("ResetCache");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<AdminAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
