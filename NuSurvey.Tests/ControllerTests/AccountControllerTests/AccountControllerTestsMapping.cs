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
    }
}
