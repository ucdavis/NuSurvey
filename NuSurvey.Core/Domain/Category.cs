using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace NuSurvey.Core.Domain
{
    public class Category : DomainObject
    {
        #region Constructor
        public Category()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            LastUpdate = DateTime.Now;
        }
        #endregion Constructor


        [Required]
        [StringLength(100)]
        public virtual string Name { get; set; }
        public virtual int Rank { get; set; }
        [Required]
        public virtual string Affirmation { get; set; }
        [Required]
        public virtual string Encouragement { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool DoNotUseForCalculations { get; set; }

        /// <summary>
        /// This should be updated for every save
        /// </summary>
        public virtual DateTime LastUpdate { get; set; }

        public virtual DateTime CreateDate { get; set; }
        [Required]
        public virtual Survey Survey { get; set; }
    }

    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Rank);
            Map(x => x.Affirmation);
            Map(x => x.Encouragement);
            Map(x => x.IsActive);
            Map(x => x.LastUpdate);
            Map(x => x.CreateDate);
            Map(x => x.DoNotUseForCalculations);

            References(x => x.Survey);
        }
    }
}
