using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace NuSurvey.Core.Domain
{
    public class Answer : DomainObject
    {

        public virtual int Score { get; set; }

        [NotNull]
        public virtual SurveyResponse SurveyResponse { get; set; }
        [NotNull]
        public virtual Category Category { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }
        [NotNull]
        public virtual Response Response { get; set; }
    }


    public class AnswerMap : ClassMap<Answer>
    {
        public AnswerMap()
        {
            Id(x => x.Id);
            Map(x => x.Score);

            References(x => x.SurveyResponse);
            References(x => x.Category);
            References(x => x.Question);
            References(x => x.Response);
        }
    }
}


