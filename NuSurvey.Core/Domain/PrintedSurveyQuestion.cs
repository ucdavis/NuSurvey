using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace NuSurvey.Core.Domain
{
    public class PrintedSurveyQuestion : DomainObject
    {
        public PrintedSurveyQuestion()
        {
            Order = 0;
        }

        public virtual PrintedSurvey PrintedSurvey { get; set; }
        public virtual Question Question { get; set; }
        public virtual Photo Photo { get; set; }
        public virtual int Order { get; set; }
    }

    public class PrintedSurveyQuestionMap : ClassMap<PrintedSurveyQuestion>
    {
        public PrintedSurveyQuestionMap()
        {
            Id(x => x.Id);

            References(x => x.PrintedSurvey);
            References(x => x.Question).Cascade.None();
            References(x => x.Photo).Cascade.None();

            Map(x => x.Order).Column("`Order`");
        }
    }
}
