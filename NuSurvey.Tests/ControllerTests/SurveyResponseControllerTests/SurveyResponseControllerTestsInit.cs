using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web.Helpers;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace NuSurvey.Tests.ControllerTests.SurveyResponseControllerTests
{
    [TestClass]
    public partial class SurveyResponseControllerTests : ControllerTestBase<SurveyResponseController>
    {
        private readonly Type _controllerClass = typeof(SurveyResponseController);
        public IRepository<SurveyResponse> SurveyResponseRepository;
        //public IExampleService ExampleService;
        public IRepository<Survey> SurveyRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            SurveyResponseRepository = FakeRepository<SurveyResponse>();
            //ExampleService = MockRepository.GenerateStub<IExampleService>();  
            Controller = new TestControllerBuilder().CreateController<SurveyResponseController>(SurveyResponseRepository);
            //Controller = new TestControllerBuilder().CreateController<SurveyResponseController>(SurveyResponseRepository, ExampleService);
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

        public SurveyResponseControllerTests()
        {
            SurveyRepository = FakeRepository<Survey>();
            Controller.Repository.Expect(a => a.OfType<Survey>()).Return(SurveyRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<SurveyResponse>()).Return(SurveyResponseRepository).Repeat.Any();	
        }
        #endregion Init

    }
}
