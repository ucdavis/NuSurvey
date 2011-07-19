using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

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
        [Display(Name = "Goal")]
        [DataType(DataType.MultilineText)]
        public virtual string Name { get; set; }

        [Display(Name = "Active")]
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
