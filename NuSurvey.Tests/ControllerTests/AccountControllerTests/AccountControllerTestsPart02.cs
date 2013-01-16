using System;
using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Models;
using Rhino.Mocks;
using UCDArch.Testing.Fakes;

namespace NuSurvey.Tests.ControllerTests.AccountControllerTests
{
    public partial class AccountControllerTests
    {
        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsToErrorControllerIfTryToEditYourself()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "me@ucdavis.edu");
            #endregion Arrange

            #region Act
            Controller.Edit("Me@ucdavis.edu")
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Can't change yourself", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetRedirectsToManageUsersIfUserNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit("test@ucdavis.edu")
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            #endregion Act

            #region Assert
            Assert.AreEqual("User Not Found", Controller.Message);
            MembershipService.AssertWasCalled(a => a.GetUser("test@ucdavis.edu"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedValues()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            MembershipService.Expect(a => a.IsUserInRole("test@ucdavis.edu", "NSUser")).Return(true).Repeat.Any();
            MembershipService.Expect(a => a.IsUserInRole("test@ucdavis.edu", "NSAdmin")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit("test@ucdavis.edu")
                .AssertViewRendered()
                .WithViewData<EditUserViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test@ucdavis.edu", result.Email);
            Assert.IsTrue(result.IsUser);
            Assert.IsFalse(result.IsAdmin);
            Assert.AreEqual("test@ucdavis.edu", result.User.UserName);
            MembershipService.AssertWasCalled(a => a.GetUser("test@ucdavis.edu"));
            MembershipService.AssertWasCalled(a => a.IsUserInRole("test@ucdavis.edu", "NSUser"));
            MembershipService.AssertWasCalled(a => a.IsUserInRole("test@ucdavis.edu", "NSAdmin"));
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostRedirectsToErrorControllerIfTryToEditYourself()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var viewModel = new EditUserViewModel {Email = "Me@ucdavis.edu"};
            #endregion Arrange

            #region Act
            Controller.Edit(viewModel)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Can't change yourself", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToManageUsersIfUserNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(null).Repeat.Any();
            var viewModel = new EditUserViewModel {Email = "test@ucdavis.edu"};
            #endregion Arrange

            #region Act
            Controller.Edit(viewModel)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            #endregion Act

            #region Assert
            Assert.AreEqual("User Not Found", Controller.Message);
            MembershipService.AssertWasCalled(a => a.GetUser("test@ucdavis.edu"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToManageUsersIfValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            var viewModel = new EditUserViewModel {Email = "test@ucdavis.edu", IsAdmin = true, IsUser = true};

            MembershipService.Expect(a => a.ManageRoles("test@ucdavis.edu", new[] {"NSAdmin", "NSUser"}))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(viewModel)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Roles Updated", Controller.Message);
            MembershipService.AssertWasCalled(a => a.ManageRoles("test@ucdavis.edu", new[] { "NSAdmin", "NSUser" }));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToManageUsersIfValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            var viewModel = new EditUserViewModel {Email = "test@ucdavis.edu", IsAdmin = false, IsUser = true};

            MembershipService.Expect(a => a.ManageRoles("test@ucdavis.edu", new[] { "", "NSUser" }))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(viewModel)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Roles Updated", Controller.Message);
            MembershipService.AssertWasCalled(a => a.ManageRoles("test@ucdavis.edu", new[] {"", "NSUser" }));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToManageUsersIfValid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            var viewModel = new EditUserViewModel {Email = "test@ucdavis.edu", IsAdmin = true, IsUser = false};

            MembershipService.Expect(a => a.ManageRoles("test@ucdavis.edu", new[] { "NSAdmin", "" }))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(viewModel)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Roles Updated", Controller.Message);
            MembershipService.AssertWasCalled(a => a.ManageRoles("test@ucdavis.edu", new[] { "NSAdmin", "" }));
            #endregion Assert
        }
        [TestMethod]
        public void TestEditPostRedirectsToManageUsersIfValid4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            var viewModel = new EditUserViewModel {Email = "test@ucdavis.edu", IsAdmin = false, IsUser = false};

            MembershipService.Expect(a => a.ManageRoles("test@ucdavis.edu", new[] { "", "" }))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(viewModel)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Roles Updated", Controller.Message);
            MembershipService.AssertWasCalled(a => a.ManageRoles("test@ucdavis.edu", new[] { "", "" }));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToManageUsersIfValid5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            var viewModel = new EditUserViewModel {Email = "test@ucdavis.edu", IsAdmin = true, IsUser = true};

            MembershipService.Expect(a => a.ManageRoles("test@ucdavis.edu", new[] { "NSAdmin", "NSUser" }))
                .Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(viewModel)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Problem with Updating Roles", Controller.Message);
            MembershipService.AssertWasCalled(a => a.ManageRoles("test@ucdavis.edu", new[] { "NSAdmin", "NSUser" }));
            #endregion Assert
        }
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
