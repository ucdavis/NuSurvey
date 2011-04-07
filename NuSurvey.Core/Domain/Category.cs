using System;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace NuSurvey.Core.Domain
{
    public class Category : DomainObject
    {
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual int Rank { get; set; }
        [Required]
        public virtual string Affirmation { get; set; }
        [Required]
        public virtual string Encouragement { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime LastUpdate { get; set; }
        [NotNull]
        public virtual Survey Survey { get; set; }
    }

    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Map(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Rank);
            Map(x => x.Affirmation);
            Map(x => x.Encouragement);
            Map(x => x.IsActive);
            Map(x => x.LastUpdate);

            References(x => x.Survey);
        }
    }
}
