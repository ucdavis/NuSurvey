using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuSurvey.Core.Domain;
using NuSurvey.MVC.Controllers.Filters;
using NuSurvey.MVC.Resources;
using NuSurvey.MVC.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.ActionResults;

namespace NuSurvey.MVC.Controllers
{
    /// <summary>
    /// Controller for the Photo class
    /// </summary>
    [Admin]
    public class PhotoController : ApplicationController
    {
	    private readonly IRepository<Photo> _photoRepository;
        private readonly IRepository<PhotoTag> _photoTagRepository;
        private readonly IPictureService _pictureService;
        private readonly IRepository<Question> _questionRepository;
        private readonly IBlobStoargeService _blobStoargeService;

        public PhotoController(IRepository<Photo> photoRepository, IRepository<PhotoTag> photoTagRepository  ,IPictureService pictureService, IRepository<Question> questionRepository, IBlobStoargeService blobStoargeService)
        {
            _photoRepository = photoRepository;
            _photoTagRepository = photoTagRepository;
            _pictureService = pictureService;
            _questionRepository = questionRepository;
            _blobStoargeService = blobStoargeService;
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
            var viewModel = PhotoEditModel.Create(questionId, null, null, "E");

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upload(PhotoEditModel photoEditModel, HttpPostedFileBase uploadedPhoto)
        {
            var photo = new Photo();
            Check.Require(uploadedPhoto != null);

            // read the file and set the original picture
            var reader = new BinaryReader(uploadedPhoto.InputStream);
            var img = reader.ReadBytes(uploadedPhoto.ContentLength);
            photo.ContentType = uploadedPhoto.ContentType;
            photo.Name = photoEditModel.Photo.Name;
            photo.FileName = uploadedPhoto.FileName;
            photo.IsPrintable = photoEditModel.Photo.IsPrintable;

            //photo.ThumbNail = _pictureService.MakeThumbnail(photo.FileContents);
            if (!string.IsNullOrWhiteSpace(photoEditModel.Tags))
            {
                foreach (var tag in photoEditModel.Tags.Split(','))
                {
                    photo.AddTag(tag.Trim());
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

            _blobStoargeService.UploadPhoto(photo.Id, img);

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

            var viewModel = PhotoEditModel.Create(null, null, null, "E");
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
                var img = reader.ReadBytes(uploadedPhoto.ContentLength);
                photo.ContentType = uploadedPhoto.ContentType;
                photo.FileName = uploadedPhoto.FileName;
                photo.DateCreated = DateTime.Now;                

                //photo.ThumbNail = _pictureService.MakeThumbnail(photo.FileContents);
                _blobStoargeService.UploadPhoto(photo.Id, img);
            }
            photo.Name = photoEditModel.Photo.Name;
            photo.IsPrintable = photoEditModel.Photo.IsPrintable;

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
                        pTag.Name = tag.Trim();
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


        public ActionResult Details(int id, int? questionId, int? surveyId, int? categoryId, string editDetail)
        {
            var photo = _photoRepository.GetNullableById(id);
            if (photo == null)
            {
                Message = "Photo not found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = PhotoEditModel.Create(questionId, surveyId, categoryId, "D");
            viewModel.Photo = photo;
            viewModel.EditDetail = editDetail;
            if (questionId.HasValue)
            {
                var question = _questionRepository.GetNullableById(questionId.Value);
                if (question != null && question.Photos.Contains(photo))
                {
                    viewModel.PhotoIsRelatedToQuestion = true;
                }
            }

            return View(viewModel);
        }

        public JsonNetResult AddRemovePhotoFromQuestion(int id, int questionId, string action)
        {
            var success = false;
            var message = string.Empty;
            try
            {
                var photo = _photoRepository.Queryable.Single(a => a.Id == id);
                var question = _questionRepository.Queryable.Single(a => a.Id == questionId);
                switch (action)
                {
                    case "Add":
                        if (question.Photos.Contains(photo))
                        {
                            success = false;
                            message = "Photo was already added to question";
                        }
                        else
                        {
                            question.Photos.Add(photo);
                            if (question.PrimaryPhoto == null)
                            {
                                question.PrimaryPhoto = photo;
                            }
                            _questionRepository.EnsurePersistent(question);
                            success = true;
                            message = "Photo added to question";
                        }
                        break;
                    case "Remove":
                        if (question.Photos.Contains(photo))
                        {
                            question.Photos.Remove(photo);
                            if (question.PrimaryPhoto == photo)
                            {
                                question.PrimaryPhoto = question.Photos.FirstOrDefault(); //Removing it, so set it to the first one in the list , if any.
                            }
                            _questionRepository.EnsurePersistent(question);
                            success = true;
                            message = "Photo removed from question";
                        }
                        else
                        {
                            success = false;
                            message = "Photo not found attached to question";
                        }
                        break;
                    default:
                        throw new Exception("Action not found");
                        break;
                }
            }
            catch (Exception)
            {
                message = "An error prevented this action.";
                return new JsonNetResult(new { success, message });

            }

            return new JsonNetResult(new { success, message });

        }

        /// <summary>
        /// This will be for Admins as well as another role.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public JsonNetResult MakePrimaryPhoto(int id, int questionId)
        {
            var success = false;
            var message = string.Empty;
            try
            {
                var photo = _photoRepository.Queryable.Single(a => a.Id == id);
                var question = _questionRepository.Queryable.Single(a => a.Id == questionId);
                if (question.Photos.Contains(photo))
                {
                    question.PrimaryPhoto = photo;
                    _questionRepository.EnsurePersistent(question);
                    success = true;
                    message = "Question updated";
                }
                else
                {
                    message = "Photo not found";
                }
            }
            catch (Exception)
            {
                message = "An error prevented this action.";
                return new JsonNetResult(new { success, message });
            }
            return new JsonNetResult(new { success, message });
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

        public ActionResult Search(string tag, int? questionId, int? surveyId, int? categoryId, string editDetail)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                Message = "Tag not entered";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var viewModel = PhotoSearchModel.Create(questionId, surveyId, categoryId, editDetail);
            viewModel.PhotoTags = _photoTagRepository.Queryable.Where(a => a.Name.ToLower() == tag.ToLower()).ToList();
            viewModel.SearchTag = tag;
            viewModel.UniqueTags =
                _photoTagRepository.Queryable.OrderBy(a => a.Name).Select(b => b.Name).Distinct().ToList();

            return View(viewModel);
        }


        public ActionResult GetThumbnail(int id)
        {
            var photo = _photoRepository.GetById(id);

            if (photo == null)
            {
                return File(new byte[0], "image/jpg");
            }

            return File( _blobStoargeService.GetPhoto(photo.Id, Resource.Thumb), "image/jpg");

        }


        public ActionResult GetPhoto(int id)
        {
            var photo = _photoRepository.GetById(id);

            if (photo == null)
            {
                return File(new byte[0], "image/jpg");
            }

            return File(_blobStoargeService.GetPhoto(photo.Id, Resource.Water), "image/jpg");
            
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
        public int? SurveyId { get; set; }
        public int? CategoryId { get; set; }
        public string EditDetail { get; set; }
        public bool PhotoIsRelatedToQuestion { get; set; }
 
		public static PhotoEditModel Create(int? questionId, int? surveyId, int? categoryId, string editDetail)
		{
			
			var viewModel = new PhotoEditModel {Photo = new Photo(), QuestionId = questionId, SurveyId = surveyId, CategoryId = categoryId, EditDetail = editDetail};
		    viewModel.PhotoIsRelatedToQuestion = false;
			return viewModel;
		}

	}
    public class PhotoAction
    {
        public PhotoTag PhotoTag { get; set; }
        public string Action { get; set; }
    }

    public class PhotoSearchModel
    {
        public IEnumerable<PhotoTag> PhotoTags { get; set; }
        public int? QuestionId { get; set; }
        public int? SurveyId { get; set; }
        public int? CategoryId { get; set; }
        public string SearchTag { get; set; }
        public string EditDetail { get; set; }
        public IList<string> UniqueTags { get; set; } 

        public static PhotoSearchModel Create(int? questionId, int? surveyId, int? categoryId, string editDetail)
        {
            var viewModel = new PhotoSearchModel{QuestionId = questionId, SurveyId = surveyId, CategoryId = categoryId, EditDetail = editDetail};                        
            viewModel.UniqueTags = new List<string>();
            return viewModel;
        }

    }
}
