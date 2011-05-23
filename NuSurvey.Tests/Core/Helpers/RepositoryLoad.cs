using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuSurvey.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;


namespace NuSurvey.Tests.Core.Helpers
{
    public static class RepositoryLoad
    {
        public static void LoadSurveys(IRepository repository,int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Survey(i + 1);
                repository.OfType<Survey>().EnsurePersistent(validEntity);
            }
        }

        public static void LoadCategories(IRepository repository, int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Category(i + 1);
                validEntity.Survey = repository.OfType<Survey>().Queryable.First();
                repository.OfType<Category>().EnsurePersistent(validEntity);
            }
        }


        public static void LoadQuestions(IRepository repository, int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Question(i + 1);
                validEntity.Survey = repository.OfType<Survey>().Queryable.First();
                validEntity.Category = repository.OfType<Category>().Queryable.First();
                repository.OfType<Question>().EnsurePersistent(validEntity);
            }
        }
    }
}
