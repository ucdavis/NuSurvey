using System;
using System.Collections.Generic;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core.Helpers;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Helpers;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;


namespace NuSurvey.Tests.ControllerTests.CategoryGoalControllerTests
{
    [TestClass]
    public partial class CategoryGoalControllerTests : ControllerTestBase<CategoryGoalController>
    {
        private readonly Type _controllerClass = typeof(CategoryGoalController);
        public IRepository<CategoryGoal> CategoryGoalRepository;
        //public IExampleService ExampleService;
        public IRepository<Category> CategoryRepository;

        #region Init
        /// <summary>C:\Users\Sylvestre\Documents\Visual Studio 2010\Projects\NuSurvey\NuSurvey.Tests\ControllerTests\HomeControllerTest.cs
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            CategoryGoalRepository = FakeRepository<CategoryGoal>();
            //ExampleService = MockRepository.GenerateStub<IExampleService>();  
            Controller = new TestControllerBuilder().CreateController<CategoryGoalController>(CategoryGoalRepository);
            //Controller = new TestControllerBuilder().CreateController<CategoryGoalController>(CategoryGoalRepository, ExampleService);
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

        public CategoryGoalControllerTests()
        {
            CategoryRepository = FakeRepository<Category>();
            Controller.Repository.Expect(a => a.OfType<Category>()).Return(CategoryRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<CategoryGoal>()).Return(CategoryGoalRepository).Repeat.Any();	
        }
        #endregion Init


        protected void SetupData1()
        {
            var survey = CreateValidEntities.Survey(3);
            var categories = new List<Category>();
            for (int i = 0; i < 4; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].Survey = survey;
                categories[i].IsCurrentVersion = true;
            }
            categories[0].IsCurrentVersion = false;
            categories[1].IsCurrentVersion = false;
            var fakeCategories = new FakeCategories();
            fakeCategories.Records(0, CategoryRepository, categories);

            var categoryGoals = new List<CategoryGoal>();
            for (int i = 0; i < 3; i++)
            {
                categoryGoals.Add(CreateValidEntities.CategoryGoal(i + 1));
                categoryGoals[i].Category = categories[i+1];
            }

            var fakeCategoryGoals = new FakeCategoryGoals();
            fakeCategoryGoals.Records(0, CategoryGoalRepository, categoryGoals);
        }
        

    
    }
}
