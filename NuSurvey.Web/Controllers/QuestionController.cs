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
    /// Controller for the Question class
    /// </summary>
    public class QuestionController : ApplicationController
    {
	    private readonly IRepository<Question> _questionRepository;

        public QuestionController(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }
    
        ////
        //// GET: /Question/
        //public ActionResult Index()
        //{
        //    var questionList = _questionRepository.Queryable;

        //    return View(questionList.ToList());
        //}

        //
        // GET: /Question/Details/5
        public ActionResult Details(int id)
        {
            var question = _questionRepository.GetNullableById(id);

            if (question == null) return RedirectToAction("Index");

            return View(question);
        }

        //
        // GET: /Question/Create
        public ActionResult Create(int id, int? categoryId)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null)
            {
                Message = "Survey Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
			var viewModel = QuestionViewModel.Create(Repository, survey);
            if (categoryId != null)
            {
                var category = Repository.OfType<Category>().GetNullableById(categoryId.Value);
                viewModel.Category = category;
            }
            
            return View(viewModel);
        } 

        //
        // POST: /Question/Create
        [HttpPost]
        public ActionResult Create(int id, Question question, ResponsesParameter[] response, string sortOrder)
        {
            var questionToCreate = new Question();

            TransferValues(question, questionToCreate);

            if (ModelState.IsValid)
            {
                _questionRepository.EnsurePersistent(questionToCreate);

                Message = "Question Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = QuestionViewModel.Create(Repository, null);
                viewModel.Question = question;

                return View(viewModel);
            }
        }

        //
        // GET: /Question/Edit/5
        public ActionResult Edit(int id)
        {
            var question = _questionRepository.GetNullableById(id);

            if (question == null) return RedirectToAction("Index");

			var viewModel = QuestionViewModel.Create(Repository, null);
			viewModel.Question = question;

			return View(viewModel);
        }
        
        //
        // POST: /Question/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Question question)
        {
            var questionToEdit = _questionRepository.GetNullableById(id);

            if (questionToEdit == null) return RedirectToAction("Index");

            TransferValues(question, questionToEdit);

            if (ModelState.IsValid)
            {
                _questionRepository.EnsurePersistent(questionToEdit);

                Message = "Question Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = QuestionViewModel.Create(Repository, null);
                viewModel.Question = question;

                return View(viewModel);
            }
        }
        
        //
        // GET: /Question/Delete/5 
        public ActionResult Delete(int id)
        {
			var question = _questionRepository.GetNullableById(id);

            if (question == null) return RedirectToAction("Index");

            return View(question);
        }

        //
        // POST: /Question/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Question question)
        {
			var questionToDelete = _questionRepository.GetNullableById(id);

            if (questionToDelete == null) return RedirectToAction("Index");

            _questionRepository.Remove(questionToDelete);

            Message = "Question Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Question source, Question destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the Question class
    /// </summary>
    public class QuestionViewModel
	{
		public Question Question { get; set; }
        public Survey Survey { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public Category Category { get; set; }
        public IEnumerable<SelectListItem> CategoryPick { get; set; }
        public string SortOrder { get; set; }
 
		public static QuestionViewModel Create(IRepository repository, Survey survey)
		{
			Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null);
			
			var viewModel = new QuestionViewModel {Question = new Question(), Survey = survey};
		    viewModel.Categories = viewModel.Survey.Categories.Where(a => a.IsCurrentVersion).OrderBy(a => a.Rank);
 
			return viewModel;
		}
	}

    public class ResponsesParameter
    {
        public string Value { get; set; }
        public int Score { get; set; }
    }
}
