using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace NuSurvey.Core.Domain
{
    public class PhotoTag : DomainObject
    {
        public virtual string Name { get; set; }
        public virtual Photo Photo { get; set; }
    }

    public class PhotoTagMap : ClassMap<PhotoTag>
    {
        public PhotoTagMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            References(x => x.Photo);
        }
    }
}
