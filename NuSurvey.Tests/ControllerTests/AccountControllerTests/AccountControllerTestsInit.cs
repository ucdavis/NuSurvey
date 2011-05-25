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
    [TestClass]
    public partial class AccountControllerTests : ControllerTestBase<AccountController>
    {
        private readonly Type _controllerClass = typeof(AccountController);

        public IEmailService EmailService;
        public IFormsAuthenticationService FormService;
        public IMembershipService MembershipService;

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

    }
}
