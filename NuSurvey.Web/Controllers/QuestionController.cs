using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Question class
    /// </summary>
    [Admin]
    public class QuestionController : ApplicationController
    {
	    private readonly IRepository<Question> _questionRepository;
        private readonly IArchiveService _archiveService;

        public QuestionController(IRepository<Question> questionRepository, IArchiveService archiveService)
        {
            _questionRepository = questionRepository;
            _archiveService = archiveService;
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
        public ActionResult Details(int id, int? categoryId)
        {
            var question = _questionRepository.GetNullableById(id);

            if (question == null)
            {
                Message = "Question not found.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = QuestionDetailViewModel.Create(Repository, question);
            if (categoryId.HasValue && categoryId.Value != 0)
            {
                viewModel.Category = question.Category;
            }

            return View(viewModel);
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
        public ActionResult Create(int id, int? categoryId, Question question, ResponsesParameter[] response, string sortOrder)
        {
            var isNewVersion = false;
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
                if (Repository.OfType<Answer>().Queryable.Where(a => a.Category.Id == category.Id).Any())
                {
                    //The category picked already has answers, so if it is valid and we create a new question, we want to create a new version of the category first.
                    isNewVersion = true;
                }
            }
            viewModel.Question = question;

            var useSort = true;
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                var ids = sortOrder.Split(' ');
                var responseIds = new int[ids.Count()];
                for (var i = 0; i < ids.Count(); i++)
                {
                    if (int.TryParse(ids[i], out responseIds[i])) continue;
                    useSort = false;
                    break;
                }
                if (useSort && responseIds.Count() == response.Count())
                {
                    var sortedResponse = new List<ResponsesParameter>();
                    for (var i = 0; i < responseIds.Count(); i++)
                    {
                        if (!string.IsNullOrWhiteSpace(response[responseIds[i]].Value))
                        {
                            sortedResponse.Add(response[responseIds[i]]);
                        }
                    }
                    viewModel.Responses = sortedResponse;
                }
                else
                {
                    viewModel.Responses = response;
                }
            }
            else
            {
                viewModel.Responses = response;
            }

            // Remove responses that do not have a Choice or that have the remove checked. This is the create, so they will never be added
            var cleanedResponse = new List<ResponsesParameter>();
            foreach (var responsesParameter in viewModel.Responses)
            {
                if (!string.IsNullOrWhiteSpace(responsesParameter.Value) && !responsesParameter.Remove)
                {
                    cleanedResponse.Add(responsesParameter);
                }
            }
            viewModel.Responses = cleanedResponse;


            var questionToCreate = new Question(survey);
            Mapper.Map(question, questionToCreate);
            var counter = 0;
            foreach (var responsesParameter in viewModel.Responses)
            {
                counter++;
                var responseToAdd = new Response
                {
                    Order = counter,
                    IsActive = true,
                    Score = responsesParameter.Score,
                    Value = responsesParameter.Value
                };

                questionToCreate.AddResponse(responseToAdd);
            }

            ModelState.Clear();
            questionToCreate.TransferValidationMessagesTo(ModelState);

            if (questionToCreate.Responses.Where(a => a.IsActive).Count() == 0)
            {
                ModelState.AddModelError("Question", "Responses are required.");
            }
            if (questionToCreate.Category != null && !questionToCreate.Category.IsCurrentVersion)
            {
                ModelState.AddModelError("Question.Category", "Selected Category is not current.");
            }

            if (ModelState.IsValid)
            {
                var extraMessage = string.Empty;
                if (isNewVersion && questionToCreate.IsActive)
                {
                    questionToCreate.Category = _archiveService.ArchiveCategory(Repository, questionToCreate.Category.Id, questionToCreate.Category);
                    viewModel.Category = questionToCreate.Category;
                    extraMessage = ", related Category versioned";
                }

                _questionRepository.EnsurePersistent(questionToCreate);

                Message = string.Format("Question Created Successfully{0}", extraMessage);

                if (viewModel.Category != null)
                {
                    return this.RedirectToAction<CategoryController>(a => a.Edit(viewModel.Category.Id));
                }
                return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id));
            }
            return View(viewModel);
        }

        //
        // GET: /Question/Edit/5
        public ActionResult Edit(int id, int surveyId, int? categoryId)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(surveyId);
            if (survey == null)
            {
                Message = "Survey Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var question = _questionRepository.GetNullableById(id);
            if (question == null)
            {
                Message = "Question Not Found.";
                return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id));
            }

			var viewModel = QuestionViewModel.Create(Repository, survey);
			viewModel.Question = question;
            if (categoryId != null)
            {
                var category = Repository.OfType<Category>().GetNullableById(categoryId.Value);
                viewModel.Category = category;
            }

            foreach (var resp in
                question.Responses.OrderBy(a => a.Order).Select(response => new ResponsesParameter {Value = response.Value, Score = response.Score, ResponseId = response.Id,  Remove = !response.IsActive}))
            {
                viewModel.Responses.Add(resp);
            }

			return View(viewModel);
        }
        
        //
        // POST: /Question/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, int surveyId, int? categoryId, Question question, ResponsesParameter[] response, string sortOrder)
        {
            //To check if there are related answers, we need to check two things:
            //  1) Does this question have related answers
            //  2) Does the related Category have related answers (cauz we might be moving the question to a different category)
            //If there are related answers we may have to version one or both categories
            //And we need to check if we need to version
            //  1) Moving question to a different category
            //  2) Active changed
            //  3) Open Ended Changed
            //  4) Response added
            //  5) Response Hidden/Unhidden (Response.IsActive)
            //  6) quesion (name) is changed


            var survey = Repository.OfType<Survey>().GetNullableById(surveyId);
            if (survey == null)
            {
                Message = "Survey Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var questionToEdit = _questionRepository.GetNullableById(id);
            if (questionToEdit == null)
            {
                Message = "Question Not Found.";
                return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id));
            }
            
            var viewModel = QuestionViewModel.Create(Repository, survey);
            if (categoryId != null)
            {
                var category = Repository.OfType<Category>().GetNullableById(categoryId.Value);
                viewModel.Category = category;
            }
            viewModel.Question = question;

            var useSort = true;
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                var ids = sortOrder.Split(' ');
                var responseIds = new int[ids.Count()];
                for (var i = 0; i < ids.Count(); i++)
                {
                    if (int.TryParse(ids[i], out responseIds[i])) continue;
                    useSort = false;
                    break;
                }
                if (useSort && responseIds.Count() == response.Count())
                {
                    var sortedResponse = new List<ResponsesParameter>();
                    for (var i = 0; i < responseIds.Count(); i++)
                    {
                        if (!string.IsNullOrWhiteSpace(response[responseIds[i]].Value))
                        {
                            sortedResponse.Add(response[responseIds[i]]);
                        }
                    }
                    viewModel.Responses = sortedResponse;
                }
                else
                {
                    viewModel.Responses = response;
                }
            }
            else
            {
                viewModel.Responses = response;
            }

            // never removed saved responses, only make them inactive. 
            var cleanedResponse = new List<ResponsesParameter>();
            foreach (var responsesParameter in viewModel.Responses)
            {
                if (responsesParameter.ResponseId != 0)
                {
                    cleanedResponse.Add(responsesParameter);
                }
                else if (!string.IsNullOrWhiteSpace(responsesParameter.Value) && !responsesParameter.Remove)
                {
                    cleanedResponse.Add(responsesParameter);
                }
            }
            viewModel.Responses = cleanedResponse;

            Mapper.Map(question, questionToEdit);

            var counter = 0;
            foreach (var responsesParameter in viewModel.Responses)
            {
                counter++;
                if (responsesParameter.ResponseId != 0)
                {
                    var foundResp = questionToEdit.Responses.Where(a => a.Id == responsesParameter.ResponseId).Single();
                    foundResp.Value = responsesParameter.Value;
                    foundResp.Score = responsesParameter.Score;
                    foundResp.IsActive = !responsesParameter.Remove;
                    foundResp.Order = counter;
                }
                else
                {
                    var responseToAdd = new Response
                    {
                        Order = counter,
                        IsActive = true,
                        Score = responsesParameter.Score,
                        Value = responsesParameter.Value
                    };

                    questionToEdit.AddResponse(responseToAdd);
                }
            }

            ModelState.Clear();
            questionToEdit.TransferValidationMessagesTo(ModelState);

            foreach (var responseCheck in questionToEdit.Responses)
            {
                if (string.IsNullOrWhiteSpace(responseCheck.Value))
                {
                    ModelState.AddModelError("Question", string.Format("Response {0} must have a choice.", responseCheck.Order));
                }
            }
            if (questionToEdit.Responses.Where(a => a.IsActive).Count() == 0)
            {
                ModelState.AddModelError("Question", "Active Responses are required.");
            }

            if (questionToEdit.Category != null && !questionToEdit.Category.IsCurrentVersion)
            {
                ModelState.AddModelError("Question.Category", "Selected Category is not current.");
            }

            if (ModelState.IsValid)
            {
                _questionRepository.EnsurePersistent(questionToEdit);

                Message = "Question Edited Successfully";

                if (viewModel.Category != null)
                {
                    return this.RedirectToAction<CategoryController>(a => a.Edit(viewModel.Category.Id));
                }
                return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id));
            }
            else
            {
                return View(viewModel);
            }
        }

        /// <summary>
        /// ReOrder Get
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <returns></returns>
        public ActionResult ReOrder(int id)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction<SurveyController>(a => a.Index());
            }

            var viewModel = QuestionListViewModel.Create(Repository, survey);
            return View(viewModel);
        }

        /// <summary>
        /// ReOrder Questions within a survey
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="tableOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult ReOrder(int id, int[] tableOrder)
        {

                var survey = Repository.OfType<Survey>().GetNullableById(id);
                if (survey == null)
                {
                    return new JsonNetResult(false);
                }

                for (var i = 0; i < tableOrder.Count(); i++)
                {
                    survey.Questions.Where(a => a.Id == tableOrder[i]).Single().Order = i + 1;
                }
                ModelState.Clear();
                survey.TransferValidationMessagesTo(ModelState);
               
                if (ModelState.IsValid)
                {
                    Repository.OfType<Survey>().EnsurePersistent(survey);
                }

                return new JsonNetResult(true);


        }

        ////
        //// GET: /Question/Delete/5 
        //public ActionResult Delete(int id)
        //{
        //    var question = _questionRepository.GetNullableById(id);

        //    if (question == null) return RedirectToAction("Index");

        //    return View(question);
        //}

        ////
        //// POST: /Question/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, Question question)
        //{
        //    var questionToDelete = _questionRepository.GetNullableById(id);

        //    if (questionToDelete == null) return RedirectToAction("Index");

        //    _questionRepository.Remove(questionToDelete);

        //    Message = "Question Removed Successfully";

        //    return RedirectToAction("Index");
        //}
        

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
        public IList<ResponsesParameter> Responses { get; set; }
 
		public static QuestionViewModel Create(IRepository repository, Survey survey)
		{
			Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null);
			
			var viewModel = new QuestionViewModel {Question = new Question(), Survey = survey};
		    viewModel.Categories = viewModel.Survey.Categories.Where(a => a.IsCurrentVersion).OrderBy(a => a.Rank);
            viewModel.Responses = new List<ResponsesParameter>();
 
			return viewModel;
		}
	}

    public class QuestionDetailViewModel
    {
        public Question Question { get; set; }
        public Category Category { get; set; }

        public static QuestionDetailViewModel Create(IRepository repository, Question question)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(question != null);

            var viewModel = new QuestionDetailViewModel {Question = question};

            return viewModel;
        }
    }

    public class QuestionListViewModel
    {
        public Survey Survey { get; set; }
        public IEnumerable<Question> Questions { get; set; }

        public static QuestionListViewModel Create(IRepository repository, Survey survey)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null);

            var viewModel = new QuestionListViewModel { Survey = survey };
            viewModel.Questions = viewModel.Survey.Questions.Where(a => a.Category.IsCurrentVersion).OrderBy(a => a.Order);

            return viewModel;
        }
    }

    public class ResponsesParameter
    {
        public string Value { get; set; }
        public int Score { get; set; }
        public bool Remove { get; set; }
        public int ResponseId { get; set; }
        public int Order { get; set; }
    }
}
