using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web.Models;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using NuSurvey.Tests.Core.Extensions;



namespace NuSurvey.Tests.ControllerTests
{
    [TestClass]
    public class AccountControllerTests : ControllerTestBase<AccountController>
    {
        private readonly Type _controllerClass = typeof(AccountController);

        public IEmailService EmailService;
        public IFormsAuthenticationService FormService;
        public IMembershipService MembershipService;
        //public IRepository<Example> ExampleRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            EmailService = MockRepository.GenerateStub<IEmailService>();
            FormService = MockRepository.GenerateStub<IFormsAuthenticationService>();
            MembershipService = MockRepository.GenerateStub<IMembershipService>();

            Controller = new TestControllerBuilder().CreateController<AccountController>(EmailService, FormService, MembershipService);
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
        public void TestLogOnGetMapping()
        {
            "~/Account/Logon/".ShouldMapTo<AccountController>(a => a.LogOn());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestLogOnPostMapping()
        {
            "~/Account/Logon/".ShouldMapTo<AccountController>(a => a.LogOn(new LogOnModel(), "test"), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestLogOffGetMapping()
        {
            "~/Account/LogOff/".ShouldMapTo<AccountController>(a => a.LogOff());
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestRegisterGetMapping()
        {
            "~/Account/Register/".ShouldMapTo<AccountController>(a => a.Register());
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestRegisterPostMapping()
        {
            "~/Account/Register/".ShouldMapTo<AccountController>(a => a.Register(new RegisterModel(), new string[0]), true);
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestManageUsersMapping()
        {
            "~/Account/ManageUsers/".ShouldMapTo<AccountController>(a => a.ManageUsers());
        }
        #endregion Mapping Tests

        #region Method Tests

        #region LogOn Tests
        #region LogOn Get Tests                    
        [TestMethod]
        public void TestLogOnGetReturnsView()
        {
            #region Arrange
                     
            #endregion Arrange

            #region Act
            var result = Controller.LogOn().AssertViewRendered();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            #endregion Assert		
        }
        #endregion LogOn Get Tests
        #region LogOn Post Tests

        [TestMethod]
        public void TestLogOnPostReturnsViewIfModelInvalid()
        {
            #region Arrange
            var model = new LogOnModel();
            model.UserName = "UserName@test.com";
            model.Password = "Password";
            Controller.ModelState.AddModelError("Test", "MoreTest"); //Force Failure
            #endregion Arrange

            #region Act
            
            var result = Controller.LogOn(model, "")
                .AssertViewRendered()
                .WithViewData<LogOnModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UserName@test.com", result.UserName);
            Assert.AreEqual("Password", result.Password);
            #endregion Assert		
        }

        [TestMethod]
        public void TestLogOnPostReturnsViewIfModelValidateUserFails()
        {
            #region Arrange
            var model = new LogOnModel();
            model.UserName = "UserName@test.com";
            model.Password = "Password";
            MembershipService.Expect(a => a.ValidateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.LogOn(model, "")
                .AssertViewRendered()
                .WithViewData<LogOnModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UserName@test.com", result.UserName);
            Assert.AreEqual("Password", result.Password);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Controller.ModelState.AssertErrorsAre("The email or password provided is incorrect.");
            MembershipService.AssertWasCalled(a => a.ValidateUser("UserName@test.com".ToLower(), "Password"));
            #endregion Assert
        }

        [TestMethod]
        public void TestLogOnPostRedirectsWhenSuccessful1()
        {
            #region Arrange
            var model = new LogOnModel();
            model.UserName = "UserName@test.com";
            model.Password = "Password";
            MembershipService.Expect(a => a.ValidateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(true).Repeat.Any();
            FormService.Expect(a => a.SignIn("UserName@test.com", false));
            #endregion Arrange

            #region Act
            var result = Controller.LogOn(model, "")
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            MembershipService.AssertWasCalled(a => a.ValidateUser("UserName@test.com".ToLower(), "Password"));
            FormService.AssertWasCalled(a => a.SignIn("UserName@test.com", false));
            #endregion Assert
        }

        [TestMethod]
        public void TestLogOnPostRedirectsWhenSuccessful2()
        {
            #region Arrange
            var model = new LogOnModel();
            model.UserName = "UserName@test.com";
            model.Password = "Password";
            MembershipService.Expect(a => a.ValidateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(true).Repeat.Any();
            FormService.Expect(a => a.SignIn("UserName@test.com", false));
            
            #endregion Arrange

            #region Act
            var result = Controller.LogOn(model, "~/Survey")
                .AssertResultIs<RedirectResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Survey", result.Url);
            MembershipService.AssertWasCalled(a => a.ValidateUser("UserName@test.com".ToLower(), "Password"));
            FormService.AssertWasCalled(a => a.SignIn("UserName@test.com", false));
            #endregion Assert
        }

        #endregion LogOn Post Tests
        #endregion LogOn Tests

        #region LogOff Tests

        [TestMethod]
        public void TestLogOffRedirects()
        {
            #region Arrange
            FormService.Expect(a => a.SignOut());
            #endregion Arrange

            #region Act
            var result = Controller.LogOff()
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            FormService.AssertWasCalled(a => a.SignOut());
            #endregion Assert		
        }
        #endregion LogOff Tests

        #region Register Tests
        #region Register Get Tests

        [TestMethod]
        public void TestRegisterGetReturnsView()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            var result = Controller.Register()
                .AssertViewRendered()
                .WithViewData<RegisterModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("NSUser", Controller.ViewBag.UserRole);
            Assert.AreEqual("NSAdmin", Controller.ViewBag.AdminRole);
            #endregion Assert		
        }
        #endregion Register Get Tests
        #region Register Post Tests

        [TestMethod]
        public void TestRegisterPostReturnsViewIfInvalid1()
        {
            #region Arrange
            Controller.ModelState.AddModelError("Test", "MoreTest"); //Force Failure
            var viewModel = new RegisterModel();
            viewModel.Email = "Test@test.com";
            #endregion Arrange

            #region Act
            var result = Controller.Register(viewModel, new string[0])
                .AssertViewRendered()
                .WithViewData<RegisterModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test@test.com".ToLower(), result.Email);
            Assert.AreEqual("NSUser", Controller.ViewBag.UserRole);
            Assert.AreEqual("NSAdmin", Controller.ViewBag.AdminRole);
            #endregion Assert		
        }

        [TestMethod]
        public void TestRegisterPostReturnsViewIfInvalid2()
        {
            #region Arrange
            var viewModel = new RegisterModel();
            viewModel.Email = "Test@test.com";            
            MembershipService.Expect(a => a.CreateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(MembershipCreateStatus.DuplicateUserName).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Register(viewModel, new string[0])
                .AssertViewRendered()
                .WithViewData<RegisterModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test@test.com".ToLower(), result.Email);
            Assert.AreEqual("NSUser", Controller.ViewBag.UserRole);
            Assert.AreEqual("NSAdmin", Controller.ViewBag.AdminRole);
            Controller.ModelState.AssertErrorsAre("User already exists. Please enter a different user.");
            MembershipService.AssertWasCalled(a => a.CreateUser("Test@test.com".ToLower(), "BTDF4hd7ehd6@!", "Test@test.com".ToLower()));
            #endregion Assert
        }

        [TestMethod]
        public void TestRegisterPostRedirectsWhenValid1()
        {
            #region Arrange
            var viewModel = new RegisterModel();
            viewModel.Email = "Test@test.com";
            MembershipService.Expect(a => a.CreateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(MembershipCreateStatus.Success).Repeat.Any();
            MembershipService.Expect(a => a.ManageRoles(Arg<string>.Is.Anything, Arg<string[]>.Is.Anything))
                .Return(true).Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("Test@test.com".ToLower())).Return("123$$321").Repeat.Any();
            EmailService.Expect(a =>a.SendNewUser(
                Arg<HttpRequestBase>.Is.Anything, 
                Arg<UrlHelper>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything));
            #endregion Arrange

            #region Act
            var result = Controller.Register(viewModel, new[] {"NSUser", "NSAdmin"})
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers());
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("NSUser", Controller.ViewBag.UserRole);
            Assert.AreEqual("NSAdmin", Controller.ViewBag.AdminRole);
            MembershipService.AssertWasCalled(a => a.CreateUser("Test@test.com".ToLower(), "BTDF4hd7ehd6@!", "Test@test.com".ToLower()));
            MembershipService.AssertWasCalled(a => a.ManageRoles("Test@test.com".ToLower(), new[] { "NSUser", "NSAdmin" }));
            EmailService.AssertWasCalled(a => a.SendNewUser(Controller.Request, Controller.Url, "Test@test.com".ToLower(), "123$$321"));
            Assert.AreEqual("User and roles created. User emailed", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestRegisterPostRedirectsWhenValid2()
        {
            #region Arrange
            var viewModel = new RegisterModel();
            viewModel.Email = "Test@test.com";
            MembershipService.Expect(a => a.CreateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(MembershipCreateStatus.Success).Repeat.Any();
            MembershipService.Expect(a => a.ManageRoles(Arg<string>.Is.Anything, Arg<string[]>.Is.Anything))
                .Return(false).Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("Test@test.com".ToLower())).Return("123$$321").Repeat.Any();
            EmailService.Expect(a => a.SendNewUser(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything));
            #endregion Arrange

            #region Act
            var result = Controller.Register(viewModel, new string[0])
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers());
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("NSUser", Controller.ViewBag.UserRole);
            Assert.AreEqual("NSAdmin", Controller.ViewBag.AdminRole);
            MembershipService.AssertWasCalled(a => a.CreateUser("Test@test.com".ToLower(), "BTDF4hd7ehd6@!", "Test@test.com".ToLower()));
            MembershipService.AssertWasCalled(a => a.ManageRoles("Test@test.com".ToLower(), new string[0]));
            EmailService.AssertWasCalled(a => a.SendNewUser(Controller.Request, Controller.Url, "Test@test.com".ToLower(), "123$$321"));
            Assert.AreEqual("User created, but problem with roles. User emailed", Controller.Message);
            #endregion Assert
        }
        #endregion Register Post Tests
        #endregion Register Tests

        #region ManageUsers Tests

        [TestMethod]
        public void TestManageUsersReturnsViewWithExpectedValues()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "NSAdmin" }, "me@ucdavis.edu");
            var usersAndRoles = new List<UsersRoles>();
            usersAndRoles.Add(new UsersRoles{Admin = true, User = false, UserName = "test1.test.com"});
            usersAndRoles.Add(new UsersRoles { Admin = false, User = false, UserName = "test2.test.com" });
            usersAndRoles.Add(new UsersRoles { Admin = true, User = true, UserName = "test3.test.com" });

            MembershipService.Expect(a => a.GetUsersAndRoles("me@ucdavis.edu")).Return(usersAndRoles.AsQueryable());
            #endregion Arrange

            #region Act
            var result = Controller.ManageUsers()
                .AssertViewRendered()
                .WithViewData<IQueryable<UsersRoles>>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            #endregion Assert		
        }
        #endregion ManageUsers Tests

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
        /// Tests the controller has only three attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyThreeAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(3, result.Count());
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
            Assert.Inconclusive("Tests are still being written. When done, remove this line.");
            Assert.AreEqual(6, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLogOnGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LogOn");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLogOnPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LogOn");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLogOnContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("LogOff");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRegisterGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Register");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<AdminAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
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
        public void TestControllerMethodRegisterPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Register");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestControllerMethodRegisterPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Register");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<AdminAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "AdminAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestControllerMethodManageUsersContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("ManageUsers");
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

        //Examples

        //[TestMethod]
        //public void TestControllerMethodLogOnContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("LogOn");
        //    #endregion Arrange

        //    #region Act
        //    //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(0, allAttributes.Count());
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodLogOutContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("LogOut");
        //    #endregion Arrange

        //    #region Act
        //    //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(0, allAttributes.Count());
        //    #endregion Assert
        //}


        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes1()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(1, allAttributes.Count());
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes2()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
        //    Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes3()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}


        #endregion Controller Method Tests

        #endregion Reflection Tests

    }
}
