using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace NuSurvey.Core.Domain
{
    public class CategoryGoal : DomainObject
    {
        #region Constructor
        public  CategoryGoal(Category category)
        {
            SetDefaults();
            Category = category;
        }

        public CategoryGoal()
        {
            SetDefaults();
        }

        public virtual void SetDefaults()
        {
            IsActive = true;
        }

        #endregion Constructor

        [Required]
        [StringLength(200)]
        [DisplayName("Goal")]
        [DataType(DataType.MultilineText)]
        public virtual string Name { get; set; }

        [DisplayName("Active")]
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
