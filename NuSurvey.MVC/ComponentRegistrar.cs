using Castle.Windsor;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Castle.MicroKernel.Registration;
using NuSurvey.MVC.Models;
using NuSurvey.MVC.Services;

namespace NuSurvey.MVC
{
    internal static class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);

            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            container.Register(Component.For<IDbContext>().ImplementedBy<DbContext>().Named("dbContext"));
            container.Register(Component.For<IArchiveService>().ImplementedBy<ArchiveService>().Named("archiveService"));
            container.Register(Component.For<IEmailService>().ImplementedBy<EmailService>().Named("emailService"));
            container.Register(Component.For<IFormsAuthenticationService>().ImplementedBy<FormsAuthenticationService>().Named("formsAuthenticationService"));
            container.Register(Component.For<IMembershipService>().ImplementedBy<AccountMembershipService>().Named("membershipService"));
            container.Register(Component.For<IPrintService>().ImplementedBy<PrintService>().Named("printService"));
            container.Register(Component.For<IScoreService>().ImplementedBy<ScoreService>().Named("scoreService"));
            container.Register(Component.For<IPictureService>().ImplementedBy<PictureService>().Named("pictureService"));
            container.Register(Component.For<IBlobStoargeService>().ImplementedBy<BlobStoargeService>().Named("blobStoargeService"));
        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.Register(Component.For(typeof(IRepositoryWithTypedId<,>)).ImplementedBy(typeof(RepositoryWithTypedId<,>)).Named("repositoryWithTypedId"));
            container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).Named("repositoryType"));
            container.Register(Component.For<IRepository>().ImplementedBy<Repository>().Named("repository"));
        }
    }
}