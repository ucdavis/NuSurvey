using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.ActionResults;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the ProgramDirector class
    /// </summary>
    [Authorize]
    public class ProgramDirectorController : ApplicationController
    {
        private readonly IRepository<Survey> _surveyRepository;
        private readonly IRepository<PrintedSurvey> _printedSurveyRepository;
        private readonly IRepository<Photo> _photoRepository;
        private readonly IPrintService _printService;


        public ProgramDirectorController(IRepository<Survey> surveyRepository, IRepository<PrintedSurvey> printedSurveyRepository, IRepository<Photo> photoRepository, IPrintService printService)
        {
            _surveyRepository = surveyRepository;
            _printedSurveyRepository = printedSurveyRepository;
            _photoRepository = photoRepository;
            _printService = printService;
        }

        //
        // GET: /ProgramDirector/
        [ProgramDirector]
        public ActionResult Index()
        {
            var userId = CurrentUser.Identity.Name;

            var viewModel = ProgramDirectorViewModel.Create();
            viewModel.Surveys = _surveyRepository.Queryable.Where(a => a.IsActive).ToList();
            viewModel.PrintedSurveys = _printedSurveyRepository.Queryable.Where(a => a.UserId == userId).ToList();

            return View(viewModel);
        }

        [ProgramDirector]
        public ActionResult Create(int surveyId)
        {
            if (_printedSurveyRepository.Queryable.Count(a => a.UserId == CurrentUser.Identity.Name) >= 100)
            {
                Message = "Maximum surveys reached, please delete existing surveys before duplicating more.";
                return this.RedirectToAction(a => a.Index());
            }

            var survey = _surveyRepository.Queryable.Single(a => a.Id == surveyId && a.IsActive);
            var printedSurvey = new PrintedSurvey();
            printedSurvey.Survey = survey;
            printedSurvey.UserId = CurrentUser.Identity.Name;


            foreach (var question in survey.Questions.Where(a => a.Category.IsCurrentVersion && a.Category.IsCurrentVersion && a.IsActive))
            {
                var psq = new PrintedSurveyQuestion();
                psq.PrintedSurvey = printedSurvey;
                psq.Question = question;
                psq.Photo = question.PrimaryPhoto;
                psq.Order = question.Order;
                if (psq.Photo == null || psq.Photo.IsPrintable == false || psq.Photo.IsActive == false)
                {
                    psq.Photo = question.Photos.FirstOrDefault(a => a.IsPrintable && a.IsActive);
                }

                printedSurvey.PrintedSurveyQuestions.Add(psq);
            }

            _printedSurveyRepository.EnsurePersistent(printedSurvey);

            return this.RedirectToAction(a => a.Index());
        }

        [ProgramDirector]
        public ActionResult Delete(int id)
        {
            var userId = CurrentUser.Identity.Name;
            var printedSurvey = _printedSurveyRepository.Queryable.Single(a => a.Id == id && a.UserId == userId);

            _printedSurveyRepository.Remove(printedSurvey);

            return this.RedirectToAction(a => a.Index());
        }

        [ProgramDirector]
        public ActionResult SelectPhotos(int id)
        {
            var userId = CurrentUser.Identity.Name;
            var printedSurvey = _printedSurveyRepository.Queryable.Single(a => a.Id == id && a.UserId == userId);

            return View(printedSurvey);
        }

        [ProgramDirector]
        public JsonNetResult MakePrimaryPhoto(int id, int printedSurveyId, int psqId)
        {
            var success = false;
            var message = string.Empty;
            try
            {
                var photo = _photoRepository.Queryable.Single(a => a.Id == id);
                var printedSurvey = _printedSurveyRepository.Queryable.Single(a => a.Id == printedSurveyId && a.UserId == CurrentUser.Identity.Name);
                var question = printedSurvey.PrintedSurveyQuestions.Single(a => a.Id == psqId);
                if (question.Question.Photos.Contains(photo))
                {
                    question.Photo = photo;
                    _printedSurveyRepository.EnsurePersistent(printedSurvey);
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

        [ProgramDirector]
        public ActionResult Print(int id)
        {
            var userId = CurrentUser.Identity.Name;
            var printedSurvey = _printedSurveyRepository.Queryable.Single(a => a.Id == id && a.UserId == userId);

            var printedFile = _printService.PrintDirector(printedSurvey, Request, Url);

            return File(printedFile.FileContents, printedFile.ContentType, string.Format("{0}-{1}.pdf", printedSurvey.Survey.ShortName.Trim(), id));
        }

    }

	/// <summary>
    /// ViewModel for the ProgramDirector class
    /// </summary>
    public class ProgramDirectorViewModel
	{
		public List<Survey> Surveys { get; set; }
        public List<PrintedSurvey> PrintedSurveys { get; set; } 
 
		public static ProgramDirectorViewModel Create()
		{		
			var viewModel = new ProgramDirectorViewModel();
 
			return viewModel;
		}
	}

    
}
