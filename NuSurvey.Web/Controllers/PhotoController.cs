using System;
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

            return View(photoList.ToList());
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
}
