using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace NuSurvey.Core.Domain
{
    public class Photo : DomainObject
    {
        public Photo()
        {
            DateCreated = DateTime.Now;

            PhotoTags = new List<PhotoTag>();
            Questions = new List<Question>();
        }

        public virtual string Name { get; set; }
        public virtual string FileName { get; set; }
        public virtual string ContentType { get; set; }
        public virtual byte[] FileContents { get; set; } 
        public virtual DateTime DateCreated { get; set; }
        public virtual byte[] ThumbNail { get; set; }

        public virtual IList<PhotoTag> PhotoTags { get; set; }
        public virtual IList<Question> Questions { get; set; }
    }

    public class PhotoMap : ClassMap<Photo>
    {
        public PhotoMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.FileName);
            Map(x => x.ContentType);
            Map(x => x.FileContents).CustomType("BinaryBlob");
            Map(x => x.DateCreated);
            Map(x => x.ThumbNail);

            HasMany(x => x.PhotoTags).Inverse();

            HasManyToMany(x => x.Questions).Table("QuestionsXPhotos").ParentKeyColumn("PhotoId").ChildKeyColumn("QuestionId");
        }
    }
}