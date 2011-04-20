using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace NuSurvey.Core.Domain
{
    public class CategoryGoal : DomainObject
    {
        [Required]
        [Length(200)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        [NotNull]
        public virtual Category Category { get; set; }
    }

    public class CategoryGoalMap : ClassMap<CategoryGoal>
    {
        public CategoryGoalMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsActive);

            References(x => x.Category);
        }
    }
}
