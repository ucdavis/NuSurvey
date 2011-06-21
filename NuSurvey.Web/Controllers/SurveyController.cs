using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.Helpers;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Survey class
    /// </summary>
    [Admin]
    public class SurveyController : ApplicationController
    {
	    private readonly IRepository<Survey> _surveyRepository;

        public SurveyController(IRepository<Survey> surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }
    
        /// <summary>
        /// #1
        /// GET: /Survey/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var surveyList = _surveyRepository.Queryable;

            return View(surveyList.ToList());
        }

        /// <summary>
        /// #2
        /// GET: /Survey/Details/5
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="filterBeginDate"></param>
        /// <param name="filterEndDate"></param>
        /// <returns></returns>
        public ActionResult Details(int id, DateTime? filterBeginDate, DateTime? filterEndDate)
        {
            var survey = _surveyRepository.GetNullableById(id);

            if (survey == null)
            {
                return this.RedirectToAction(a => a.Index());
                //return RedirectToAction("Index");
            }

            var viewModel = SurveyResponseDetailViewModel.Create(Repository, survey, filterBeginDate, filterEndDate);

            return View(viewModel);
        }

        /// <summary>
        /// #3
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <returns></returns>
        public ActionResult PendingDetails(int id)
        {
            var survey = _surveyRepository.GetNullableById(id);

            if (survey == null)
            {
                return this.RedirectToAction(a => a.Index());
                //return RedirectToAction("Index");
            }

            var viewModel = SurveyPendingResponseDetailViewModel.Create(Repository, survey);

            return View(viewModel);
        }

        /// <summary>
        /// #4
        /// GET: /Survey/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
			var viewModel = SurveyViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        /// <summary>
        /// #5
        /// POST: /Survey/Create
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Survey survey)
        {
            //Not sure if this is really needed
            //ModelState.Clear();
            //survey.TransferValidationMessagesTo(ModelState); 

            if (ModelState.IsValid)
            {
                _surveyRepository.EnsurePersistent(survey);

                Message = "Survey Created Successfully";

                return this.RedirectToAction(a => a.Edit(survey.Id));
            }
            else
            {
				var viewModel = SurveyViewModel.Create(Repository);
                viewModel.Survey = survey;

                return View(viewModel);
            }
        }

        /// <summary>
        /// #6
        /// GET: /Survey/Edit/5
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var survey = _surveyRepository.GetNullableById(id);

            if (survey == null)
            {
                return this.RedirectToAction(a => a.Index());
            }

			return View(survey);
        }
        
        /// <summary>
        /// #7
        /// POST: /Survey/Edit/5
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="survey"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, Survey survey)
        {
            //TODO: Add validation where active must have other values?
            var surveyToEdit = _surveyRepository.GetNullableById(id);

            if (surveyToEdit == null)
            {
                return this.RedirectToAction(a => a.Index());
            }

            Mapper.Map(survey, surveyToEdit);

            ModelState.Clear();
            surveyToEdit.TransferValidationMessagesTo(ModelState);


            if (ModelState.IsValid)
            {
                _surveyRepository.EnsurePersistent(surveyToEdit);

                Message = "Survey Edited Successfully";

                return this.RedirectToAction(a => a.Edit(surveyToEdit.Id));
            }
            else
            {
                return View(surveyToEdit);
            }
        }
        
    }

	/// <summary>
    /// ViewModel for the Survey class
    /// </summary>
    public class SurveyViewModel
	{
		public Survey Survey { get; set; }
 
		public static SurveyViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new SurveyViewModel {Survey = new Survey()};
 
			return viewModel;
		}
	}

    public class SurveyResponseDetailViewModel
    {
        public Survey Survey { get; set; }
        public DateTime? FilterBeginDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public IEnumerable<SurveyResponse> SurveyResponses { get; set; }
        public bool HasPendingResponses { get; set; }

        public static SurveyResponseDetailViewModel Create(IRepository repository, Survey survey, DateTime? beginDate, DateTime? endDate)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null, "Survey must be supplied");

            var viewModel = new SurveyResponseDetailViewModel { Survey = survey, FilterBeginDate = beginDate, FilterEndDate = endDate};
            viewModel.SurveyResponses = viewModel.Survey.SurveyResponses.AsQueryable().Where(a => !a.IsPending);
            viewModel.HasPendingResponses = viewModel.Survey.SurveyResponses.Where(a => a.IsPending).Any();
            if (beginDate != null)
            {
                beginDate = beginDate.Value.Date;
                viewModel.SurveyResponses = viewModel.SurveyResponses.Where(a => a.DateTaken >= beginDate);
            }
            if (endDate != null)
            {
                endDate = endDate.Value.Date.AddDays(1).AddMinutes(-1);
                viewModel.SurveyResponses = viewModel.SurveyResponses.Where(a => a.DateTaken <= endDate);
            }

            return viewModel;
        }
    }

    public class SurveyPendingResponseDetailViewModel
    {
        public Survey Survey { get; set; }
        public IEnumerable<SurveyResponse> SurveyResponses { get; set; }

        public static SurveyPendingResponseDetailViewModel Create(IRepository repository, Survey survey)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null, "Survey must be supplied");

            var viewModel = new SurveyPendingResponseDetailViewModel { Survey = survey };
            viewModel.SurveyResponses = viewModel.Survey.SurveyResponses.AsQueryable().Where(a => a.IsPending);

            return viewModel;
        }
    }
}
