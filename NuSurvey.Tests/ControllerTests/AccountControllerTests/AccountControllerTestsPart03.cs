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
        #region Delete Tests
        #region Delete Get Tests
        [TestMethod]
        public void TestDeleteGetRedirectsToErrorControllerIfTryToEditYourself()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            #endregion Arrange

            #region Act
            Controller.Delete("Me@ucdavis.edu")
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Can't delete yourself", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetRedirectsToManageUsersIfUserNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete("test@ucdavis.edu")
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false,false,false));
            #endregion Act

            #region Assert
            Assert.AreEqual("User Not Found", Controller.Message);
            MembershipService.AssertWasCalled(a => a.GetUser("test@ucdavis.edu"));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteGetReturnsViewWithExpectedValues()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            MembershipService.Expect(a => a.IsUserInRole("test@ucdavis.edu", "NSUser")).Return(true).Repeat.Any();
            MembershipService.Expect(a => a.IsUserInRole("test@ucdavis.edu", "NSAdmin")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Delete("test@ucdavis.edu")
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
        #endregion Delete Get Tests
        #region Delete Post Tests
        [TestMethod]
        public void TestDeletePostRedirectsToErrorControllerIfTryToEditYourself1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            #endregion Arrange

            #region Act
            Controller.Delete("Me@ucdavis.edu", false)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Can't delete yourself", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToManageUsersIfUserNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete("test@ucdavis.edu", false)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false,false,false));
            #endregion Act

            #region Assert
            Assert.AreEqual("User Not Found", Controller.Message);
            MembershipService.AssertWasCalled(a => a.GetUser("test@ucdavis.edu"));
            #endregion Assert
        }
        [TestMethod]
        public void TestDeletePostRedirectsToErrorControllerIfTryToEditYourself2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            #endregion Arrange

            #region Act
            Controller.Delete("Me@ucdavis.edu", true)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.NotAuthorized());
            #endregion Act

            #region Assert
            Assert.AreEqual("Can't delete yourself", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToManageUsersIfUserNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete("test@ucdavis.edu", true)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false,false,false));
            #endregion Act

            #region Assert
            Assert.AreEqual("User Not Found", Controller.Message);
            MembershipService.AssertWasCalled(a => a.GetUser("test@ucdavis.edu"));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToManageUsersIfValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
       
            MembershipService.Expect(a => a.DeleteUser("test@ucdavis.edu"))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete("test@ucdavis.edu", true)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false,false,false));
            #endregion Act

            #region Assert
            Assert.AreEqual("User Removed", Controller.Message);
            MembershipService.AssertWasCalled(a => a.DeleteUser("test@ucdavis.edu"));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToManageUsersIfValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();

            MembershipService.Expect(a => a.DeleteUser("test@ucdavis.edu"))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete("test@ucdavis.edu", false)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false,false,false));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            MembershipService.AssertWasNotCalled(a => a.DeleteUser(Arg<string>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeletePostRedirectsToManageUsersIfValid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me@ucdavis.edu");
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();

            MembershipService.Expect(a => a.DeleteUser("test@ucdavis.edu"))
                .Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete("test@ucdavis.edu", true)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ManageUsers(false,false,false));
            #endregion Act

            #region Assert
            Assert.AreEqual("Remove User Failed", Controller.Message);
            MembershipService.AssertWasCalled(a => a.DeleteUser("test@ucdavis.edu"));
            #endregion Assert
        }
        #endregion Delete Post Tests
        #endregion Delete Tests
    }
}
