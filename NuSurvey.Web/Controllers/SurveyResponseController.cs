using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NuSurvey.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the SurveyResponse class
    /// </summary>
    [Authorize]
    public class SurveyResponseController : ApplicationController
    {
	    private readonly IRepository<SurveyResponse> _surveyResponseRepository;

        public SurveyResponseController(IRepository<SurveyResponse> surveyResponseRepository)
        {
            _surveyResponseRepository = surveyResponseRepository;
        }
    
        //
        // GET: /SurveyResponse/
        public ActionResult Index()
        {
            //var surveyResponseList = _surveyResponseRepository.Queryable;

            //return View(surveyResponseList.ToList());

            var activeSurveyList = Repository.OfType<Survey>().Queryable.Where(a => a.IsActive);

            return View(activeSurveyList);
        }

        //
        // GET: /SurveyResponse/Details/5
        //public ActionResult Details(int id)
        //{
        //    var surveyResponse = _surveyResponseRepository.GetNullableById(id);

        //    if (surveyResponse == null) return RedirectToAction("Index");

        //    return View(surveyResponse);
        //}

        /// <summary>
        /// GET: /SurveyResponse/Create
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null || !survey.IsActive)
            {
                Message = "Survey not found or not active.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
			var viewModel = SurveyResponseViewModel.Create(Repository, survey);
            
            return View(viewModel);
        } 

        //
        // POST: /SurveyResponse/Create
        [HttpPost]
        public ActionResult Create(int id, SurveyResponse surveyResponse, QuestionAnswerParameter[] questions)
        {
            var surveyResponseToCreate = new SurveyResponse();

            TransferValues(surveyResponse, surveyResponseToCreate);

            if (ModelState.IsValid)
            {
                _surveyResponseRepository.EnsurePersistent(surveyResponseToCreate);

                Message = "SurveyResponse Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = SurveyResponseViewModel.Create(Repository, null); //TODO: FIx
                viewModel.SurveyResponse = surveyResponse;

                return View(viewModel);
            }
        }

        //
        // GET: /SurveyResponse/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    var surveyResponse = _surveyResponseRepository.GetNullableById(id);

        //    if (surveyResponse == null) return RedirectToAction("Index");

        //    var viewModel = SurveyResponseViewModel.Create(Repository);
        //    viewModel.SurveyResponse = surveyResponse;

        //    return View(viewModel);
        //}
        
        //
        // POST: /SurveyResponse/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, SurveyResponse surveyResponse)
        //{
        //    var surveyResponseToEdit = _surveyResponseRepository.GetNullableById(id);

        //    if (surveyResponseToEdit == null) return RedirectToAction("Index");

        //    TransferValues(surveyResponse, surveyResponseToEdit);

        //    if (ModelState.IsValid)
        //    {
        //        _surveyResponseRepository.EnsurePersistent(surveyResponseToEdit);

        //        Message = "SurveyResponse Edited Successfully";

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        var viewModel = SurveyResponseViewModel.Create(Repository);
        //        viewModel.SurveyResponse = surveyResponse;

        //        return View(viewModel);
        //    }
        //}
        
        //
        // GET: /SurveyResponse/Delete/5 
        //public ActionResult Delete(int id)
        //{
        //    var surveyResponse = _surveyResponseRepository.GetNullableById(id);

        //    if (surveyResponse == null) return RedirectToAction("Index");

        //    return View(surveyResponse);
        //}

        //
        // POST: /SurveyResponse/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, SurveyResponse surveyResponse)
        //{
        //    var surveyResponseToDelete = _surveyResponseRepository.GetNullableById(id);

        //    if (surveyResponseToDelete == null) return RedirectToAction("Index");

        //    _surveyResponseRepository.Remove(surveyResponseToDelete);

        //    Message = "SurveyResponse Removed Successfully";

        //    return RedirectToAction("Index");
        //}
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(SurveyResponse source, SurveyResponse destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the SurveyResponse class
    /// </summary>
    public class SurveyResponseViewModel
	{
		public SurveyResponse SurveyResponse { get; set; }
        public IList<Question> Questions { get; set; }
        public Survey Survey { get; set; }
 
		public static SurveyResponseViewModel Create(IRepository repository, Survey survey)
		{
			Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null);
			
			var viewModel = new SurveyResponseViewModel {SurveyResponse = new SurveyResponse(), Survey = survey};
		    viewModel.Questions = viewModel.Survey.Questions
                .Where(a => a.IsActive && a.Category != null && a.Category.IsActive)
                .OrderBy(a => a.Order).ToList();            
			return viewModel;
		}
	}

    public class QuestionAnswerParameter
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }
    }
}
