using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Models;

namespace NuSurvey.Tests.ControllerTests.AccountControllerTests
{
    public partial class AccountControllerTests
    {
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

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Account/Edit/test".ShouldMapTo<AccountController>(a => a.Edit("test"));
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Account/Edit/".ShouldMapTo<AccountController>(a => a.Edit(new EditUserViewModel()), true);
        }

        /// <summary>
        /// #9
        /// </summary>
        [TestMethod]
        public void TestDeleteGetMapping()
        {
            "~/Account/Delete/test".ShouldMapTo<AccountController>(a => a.Delete("test"));
        }

        /// <summary>
        /// #10
        /// </summary>
        [TestMethod]
        public void TestDeletePostMapping()
        {
            "~/Account/Delete/test?confirm=0".ShouldMapTo<AccountController>(a => a.Delete("test", false), true);
        }

        /// <summary>
        /// #11
        /// </summary>
        [TestMethod]
        public void TestForgotPasswordGetMapping()
        {
            "~/Account/ForgotPassword/".ShouldMapTo<AccountController>(a => a.ForgotPassword());
        }
        #endregion Mapping Tests
    }
}
