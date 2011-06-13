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
        public IRepository<Answer> AnswerRepository;

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

            AnswerRepository = FakeRepository<Answer>();
            Controller.Repository.Expect(a => a.OfType<Answer>()).Return(AnswerRepository).Repeat.Any();

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

        protected void SetupData2()
        {
            var answers = new List<Answer>();
            for (int i = 0; i < 3; i++)
            {
                answers.Add(CreateValidEntities.Answer(i + 1));
                answers[i].Category = CategoryRepository.GetNullableById(1);
            }
            new FakeAnswers(0, AnswerRepository, answers);           
        }

        /// <summary>
        /// create 3 surveys (use #2)
        /// Create categories (have two of each of these categories):
        /// 1) Has no answers, current version, active
        ///     1.1) Question is active
        ///     1.2) Question is Not Active
        /// 2) has no answers, current version, not active
        ///     2.1) Question is active
        ///     2.2) Question is not active
        /// 3) has answers, is not current version //This one can't be edited (and should not be in the select list)
        ///     3.1) Question is active
        ///     3.2) Question is not active
        /// 4) Has Answers, current version, active 
        ///     4.1) Question is active
        ///     4.2) Question is not active
        /// </summary>
        protected void SetupData3()
        {
            new FakeSurveys(3, SurveyRepository);
            var categories = new List<Category>();
            for (int i = 0; i < 8; i++)
            {
                categories.Add(CreateValidEntities.Category(i+1));
                categories[i].Survey = SurveyRepository.GetNullableById(2);
                categories[i].AddQuestions(CreateValidEntities.Question((i * 2) + 1));
                categories[i].AddQuestions(CreateValidEntities.Question((i * 2) + 2));
                categories[i].Questions[0].IsActive = true;
                categories[i].Questions[1].IsActive = false;
                categories[i].Questions[0].Responses.Add(CreateValidEntities.Response((i * 2) + 1));
                categories[i].Questions[0].Responses.Add(CreateValidEntities.Response((i * 2) + 2));
                categories[i].Questions[1].Responses.Add(CreateValidEntities.Response((i * 2) + 3));
                categories[i].Questions[1].Responses.Add(CreateValidEntities.Response((i * 2) + 4));
                categories[i].Questions[0].SetIdTo((i * 2) + 1);
                categories[i].Questions[1].SetIdTo((i * 2) + 2);
            }
            categories[0].IsActive = true;
            categories[0].IsCurrentVersion = true;
            categories[1].IsActive = true;
            categories[1].IsCurrentVersion = true;
            categories[2].IsActive = false;
            categories[2].IsCurrentVersion = true;
            categories[3].IsActive = false;
            categories[3].IsCurrentVersion = true;
            categories[4].IsActive = true;
            categories[4].IsCurrentVersion = false;
            categories[5].IsActive = true;
            categories[5].IsCurrentVersion = false;
            categories[6].IsActive = true;
            categories[6].IsCurrentVersion = true;
            categories[7].IsActive = true;
            categories[7].IsCurrentVersion = true;

            new FakeCategories(0, CategoryRepository, categories);

            var answers = new List<Answer>();
            for (int i = 0; i < 4; i++)
            {
                answers.Add(CreateValidEntities.Answer(i+1));
            }
            answers[0].Category = CategoryRepository.GetNullableById(4);
            answers[1].Category = CategoryRepository.GetNullableById(5);
            answers[2].Category = CategoryRepository.GetNullableById(6);
            answers[3].Category = CategoryRepository.GetNullableById(7);
            foreach (var answer in answers)
            {
                answer.Question = answer.Category.Questions[0];
            }
            new FakeAnswers(0, AnswerRepository, answers);
        }
    }
}
