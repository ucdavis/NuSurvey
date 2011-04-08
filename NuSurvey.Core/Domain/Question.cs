using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace NuSurvey.Core.Domain
{
    public class Question : DomainObject
    {
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsOpenEnded { get; set; }
        [NotNull]
        public virtual Category Category { get; set; }
        [NotNull]
        public virtual Survey Survey { get; set; }
    }
    public class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            Id(x => x.Id);
            Map(x => x.IsActive);
            Map(x => x.Order).Column("`Order`");
            Map(x => x.IsOpenEnded);

            References(x => x.Category);
            References(x => x.Survey);
        }
    }
}
