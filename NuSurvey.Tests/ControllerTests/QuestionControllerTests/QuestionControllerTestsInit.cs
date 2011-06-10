using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using NuSurvey.Core.Domain;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Helpers;
using NuSurvey.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace NuSurvey.Tests.ControllerTests.QuestionControllerTests
{
    [TestClass]
    public partial class QuestionControllerTests : ControllerTestBase<QuestionController>
    {
        private readonly Type _controllerClass = typeof(QuestionController);
        public IRepository<Question> QuestionRepository;
        public IArchiveService ArchiveService;
        public IRepository<Survey> SurveyRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            QuestionRepository = FakeRepository<Question>();
            ArchiveService = MockRepository.GenerateStub<IArchiveService>();
            Controller = new TestControllerBuilder().CreateController<QuestionController>(QuestionRepository, ArchiveService);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();
            base.RegisterAdditionalServices(container);
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work

        }

        protected override void InitServiceLocator()
        {
            var container = Core.ServiceLocatorInitializer.Init();

            RegisterAdditionalServices(container);
        }
        public QuestionControllerTests()
        {
            SurveyRepository = FakeRepository<Survey>();
            Controller.Repository.Expect(a => a.OfType<Survey>()).Return(SurveyRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();	
        }
        #endregion Init

    }
}
