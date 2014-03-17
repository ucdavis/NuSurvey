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
            
        }
        public User(string id) : this()
        {
            Id = id == null ? null : id.ToLower();
        }
        [Required]
        [StringLength(100)]
        public virtual string Firstname { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string LastName { get; set; }

        [StringLength(100)]
        public virtual string Title { get; set; }

        [StringLength(250)]
        [Display(Name = "Agency/Institution")]
        public virtual string Agency { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string City { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string State { get; set; }
    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Firstname);
            Map(x => x.LastName);
            Map(x => x.Title);
            Map(x => x.Agency);
            Map(x => x.City);
            Map(x => x.State);
        }
    }
}
