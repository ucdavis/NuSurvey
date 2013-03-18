using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace NuSurvey.Core.Domain
{
    public class PrintedSurvey : DomainObject
    {
        public PrintedSurvey()
        {
            PrintedSurveyQuestions = new List<PrintedSurveyQuestion>();
            DateCreated = DateTime.Now;
        }

        public virtual string UserId { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual Survey Survey { get; set; }

        public virtual IList<PrintedSurveyQuestion> PrintedSurveyQuestions { get; set; }
    }

    public class PrintedSurveyMap : ClassMap<PrintedSurvey>
    {
        public PrintedSurveyMap()
        {
            Id(x => x.Id);

            Map(x => x.UserId);
            Map(x => x.DateCreated);

            References(x => x.Survey);

            HasMany(x => x.PrintedSurveyQuestions).Inverse().Cascade.AllDeleteOrphan();
        }
    }
}
