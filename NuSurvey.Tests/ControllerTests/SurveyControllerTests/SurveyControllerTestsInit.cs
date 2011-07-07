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


namespace NuSurvey.Tests.ControllerTests.SurveyControllerTests
{
    [TestClass]
    public partial class SurveyControllerTests : ControllerTestBase<SurveyController>
    {
        private readonly Type _controllerClass = typeof(SurveyController);
        public IRepository<Survey> SurveyRepository;
        //public IExampleService ExampleService;
        //public IRepository<Example> ExampleRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            SurveyRepository = FakeRepository<Survey>();
            //ExampleService = MockRepository.GenerateStub<IExampleService>();  
            Controller = new TestControllerBuilder().CreateController<SurveyController>(SurveyRepository);
            //Controller = new TestControllerBuilder().CreateController<SurveyController>(SurveyRepository, ExampleService);
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

        public SurveyControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Survey>()).Return(SurveyRepository).Repeat.Any();	
        }
        #endregion Init



       protected void SetupData1()
       {
           var surveys = new List<Survey>();
           for (int i = 0; i < 3; i++)
           {
               surveys.Add(CreateValidEntities.Survey(i + 1));

           }

           for (int i = 0; i < 5; i++)
           {
               surveys[0].SurveyResponses.Add(CreateValidEntities.SurveyResponse(i));
               surveys[0].SurveyResponses[i].IsPending = false;
               if (i % 2 == 0)
               {
                   surveys[0].SurveyResponses[i].DateTaken = new DateTime(2011, 01, i + 1).Date.AddMinutes(i);
               }
               else
               {
                   surveys[0].SurveyResponses[i].DateTaken = new DateTime(2011, 01, i + 2).Date.AddMinutes(-(i + 1));
               }
           }

           for (int i = 0; i < 5; i++)
           {
               surveys[1].SurveyResponses.Add(CreateValidEntities.SurveyResponse(i));
               surveys[1].SurveyResponses[i].IsPending = false;
               if (i % 2 == 0)
               {
                   surveys[1].SurveyResponses[i].DateTaken = new DateTime(2011, 01, i + 1).Date.AddMinutes(i);
                   surveys[1].SurveyResponses[i].IsPending = true;
               }
               else
               {
                   surveys[1].SurveyResponses[i].DateTaken = new DateTime(2011, 01, i + 2).Date.AddMinutes(-(i + 1));
               }
           }

           for (int i = 0; i < 5; i++)
           {
               surveys[2].SurveyResponses.Add(CreateValidEntities.SurveyResponse(i));
               surveys[2].SurveyResponses[i].IsPending = i % 2 != 0;
               surveys[2].SurveyResponses[i].DateTaken = new DateTime(2011, 05, 1).Date.AddMinutes(30);
           }
           new FakeSurveys(0, SurveyRepository, surveys);
       }

       protected void SetupData2()
       {
           var surveys = new List<Survey>();
           for (int i = 0; i < 4; i++)
           {
               surveys.Add(CreateValidEntities.Survey(i + 1));

           }

           for (int i = 0; i < 5; i++)
           {
               surveys[0].SurveyResponses.Add(CreateValidEntities.SurveyResponse(i));
               surveys[0].SurveyResponses[i].IsPending = false;
               if (i % 2 == 0)
               {
                   surveys[0].SurveyResponses[i].DateTaken = new DateTime(2011, 01, i + 1).Date.AddMinutes(i);
               }
               else
               {
                   surveys[0].SurveyResponses[i].DateTaken = new DateTime(2011, 01, i + 2).Date.AddMinutes(-(i + 1));
               }
               surveys[0].SurveyResponses[i].UserId = "match@test.com";
           }

           for (int i = 0; i < 5; i++)
           {
               surveys[1].SurveyResponses.Add(CreateValidEntities.SurveyResponse(i));
               surveys[1].SurveyResponses[i].IsPending = false;
               if (i % 2 == 0)
               {
                   surveys[1].SurveyResponses[i].DateTaken = new DateTime(2011, 01, i + 1).Date.AddMinutes(i);
                   surveys[1].SurveyResponses[i].IsPending = true;
               }
               else
               {
                   surveys[1].SurveyResponses[i].DateTaken = new DateTime(2011, 01, i + 2).Date.AddMinutes(-(i + 1));
               }
               surveys[1].SurveyResponses[i].UserId = "match@test.com";
           }

           for (int i = 0; i < 5; i++)
           {
               surveys[2].SurveyResponses.Add(CreateValidEntities.SurveyResponse(i));
               surveys[2].SurveyResponses[i].IsPending = i % 2 != 0;
               surveys[2].SurveyResponses[i].DateTaken = new DateTime(2011, 05, 1).Date.AddMinutes(30);
               surveys[2].SurveyResponses[i].UserId = "match@test.com";
           }

           for (int i = 0; i < 5; i++)
           {
               surveys[3].SurveyResponses.Add(CreateValidEntities.SurveyResponse(i));
               surveys[3].SurveyResponses[i].IsPending = false;
               surveys[3].SurveyResponses[i].DateTaken = new DateTime(2011, 05, 1).Date.AddMinutes(30);
               surveys[3].SurveyResponses[i].UserId = "match@test.com";
           }
           surveys[3].SurveyResponses[2].UserId = "noMatch@test.com";
           surveys[3].SurveyResponses[3].UserId = "noMatch@test.com";

           new FakeSurveys(0, SurveyRepository, surveys);
       }

        
    }
}
