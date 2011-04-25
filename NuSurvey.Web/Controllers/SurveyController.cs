using System;
using System.Linq;
using System.Web.Mvc;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

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
        public ActionResult Details(int id)
        {
            var survey = _surveyRepository.GetNullableById(id);

            if (survey == null) return RedirectToAction("Index");          

            return View(survey.SurveyResponses);
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
            var surveyToCreate = new Survey();

            TransferValues(survey, surveyToCreate);

            if (ModelState.IsValid)
            {
                _surveyRepository.EnsurePersistent(surveyToCreate);

                Message = "Survey Created Successfully";

                return RedirectToAction("Index");
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

            if (survey == null) return RedirectToAction("Index");

			var viewModel = SurveyViewModel.Create(Repository);
			viewModel.Survey = survey;

			return View(viewModel);
        }
        
        //
        // POST: /Survey/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Survey survey)
        {
            var surveyToEdit = _surveyRepository.GetNullableById(id);

            if (surveyToEdit == null) return RedirectToAction("Index");

            TransferValues(survey, surveyToEdit);

            if (ModelState.IsValid)
            {
                _surveyRepository.EnsurePersistent(surveyToEdit);

                Message = "Survey Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = SurveyViewModel.Create(Repository);
                viewModel.Survey = survey;

                return View(viewModel);
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
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Survey source, Survey destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
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
}
