using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
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
        public IRepository<Category> CategoryRepository;

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

            CategoryRepository = FakeRepository<Category>();
            Controller.Repository.Expect(a => a.OfType<Category>()).Return(CategoryRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();	
        }
        #endregion Init


        protected void SetupData1()
        {
            var categories = new List<Category>();
            for (int i = 0; i < 10; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
            }
            categories[2].IsCurrentVersion = false;
            categories[3].IsActive = false;

            var surveys = new List<Survey>();
            for (int i = 0; i < 3; i++)
            {
                surveys.Add(CreateValidEntities.Survey(i + 1));
            }
            surveys[2].Categories.Add(categories[1]);
            surveys[2].Categories.Add(categories[2]);
            surveys[2].Categories.Add(categories[3]);
            surveys[2].Categories.Add(categories[4]);
            new FakeSurveys(0, SurveyRepository, surveys);
            new FakeCategories(0, CategoryRepository, categories);
        }
    }
}
