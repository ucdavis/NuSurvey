using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using NuSurvey.Tests.Core.Helpers;
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
        public IRepository<Category> CategoryRepository;
        public IRepository<CategoryTotalMaxScore> CategoryTotalMaxScoreRepository;
        public IRepository<Question> QuestionRepository;

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

            CategoryRepository = FakeRepository<Category>();
            Controller.Repository.Expect(a => a.OfType<Category>()).Return(CategoryRepository).Repeat.Any();

            QuestionRepository = FakeRepository<Question>();
            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();

            CategoryTotalMaxScoreRepository = FakeRepository<CategoryTotalMaxScore>();
            Controller.Repository.Expect(a => a.OfType<CategoryTotalMaxScore>()).Return(CategoryTotalMaxScoreRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<SurveyResponse>()).Return(SurveyResponseRepository).Repeat.Any();	
        }
        #endregion Init

        protected void SetupDataForSingleAnswer()
        {
            new FakeCategoryTotalMaxScore(5, CategoryTotalMaxScoreRepository);

            var categories = new List<Category>();
            for (int i = 0; i < 5; i++)
            {
                categories.Add(CreateValidEntities.Category(i + 1));
                categories[i].IsActive = true;
            }
            categories[0].IsActive = false;
            categories[1].IsCurrentVersion = false;

            new FakeCategories(0, CategoryRepository, categories);

            var questions = new List<Question>();
            var count = 0;
            for (int i = 0; i < 3; i++)
            {                
                foreach (var category in CategoryRepository.Queryable.ToList())
                {
                    count++;
                    questions.Add(CreateValidEntities.Question(count));
                    questions[count - 1].Category = category;
                    if (count % 3 == 0)
                    {
                        questions[count - 1].IsActive = false;                        
                    }
                }
            }

            new FakeQuestions(0, QuestionRepository, questions);

            var surveys = new List<Survey>();
            surveys.Add(CreateValidEntities.Survey(1));
            surveys[0].Categories = CategoryRepository.Queryable.ToList();
            surveys[0].Questions = QuestionRepository.Queryable.ToList();

            new FakeSurveys(0, SurveyRepository, surveys);
        }


    }
}
