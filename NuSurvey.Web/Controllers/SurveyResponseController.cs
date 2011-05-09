using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Helpers;

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

        
        /// <summary>
        /// Called from the Survey Details.
        /// GET: /SurveyResponse/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Admin]
        public ActionResult Details(int id)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);

            if (surveyResponse == null) return RedirectToAction("Index");

            var viewModel = SurveyReponseDetailViewModel.Create(Repository, surveyResponse);

            return View(viewModel);
        }

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
            //foreach (var category in survey.Categories.Where(a => !a.DoNotUseForCalculations))
            //{
            //    var catMaxScore =
            //        Repository.OfType<CategoryMaxScore>().Queryable.Where(a => a.Category == category).FirstOrDefault();
            //    if (catMaxScore == null)
            //    {
            //        Message = "Survey does not have related CategoryMaxScore records.";
            //        return this.RedirectToAction<ErrorController>(a => a.Index());
            //    }
            //}
			var viewModel = SurveyResponseViewModel.Create(Repository, survey);
            
            return View(viewModel);
        } 

        //
        // POST: /SurveyResponse/Create
        [HttpPost]
        public ActionResult Create(int id, SurveyResponse surveyResponse, QuestionAnswerParameter[] questions)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null || !survey.IsActive)
            {
                Message = "Survey not found or not active.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var surveyResponseToCreate = new SurveyResponse(survey);
            if (questions == null)
            {
                questions = new QuestionAnswerParameter[0];
            }

            TransferValues(surveyResponse, surveyResponseToCreate, questions);
            surveyResponseToCreate.UserId = CurrentUser.Identity.Name;

            if (survey.Questions.Where(a => a.IsActive && a.Category.IsActive).Count() != questions.Count())
            {
                Message = "You must answer all survey questions.";
            }
            ModelState.Clear();
            surveyResponseToCreate.TransferValidationMessagesTo(ModelState);

            for (int i = 0; i < questions.Count(); i++)
            {
                var i1 = i;
                if (!surveyResponseToCreate.Answers.Where(a => a.Question.Id == questions[i1].QuestionId).Any())
                {
                    if (survey.Questions.Where(a => a.Id == questions[i1].QuestionId).Single().IsOpenEnded)
                    {
                        if (string.IsNullOrWhiteSpace(questions[i1].Answer))
                        {
                            ModelState.AddModelError(string.Format("Questions[{0}]", i1), "Numeric answer to Question is required"); 
                        }
                        else
                        {
                            ModelState.AddModelError(string.Format("Questions[{0}]", i1), "Answer must be a number");  
                        }                 
                    }
                    else
                    {
                        ModelState.AddModelError(string.Format("Questions[{0}]", i1), "Answer is required");
                    }
                }
            }


            if (ModelState.IsValid)
            {
                var scores = new List<Scores>();
                foreach (var category in survey.Categories.Where(a => !a.DoNotUseForCalculations && a.IsActive && a.IsCurrentVersion))
                {
                    var score = new Scores();
                    score.Category = category;
                    score.MaxScore = Repository.OfType<CategoryTotalMaxScore>().GetNullableById(category.Id).TotalMaxScore; //Repository.OfType<CategoryMaxScore>().Queryable.Where(a => a.Category == category).First().MaxScore;
                    score.TotalScore = surveyResponseToCreate.Answers.Where(a => a.Category == category).Sum(b => b.Score);
                    score.Percent = (score.TotalScore / score.MaxScore) * 100m;
                    score.Rank = category.Rank;
                    scores.Add(score);
                }

                surveyResponseToCreate.PositiveCategory = scores
                    .OrderByDescending(a => a.Percent)
                    .ThenBy(a => a.Rank)
                    .FirstOrDefault().Category;

                surveyResponseToCreate.NegativeCategory1 = scores
                    .Where(a => a.Category != surveyResponseToCreate.PositiveCategory)
                    .OrderBy(a => a.Percent)
                    .ThenBy(a => a.Rank)
                    .FirstOrDefault().Category;
                surveyResponseToCreate.NegativeCategory2 = scores                    
                    .Where(a => a.Category != surveyResponseToCreate.PositiveCategory && a.Category != surveyResponseToCreate.NegativeCategory1)
                    .OrderBy(a => a.Percent)
                    .ThenBy(a => a.Rank)
                    .FirstOrDefault().Category;
                _surveyResponseRepository.EnsurePersistent(surveyResponseToCreate);

                Message = "SurveyResponse Created Successfully";

                return this.RedirectToAction<SurveyResponseController>(a => a.Results(surveyResponseToCreate.Id));
            }
            else
            {
                foreach (var modelState in ModelState.Values.Where(a => a.Errors.Count() > 0))
                {
                    var x = modelState;
                }
				var viewModel = SurveyResponseViewModel.Create(Repository, survey);
                viewModel.SurveyResponse = surveyResponse;
                viewModel.SurveyAnswers = questions;

                return View(viewModel);
            }
        }

        /// <summary>
        /// Get: /SurveyResponse/Results
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Results(int id)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (surveyResponse == null)
            {
                Message = "Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!CurrentUser.IsInRole(RoleNames.Admin))
            {
                if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                {
                    return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                }
            }

            return View(surveyResponse);
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
        private static void TransferValues(SurveyResponse source, SurveyResponse destination, QuestionAnswerParameter[] questions)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)

            destination.StudentId = source.StudentId;
            var queryableQuestions = questions.AsQueryable();
            foreach (var question in destination.Survey.Questions)
            {
                Question question1 = question;
                var passedQuestion = queryableQuestions.Where(a => a.QuestionId == question1.Id).SingleOrDefault();
                if (passedQuestion == null)
                {
                    continue;
                }
                var answer = new Answer();
                answer.Question = question;
                answer.Category = question.Category;                
                if (question.IsOpenEnded)
                {
                    #region Open Ended Logic

                    int number;
                    if (Int32.TryParse(passedQuestion.Answer, out number))
                    {
                        answer.OpenEndedAnswer = number;
                        answer.Response = question1.Responses.Where(a => a.Value == number.ToString()).FirstOrDefault();
                        if (answer.Response == null)
                        {
                            var highResponse = question1.Responses.Where(a => a.Value.Contains("+")).FirstOrDefault();
                            if (highResponse != null)
                            {
                                int highValue;
                                if (Int32.TryParse(highResponse.Value, out highValue))
                                {
                                    if (number >= highValue)
                                    {
                                        answer.Response = highResponse;
                                    }
                                }
                            }
                        }
                        if (answer.Response == null)
                        {
                            var lowResponse = question1.Responses.Where(a => a.Value.Contains("-")).FirstOrDefault();
                            if (lowResponse != null)
                            {
                                int lowValue;
                                if (Int32.TryParse(lowResponse.Value, out lowValue))
                                {
                                    if (number <= Math.Abs(lowValue))
                                    {
                                        answer.Response = lowResponse;
                                    }
                                }
                            }
                        }                        
                    }
                    else
                    {
                        continue;
                    }
                    #endregion Open Ended Logic
                }
                else
                {
                    answer.Response = question1.Responses.Where(a => a.Id == passedQuestion.ResponseId).FirstOrDefault();
                }
                if (answer.Category.DoNotUseForCalculations && answer.Response == null && answer.OpenEndedAnswer != null)
                {
                    answer.Score = 0;
                    destination.AddAnswers(answer);
                }
                if (answer.Response != null)
                {
                    if (answer.Question.Category.DoNotUseForCalculations)
                    {
                        answer.Score = 0;
                    }
                    else
                    {
                        answer.Score = answer.Response.Score;
                    }
                    destination.AddAnswers(answer);
                }
                
            }      
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
	    public QuestionAnswerParameter[] SurveyAnswers;
 
		public static SurveyResponseViewModel Create(IRepository repository, Survey survey)
		{
			Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null);
			
			var viewModel = new SurveyResponseViewModel {SurveyResponse = new SurveyResponse(survey), Survey = survey};
		    //viewModel.SurveyResponse.Survey = survey;
		    viewModel.Questions = viewModel.Survey.Questions
                .Where(a => a.IsActive && a.Category != null && a.Category.IsActive)
                .OrderBy(a => a.Order).ToList();            
			return viewModel;
		}
	}

    public class SurveyReponseDetailViewModel
    {
        public SurveyResponse SurveyResponse { get; set; }
        public IList<Scores> Scores { get; set; }

        public static SurveyReponseDetailViewModel Create(IRepository repository, SurveyResponse surveyResponse)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(surveyResponse != null);

            var viewModel = new SurveyReponseDetailViewModel {SurveyResponse = surveyResponse};

            //Get all the related categories that had answers.
            var relatedCategoryIds = viewModel.SurveyResponse.Answers.Select(x => x.Category.Id).ToList();
            viewModel.Scores = new List<Scores>();
            foreach (var category in viewModel.SurveyResponse.Survey.Categories.Where(a => !a.DoNotUseForCalculations && relatedCategoryIds.Contains(a.Id)))
            {
                var score = new Scores();
                score.Category = category;
                score.MaxScore = repository.OfType<CategoryTotalMaxScore>().GetNullableById(category.Id).TotalMaxScore;
                score.TotalScore = viewModel.SurveyResponse.Answers.Where(a => a.Category == category).Sum(b => b.Score);
                score.Percent = (score.TotalScore / score.MaxScore) * 100m;
                score.Rank = category.Rank;
                viewModel.Scores.Add(score);
            }


            return viewModel;
        }
    }

    public class QuestionAnswerParameter
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public int ResponseId { get; set; }
    }

    public class Scores
    {
        public Category Category { get; set; }
        public decimal MaxScore { get; set; }
        public decimal TotalScore { get; set; }
        public decimal Percent { get; set; }
        public int Rank { get; set; }
    }
}
