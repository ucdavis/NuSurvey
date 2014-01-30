using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            IsActive = true;
            IsPrintable = true;
        }
        [Required]
        [StringLength(100)]
        public virtual string Name { get; set; }
        public virtual string FileName { get; set; }
        public virtual string ContentType { get; set; }
        //public virtual byte[] FileContents { get; set; } 
        public virtual DateTime DateCreated { get; set; }
        //public virtual byte[] ThumbNail { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual bool IsPrintable { get; set; }

        public virtual IList<PhotoTag> PhotoTags { get; set; }
        public virtual IList<Question> Questions { get; set; }

        public virtual void AddTag(string tagText)
        {
            var tag = new PhotoTag();
            tag.Name = tagText;
            tag.Photo = this;
            PhotoTags.Add(tag);
        }
    }

    public class PhotoMap : ClassMap<Photo>
    {
        public PhotoMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.FileName);
            Map(x => x.ContentType);
            //Map(x => x.FileContents).CustomType("BinaryBlob");
            Map(x => x.DateCreated);
            //Map(x => x.ThumbNail);
            Map(x => x.IsActive);
            Map(x => x.IsPrintable);

            HasMany(x => x.PhotoTags).Inverse();

            HasManyToMany(x => x.Questions).Table("QuestionsXPhotos").ParentKeyColumn("PhotoId").ChildKeyColumn("QuestionId");
        }
    }
}