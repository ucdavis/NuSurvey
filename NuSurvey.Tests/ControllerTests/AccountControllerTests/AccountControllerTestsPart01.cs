using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Models;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.AccountControllerTests
{
    public partial class AccountControllerTests
    {
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
            var model = new LogOnModel {UserName = "UserName@test.com", Password = "Password"};
            Controller.ModelState.AddModelError("Test", @"MoreTest"); //Force Failure
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
            var model = new LogOnModel {UserName = "UserName@test.com", Password = "Password"};
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
            var model = new LogOnModel {UserName = "UserName@test.com", Password = "Password"};
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
            var model = new LogOnModel {UserName = "UserName@test.com", Password = "Password"};
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
            Controller.ModelState.AddModelError("Test", @"MoreTest"); //Force Failure
            var viewModel = new RegisterModel {Email = "Test@test.com"};
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
            var viewModel = new RegisterModel {Email = "Test@test.com"};
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
            var viewModel = new RegisterModel {Email = "Test@test.com"};
            MembershipService.Expect(a => a.CreateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Return(MembershipCreateStatus.Success).Repeat.Any();
            MembershipService.Expect(a => a.ManageRoles(Arg<string>.Is.Anything, Arg<string[]>.Is.Anything))
                .Return(true).Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("Test@test.com".ToLower())).Return("123$$321").Repeat.Any();
            EmailService.Expect(a => a.SendNewUser(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything));
            #endregion Arrange

            #region Act
            var result = Controller.Register(viewModel, new[] { "NSUser", "NSAdmin" })
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false,false,false));
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
            var viewModel = new RegisterModel {Email = "Test@test.com"};
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
                .ToAction<AccountController>(a => a.ManageUsers(false,false,false));
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
        public void TestManageUsersReturnsViewWithExpectedValues1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "NSAdmin" }, "me@ucdavis.edu");
            var usersAndRoles = new List<UsersRoles>
            {
                new UsersRoles {Admin = true, User = false, UserName = "test1.test.com"},
                new UsersRoles {Admin = false, User = false, UserName = "test2.test.com"},
                new UsersRoles {Admin = true, User = true, UserName = "test3.test.com"}
            };

            MembershipService.Expect(a => a.GetUsersAndRoles("me@ucdavis.edu")).Return(usersAndRoles.AsQueryable());
            #endregion Arrange

            #region Act
            var result = Controller.ManageUsers(false,false,false)
                .AssertViewRendered()
                .WithViewData<ManageUsersViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Users.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestManageUsersReturnsViewWithExpectedValues2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "NSAdmin" }, "me@ucdavis.edu");
            var usersAndRoles = new List<UsersRoles>
            {
                new UsersRoles {Admin = true, User = false, UserName = "test1.test.com"},
                new UsersRoles {Admin = false, User = false, UserName = "test2.test.com"},
                new UsersRoles {Admin = true, User = true, UserName = "test3.test.com"}
            };

            MembershipService.Expect(a => a.GetUsersAndRoles("me@ucdavis.edu")).Return(usersAndRoles.AsQueryable());
            #endregion Arrange

            #region Act
            var result = Controller.ManageUsers()
                .AssertViewRendered()
                .WithViewData<ManageUsersViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Users.Count());
            #endregion Assert
        }
        #endregion ManageUsers Tests
    }
}
