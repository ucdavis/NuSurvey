using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace NuSurvey.Core.Domain
{
    public class CategoryGoal : DomainObject
    {
        [Required]
        [StringLength(200)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        [Required]
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
