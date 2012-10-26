using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Photo class
    /// </summary>
    [Admin]
    public class PhotoController : ApplicationController
    {
	    private readonly IRepository<Photo> _photoRepository;
        private readonly IPictureService _pictureService;

        public PhotoController(IRepository<Photo> photoRepository, IPictureService pictureService)
        {
            _photoRepository = photoRepository;
            _pictureService = pictureService;
        }

        //
        // GET: /Photo/
        public ActionResult Index()
        {
            var photoList = _photoRepository.Queryable;

            return View(photoList.Where(a => a.IsActive).ToList());
        }


        public ActionResult Upload(int? questionId)
        {
            var viewModel = PhotoEditModel.Create(questionId);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upload(PhotoEditModel photoEditModel, HttpPostedFileBase uploadedPhoto)
        {
            var photo = new Photo();
            Check.Require(uploadedPhoto != null);

            // read the file and set the original picture
            var reader = new BinaryReader(uploadedPhoto.InputStream);
            photo.FileContents = reader.ReadBytes(uploadedPhoto.ContentLength);
            photo.ContentType = uploadedPhoto.ContentType;
            photo.Name = photoEditModel.Photo.Name;
            photo.FileName = uploadedPhoto.FileName;

            photo.ThumbNail = _pictureService.MakeThumbnail(photo.FileContents);
            if (!string.IsNullOrWhiteSpace(photoEditModel.Tags))
            {
                foreach (var tag in photoEditModel.Tags.Split(','))
                {
                    photo.AddTag(tag);
                }
            }

            if (photoEditModel.QuestionId.HasValue)
            {
                var question = Repository.OfType<Question>().GetById(photoEditModel.QuestionId.Value);
                if (question != null)
                {
                    photo.Questions.Add(question);
                }
            }


            _photoRepository.EnsurePersistent(photo);
            foreach (var tag in photo.PhotoTags)
            {
                Repository.OfType<PhotoTag>().EnsurePersistent(tag);
            }

            return this.RedirectToAction(a => a.Index());
        }

        public ActionResult Edit(int id, PhotoEditModel photoEditModel)
        {
            var photo = _photoRepository.GetNullableById(id);
            if (photo == null)
            {
                Message = "Photo not found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = PhotoEditModel.Create(null);
            viewModel.Photo = photo;

            var tags = new List<string>();
            foreach (var photoTag in photo.PhotoTags)
            {
                tags.Add(photoTag.Name);
            }

            viewModel.Tags = string.Join(",", tags.ToArray());

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, PhotoEditModel photoEditModel, HttpPostedFileBase uploadedPhoto)
        {
            var photo = _photoRepository.GetNullableById(id);
            if (photo == null)
            {
                Message = "Photo not found";
                return this.RedirectToAction(a => a.Index());
            }
            if (uploadedPhoto != null)
            {
                // read the file and set the original picture
                var reader = new BinaryReader(uploadedPhoto.InputStream);
                photo.FileContents = reader.ReadBytes(uploadedPhoto.ContentLength);
                photo.ContentType = uploadedPhoto.ContentType;
                photo.FileName = uploadedPhoto.FileName;
                photo.DateCreated = DateTime.Now;

                photo.ThumbNail = _pictureService.MakeThumbnail(photo.FileContents);
            }
            photo.Name = photoEditModel.Photo.Name;

            //TODO: Test tags...
            var photoTags = new List<PhotoAction>();
            if (!string.IsNullOrWhiteSpace(photoEditModel.Tags))
            {
                foreach (var tag in photoEditModel.Tags.Split(','))
                {
                    var photoAction = new PhotoAction();
                    var pTag = photo.PhotoTags.FirstOrDefault(a => a.Name == tag);
                    if (pTag == null)
                    {
                        pTag = new PhotoTag();
                        pTag.Name = tag;
                        pTag.Photo = photo;
                        photoAction.Action = "Add";
                    }
                    else
                    {
                        photoAction.Action = "Keep";
                    }
                    photoAction.PhotoTag = pTag;
                    photoTags.Add(photoAction);
                }
            }
            foreach (var existingTags in photo.PhotoTags)
            {
                if (!photoTags.Any(a => a.PhotoTag.Id == existingTags.Id))
                {
                    var pTag = new PhotoAction();
                    pTag.PhotoTag = existingTags;
                    pTag.Action = "Remove";
                    photoTags.Add(pTag);
                }
            }
            foreach (var photoAction in photoTags)
            {
                if (photoAction.Action == "Remove")
                {
                    Repository.OfType<PhotoTag>().Remove(photoAction.PhotoTag);
                }
                else if(photoAction.Action == "Add")
                {
                    Repository.OfType<PhotoTag>().EnsurePersistent(photoAction.PhotoTag);
                }
            }

            Message = "Photo Updated";

            _photoRepository.EnsurePersistent(photo);
            return this.RedirectToAction(a => a.Index());
        }


        public ActionResult Details(int id)
        {
            var photo = _photoRepository.GetNullableById(id);
            if (photo == null)
            {
                Message = "Photo not found";
                return this.RedirectToAction(a => a.Index());
            }

            return View(photo);
        }

        public ActionResult Delete(int id)
        {
            var photo = _photoRepository.GetNullableById(id);
            if (photo == null)
            {
                Message = "Photo not found";
                return this.RedirectToAction(a => a.Index());
            }

            return View(photo);
        }

        [HttpPost]
        public ActionResult Delete(Photo photo)
        {
            var photoToDelete = _photoRepository.GetNullableById(photo.Id);
            if (photoToDelete == null)
            {
                Message = "Photo not found";
            }
            else
            {

                photoToDelete.IsActive = false;
                _photoRepository.EnsurePersistent(photoToDelete);
            }

            return this.RedirectToAction(a => a.Index());
        }

        public ActionResult GetThumbnail(int id)
        {
            var photo = _photoRepository.GetById(id);

            if (photo == null)
            {
                return File(new byte[0], "image/jpg");
            }

            if (photo.ThumbNail != null)
            {
                return File( photo.ThumbNail, "image/jpg");
            }
            else
            {
                return File(new byte[0], "image/jpg");
            }
        }

        public ActionResult GetPhoto(int id)
        {
            var photo = _photoRepository.GetById(id);

            if (photo == null)
            {
                return File(new byte[0], "image/jpg");
            }

            if (photo.ThumbNail != null)
            {
                return File(photo.FileContents, "image/jpg");
            }
            else
            {
                return File(new byte[0], "image/jpg");
            }
        }

    }

	/// <summary>
    /// ViewModel for the Photo class
    /// </summary>
    public class PhotoEditModel
	{
		public Photo Photo { get; set; }
        public string Tags { get; set; }
        public int? QuestionId { get; set; }
 
		public static PhotoEditModel Create(int? questionId)
		{
			
			var viewModel = new PhotoEditModel {Photo = new Photo(), QuestionId = questionId};
 
			return viewModel;
		}

	}
    public class PhotoAction
    {
        public PhotoTag PhotoTag { get; set; }
        public string Action { get; set; }
    }
}
