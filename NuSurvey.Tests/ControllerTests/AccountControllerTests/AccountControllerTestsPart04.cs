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
    }
}
