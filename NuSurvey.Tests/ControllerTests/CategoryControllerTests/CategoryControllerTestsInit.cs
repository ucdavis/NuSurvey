using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using NuSurvey.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using NuSurvey.Web;
using NuSurvey.Web.Controllers;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using Rhino.Mocks;
using UCDArch.Core.CommonValidator;
//using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Web.IoC;
using UCDArch.Core.PersistanceSupport;
using Castle.MicroKernel.Registration;


namespace NuSurvey.Tests.ControllerTests.CategoryControllerTests
{
    [TestClass]
    public partial class CategoryControllerTests : ControllerTestBase<CategoryController>
    {
        private readonly Type _controllerClass = typeof(CategoryController);
        public IRepository<Category> CategoryRepository;
        public IArchiveService ArchiveService;
        public IRepository<Survey> SurveyRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            CategoryRepository = FakeRepository<Category>();
            ArchiveService = MockRepository.GenerateStub<IArchiveService>();  
            Controller = new TestControllerBuilder().CreateController<CategoryController>(CategoryRepository, ArchiveService);
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            
        }

        protected override void InitServiceLocator()
        {
            var container = Core.ServiceLocatorInitializer.Init();

            base.RegisterAdditionalServices(container);
        }
        public CategoryControllerTests()
        {
            SurveyRepository = FakeRepository<Survey>();
            Controller.Repository.Expect(a => a.OfType<Survey>()).Return(SurveyRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Category>()).Return(CategoryRepository).Repeat.Any();	
        }
        #endregion Init

        
    }
}
