using System.ComponentModel.DataAnnotations;
using System.Data;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;


namespace NuSurvey.Core.Domain
{
    public class User : DomainObjectWithTypedId<string>
    {
        public User()
        {
            TargetPopulationWic = false;
            TargetPopulationSnap = false;
            TargetPopulationHeadStart = false;
            TargetPopulationEfnep = false;
            TargetPopulationLowIncome = false;
            TargetPopulationOther = false;
        }
        public User(string id) : this()
        {
            Id = id == null ? null : id.ToLower();
        }
        [Required]
        [StringLength(200)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(200)]
        public virtual string Title { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Agency/Institution")]
        public virtual string Agency { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string Street { get; set; }


        [Required]
        [StringLength(100)]
        public virtual string City { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string State { get; set; }

        [Required]
        [StringLength(11)]
        public virtual string Zip { get; set; }
        
        [Display(Name = "WIC")]
        public virtual bool TargetPopulationWic { get; set; }

        [Display(Name = "SNAP")]
        public virtual bool TargetPopulationSnap { get; set; }

        [Display(Name = "Head Start")]
        public virtual bool TargetPopulationHeadStart { get; set; }

        [Display(Name = "EFNEP")]
        public virtual bool TargetPopulationEfnep { get; set; }

        [Display(Name = "Low-income")]
        public virtual bool TargetPopulationLowIncome { get; set; }

        [Display(Name = "Other ")]
        public virtual bool TargetPopulationOther { get; set; }

    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.Title);
            Map(x => x.Agency);
            Map(x => x.Street);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.Zip);
            Map(x => x.TargetPopulationWic);
            Map(x => x.TargetPopulationSnap);
            Map(x => x.TargetPopulationHeadStart);
            Map(x => x.TargetPopulationEfnep);
            Map(x => x.TargetPopulationLowIncome);
            Map(x => x.TargetPopulationOther);
        }
    }
}
