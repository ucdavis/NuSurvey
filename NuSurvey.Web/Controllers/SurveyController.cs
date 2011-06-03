using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;

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
    
        //
        // GET: /Survey/
        public ActionResult Index()
        {
            var surveyList = _surveyRepository.Queryable;

            return View(surveyList.ToList());
        }

        //
        // GET: /Survey/Details/5
        public ActionResult Details(int id, DateTime? filterBeginDate, DateTime? filterEndDate)
        {
            var survey = _surveyRepository.GetNullableById(id);

            if (survey == null) return RedirectToAction("Index");

            var viewModel = SurveyResponseDetailViewModel.Create(Repository, survey, filterBeginDate, filterEndDate);

            return View(viewModel);
        }

        //
        // GET: /Survey/Create
        public ActionResult Create()
        {
			var viewModel = SurveyViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /Survey/Create
        [HttpPost]
        public ActionResult Create(Survey survey)
        {
            //var surveyToCreate = new Survey();

            //TransferValues(survey, surveyToCreate);

            if (ModelState.IsValid)
            {
                //_surveyRepository.EnsurePersistent(surveyToCreate);
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

        //
        // GET: /Survey/Edit/5
        public ActionResult Edit(int id)
        {
            var survey = _surveyRepository.GetNullableById(id);

            if (survey == null)
            {
                return this.RedirectToAction(a => a.Index());
            }

			return View(survey);
        }
        
        //
        // POST: /Survey/Edit/5
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
        
        //
        // GET: /Survey/Delete/5 
        public ActionResult Delete(int id)
        {
			var survey = _surveyRepository.GetNullableById(id);

            if (survey == null) return RedirectToAction("Index");

            return View(survey);
        }

        //
        // POST: /Survey/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Survey survey)
        {
			var surveyToDelete = _surveyRepository.GetNullableById(id);

            if (surveyToDelete == null) return RedirectToAction("Index");

            _surveyRepository.Remove(surveyToDelete);

            Message = "Survey Removed Successfully";

            return RedirectToAction("Index");
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

        public static SurveyResponseDetailViewModel Create(IRepository repository, Survey survey, DateTime? beginDate, DateTime? endDate)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null, "Survey must be supplied");

            var viewModel = new SurveyResponseDetailViewModel { Survey = survey, FilterBeginDate = beginDate, FilterEndDate = endDate};
            viewModel.SurveyResponses = viewModel.Survey.SurveyResponses.AsQueryable();
            if (beginDate != null)
            {
                beginDate = beginDate.Value.Date;
                viewModel.SurveyResponses = viewModel.SurveyResponses.Where(a => a.DateTaken > beginDate);
            }
            if (endDate != null)
            {
                endDate = endDate.Value.Date.AddDays(1).AddMinutes(-1);
                viewModel.SurveyResponses = viewModel.SurveyResponses.Where(a => a.DateTaken < endDate);
            }

            return viewModel;
        }
    }
}
