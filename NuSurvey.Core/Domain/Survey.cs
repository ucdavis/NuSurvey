using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace NuSurvey.Core.Domain
{
    public class Survey : DomainObject
    {
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }
    }

    public class SurveyMap : ClassMap<Survey>
    {
        public SurveyMap()
        {
            Map(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsActive);
        }
    }
}
