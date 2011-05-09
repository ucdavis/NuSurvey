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
        public static void LoadSurveys(IRepository<Survey> repository,int entriesToAdd)
        {
            for (int i = 0; i < entriesToAdd; i++)
            {
                var validEntity = CreateValidEntities.Survey(i + 1);
                repository.EnsurePersistent(validEntity);
            }
        }
    }
}
