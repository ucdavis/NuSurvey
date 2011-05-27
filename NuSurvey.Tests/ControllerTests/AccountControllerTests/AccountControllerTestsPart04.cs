using System;
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
        #region ForgotPassword Tests
        #region ForgotPassword Get Tests

        [TestMethod]
        public void TestForgotPasswordReturnsView()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            var result = Controller.ForgotPassword()
                .AssertViewRendered()
                .WithViewData<ForgotPasswordModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.UserName);
            #endregion Assert
        }
        #endregion ForgotPassword Get Tests
        #region ForgotPassword Post Tests

        [TestMethod]
        public void TestForgotPasswordPostWhenNotValidReturnsView1()
        {
            #region Arrange
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ForgotPassword("test@ucdavis.edu", false)
                .AssertViewRendered()
                .WithViewData<ForgotPasswordModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test@ucdavis.edu", result.UserName);
            Controller.ModelState.AssertErrorsAre("Recaptcha value not valid");
            #endregion Assert
        }

        [TestMethod]
        public void TestForgotPasswordPostWhenNotValidReturnsView2()
        {
            #region Arrange
            //var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(null).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ForgotPassword("test@ucdavis.edu", true)
                .AssertViewRendered()
                .WithViewData<ForgotPasswordModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test@ucdavis.edu", result.UserName);
            Controller.ModelState.AssertErrorsAre("Email not found");
            #endregion Assert
        }


        [TestMethod]
        public void TestForgotPasswordPostRedirectsToLogOnWhenSuccessful()
        {
            #region Arrange
            var memberShipUser = new MembershipUser("AspNetSqlMembershipProvider", "test@ucdavis.edu", "", "test@ucdavis.edu", string.Empty, string.Empty, true, false, new DateTime(2011, 01, 02), new DateTime(2011, 01, 03), new DateTime(2011, 01, 04), new DateTime(2011, 01, 05), new DateTime(2011, 01, 06));
            MembershipService.Expect(a => a.GetUser("test@ucdavis.edu")).Return(memberShipUser).Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("test@ucdavis.edu")).Return("123321").Repeat.Any();
            EmailService.Expect(a => a.SendPasswordReset("test@ucdavis.edu", "123321")).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ForgotPassword("test@ucdavis.edu", true)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.LogOn());
            #endregion Act

            #region Assert
            Assert.AreEqual("A new password has been sent to your email. It should arrive in a few minutes. If you do not receive it, please check your email filters.", Controller.Message);
            MembershipService.AssertWasCalled(a => a.ResetPassword("test@ucdavis.edu"));
            MembershipService.AssertWasCalled(a => a.GetUser("test@ucdavis.edu"));
            EmailService.AssertWasCalled(a => a.SendPasswordReset("test@ucdavis.edu", "123321"));
            #endregion Assert		
        }
        #endregion ForgotPassword Post Tests
        #endregion ForgotPassword Tests

        #region ChangePassword Tests
        #region ChangePassword Get Tests

        [TestMethod]
        public void TestChangePasswordReturnsView()
        {
            #region Arrange
            MembershipService.Expect(a => a.MinPasswordLength).Return(6);
            #endregion Arrange

            #region Act
            var result = Controller.ChangePassword()
                .AssertViewRendered();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.ViewBag.PasswordLength);
            MembershipService.AssertWasCalled(a => a.MinPasswordLength);
            #endregion Assert		
        }
        #endregion ChangePassword Get Tests
        #region ChangePassword Post Tests

        [TestMethod]
        public void TestChangePasswordPostReturnsViewIfNotValid()
        {
            #region Arrange
            MembershipService.Expect(a => a.MinPasswordLength).Return(8);
            Controller.ModelState.AddModelError("Fake", @"force error");
            var viewModel = new ChangePasswordModel {OldPassword = "oldie", NewPassword = "newbe", ConfirmPassword = "no Match"};
            #endregion Arrange

            #region Act
            var result = Controller.ChangePassword(viewModel)
                .AssertViewRendered()
                .WithViewData<ChangePasswordModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("oldie", result.OldPassword);
            Assert.AreEqual("newbe", result.NewPassword);
            Assert.AreEqual("no Match", result.ConfirmPassword);
            Assert.AreEqual(8, Controller.ViewBag.PasswordLength);
            Controller.ModelState.AssertErrorsAre("force error");
            MembershipService.AssertWasCalled(a => a.MinPasswordLength);
            #endregion Assert		
        }


        [TestMethod]
        public void TestChangePasswordReturnsViewIfServiceFails()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me@test.com");
            MembershipService.Expect(a => a.MinPasswordLength).Return(8);
            MembershipService.Expect(a => a.ChangePassword("Me@test.com", "oldie", "newbe")).Return(false).Repeat.Any();
            var viewModel = new ChangePasswordModel {OldPassword = "oldie", NewPassword = "newbe", ConfirmPassword = "no Match"};
            #endregion Arrange

            #region Act
            var result = Controller.ChangePassword(viewModel)
                .AssertViewRendered()
                .WithViewData<ChangePasswordModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("oldie", result.OldPassword);
            Assert.AreEqual("newbe", result.NewPassword);
            Assert.AreEqual("no Match", result.ConfirmPassword);
            Assert.AreEqual(8, Controller.ViewBag.PasswordLength);
            Controller.ModelState.AssertErrorsAre("The current password is incorrect or the new password is invalid.");
            MembershipService.AssertWasCalled(a => a.MinPasswordLength);
            MembershipService.AssertWasCalled(a => a.ChangePassword("Me@test.com", "oldie", "newbe"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestChangePasswordRedirectsToChangePasswordSuccessIfSuccessful()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me@test.com");
            MembershipService.Expect(a => a.ChangePassword("Me@test.com", "oldie", "newbe")).Return(true).Repeat.Any();
            var viewModel = new ChangePasswordModel {OldPassword = "oldie", NewPassword = "newbe", ConfirmPassword = "no Match"};
            #endregion Arrange

            #region Act
            Controller.ChangePassword(viewModel)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ChangePasswordSuccess());
            #endregion Act

            #region Assert
            MembershipService.AssertWasCalled(a => a.ChangePassword("Me@test.com", "oldie", "newbe"));
            #endregion Assert
        }
        #endregion ChangePassword Post Tests
        #endregion ChangePassword Tests
    }
}
