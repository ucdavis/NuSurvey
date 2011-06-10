using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;


namespace NuSurvey.Tests.ControllerTests.PrintControllerTests
{
    [TestClass]
    public partial class PrintControllerTests : ControllerTestBase<PrintController>
    {
        private readonly Type _controllerClass = typeof(PrintController);
        public IRepository<Survey> SurveyRepository;
        public IRepository<SurveyResponse> SurveyResponseRepository;
        public IPrintService PrintService;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            SurveyRepository = FakeRepository<Survey>();
            SurveyResponseRepository = FakeRepository<SurveyResponse>();
            PrintService = MockRepository.GenerateStub<IPrintService>();
            Controller = new TestControllerBuilder().CreateController<PrintController>(SurveyRepository, SurveyResponseRepository, PrintService);
 
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();            
        }

        public PrintControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Survey>()).Return(SurveyRepository).Repeat.Any();
            Controller.Repository.Expect(a => a.OfType<SurveyResponse>()).Return(SurveyResponseRepository).Repeat.Any();
        }
        #endregion Init
    }
}
