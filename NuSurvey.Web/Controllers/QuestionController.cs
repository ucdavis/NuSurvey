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

        /// <summary>
        /// #1
        /// GET: /Question/Details/5
        /// </summary>
        /// <param name="id">Question Id</param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// #2
        /// GET: /Question/Create
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// #3
        /// POST: /Question/Create
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="categoryId"></param>
        /// <param name="question"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int id, int? categoryId, Question question, ResponsesParameter[] response)
        {
            var isNewVersion = false;
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null)
            {
                Message = "Survey Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var viewModel = QuestionViewModel.Create(Repository, survey);
            if (categoryId != null) //This category Id is just used for defaults and navigation
            {
                var category = Repository.OfType<Category>().GetNullableById(categoryId.Value);
                viewModel.Category = category;
            }
            viewModel.Question = question;

            if (question.Category != null && Repository.OfType<Answer>().Queryable.Where(a => a.Category.Id == question.Category.Id).Any())
            {
                //The category picked already has answers, so if it is valid and we create a new question, we want to create a new version of the category first.
                isNewVersion = true;
            }


            // Remove responses that do not have a Choice or that have the remove checked. This is the create, so they will never be added
            var cleanedResponse = new List<ResponsesParameter>();
            foreach (var responsesParameter in response.OrderBy(a => a.Order))
            {
                if (!string.IsNullOrWhiteSpace(responsesParameter.Value) && !responsesParameter.Remove)
                {
                    cleanedResponse.Add(responsesParameter);
                }
            }
            viewModel.Responses = cleanedResponse;


            var questionToCreate = new Question(survey);
            Mapper.Map(question, questionToCreate);

            foreach (var responsesParameter in viewModel.Responses)
            {
                var responseToAdd = new Response
                {
                    Order = responsesParameter.Order,
                    IsActive = true,
                    Score = responsesParameter.Score.GetValueOrDefault(0),
                    Value = responsesParameter.Value
                };

                questionToCreate.AddResponse(responseToAdd);
            }

            ModelState.Clear();
            questionToCreate.TransferValidationMessagesTo(ModelState);

            foreach (var responsesParameter in cleanedResponse)
            {
                if (!responsesParameter.Score.HasValue)
                {
                    ModelState.AddModelError("Question", "All responses need a score");
                }
                else
                {
                    if(questionToCreate.IsOpenEnded)
                    {
                        switch ((QuestionType)questionToCreate.OpenEndedQuestionType)
                        {
                            case QuestionType.WholeNumber:
                                int number;
                                if (!int.TryParse(responsesParameter.Value, out number))
                                {
                                    ModelState.AddModelError("Question", "Choices must be whole numbers");
                                }
                                break;
                            case QuestionType.Decimal:
                                float floatNumber;
                                if (!float.TryParse(responsesParameter.Value, out floatNumber))
                                {
                                    ModelState.AddModelError("Question", "Choices must be numbers (decimal ok)");
                                }
                                break;
                            default:
                                ModelState.AddModelError("Question", "time and time range not supported yet");
                                break;
                        }
                    }
                }
            }

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

        /// <summary>
        /// #4
        /// GET: /Question/Edit/5
        /// </summary>
        /// <param name="id">Question Id</param>
        /// <param name="surveyId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
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

            if (question.Survey.Id != survey.Id)
            {
                Message = "Question not related to current survey";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!question.Category.IsCurrentVersion)
            {
                Message = "Question's related category is not current version";
                return this.RedirectToAction<ErrorController>(a => a.Index());
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
        

        /// <summary>
        /// #5
        /// POST: /Question/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="surveyId"></param>
        /// <param name="categoryId"></param>
        /// <param name="question"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, int surveyId, int? categoryId, Question question, ResponsesParameter[] response)
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

            var originalCategoryId = 0;
            var newCategoryId = 0;
            var originalCategoryHasAnswers = false;
            var newCategoryHasAnswers = false;
            var originalHasChanges = false;
            var newHasChanges = false;

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


            //Save the Id of the original Category in case we need to version it.
            originalCategoryId = questionToEdit.Category.Id;
            originalCategoryHasAnswers = Repository.OfType<Answer>().Queryable.Where(a => a.Category.Id == originalCategoryId).Any();
            if (question.Category != null && question.Category.Id != originalCategoryId)
            {
                newCategoryId = question.Category.Id;
                newCategoryHasAnswers = Repository.OfType<Answer>().Queryable.Where(a => a.Category.Id == newCategoryId).Any();
            }
            
            var viewModel = QuestionViewModel.Create(Repository, survey);
            if (categoryId != null)
            {
                var category = Repository.OfType<Category>().GetNullableById(categoryId.Value);
                viewModel.Category = category;
            }
            viewModel.Question = question;



            // never removed saved responses, only make them inactive. 
            var cleanedResponse = new List<ResponsesParameter>();
            foreach (var responsesParameter in response.OrderBy(a => a.Order))
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


            //Version Checks Part1
            if (originalCategoryHasAnswers) //original category and has answers
            {
                if (newCategoryId != 0) //Changed to a different Category
                {
                    //if we are changing the category, but the question was not active then there would not be any changes to the original category
                    if (questionToEdit.IsActive) 
                    { 
                        originalHasChanges = true;
                    }
                } else if (questionToEdit.IsActive != question.IsActive) //Active state changed
                {
                    originalHasChanges = true;
                } else if (questionToEdit.IsOpenEnded != question.IsOpenEnded) //OpenEnded Question Changed
                {
                    originalHasChanges = true;
                } else if (questionToEdit.OpenEndedQuestionType != question.OpenEndedQuestionType)  //type of open ended question changed
                {
                    originalHasChanges = true;
                } else if (questionToEdit.Name.ToLower() != question.Name.ToLower()) //Question (name) has changed
                {
                    originalHasChanges = true;
                } else if (viewModel.Responses.Count != questionToEdit.Responses.Count) //Number of possible responses has changed
                {
                    //Added
                    originalHasChanges = true;
                } 
                else
                {
                    foreach (var responsesParameter in viewModel.Responses)
                    {
                        var foundResp = questionToEdit.Responses.Where(a => a.Id == responsesParameter.ResponseId).Single();
                        if (foundResp.Value.ToLower() != responsesParameter.Value.ToLower()) //Response Value (choice) has changed
                        {
                            originalHasChanges = true;
                            break;
                        } else if (foundResp.Score != responsesParameter.Score.GetValueOrDefault(0)) //Score has Changed
                        {
                            originalHasChanges = true;
                            break;
                        } else if (foundResp.IsActive == responsesParameter.Remove) //Hide response has changed
                        {
                            originalHasChanges = true;
                            break;
                        }
                    }
                }
            }
            if (newCategoryId != 0 && newCategoryHasAnswers)
            {
                var newCategory = Repository.OfType<Category>().GetNullableById(newCategoryId);
                if (newCategory != null)
                {
                    if (newCategory.IsActive && question.IsActive) //If the new Category isn't active it shouldn't have answers
                    {
                        newHasChanges = true;
                    }
                }
            }



            //Mapper.Map(question, questionToEdit);
            question.Responses.Clear();

            foreach (var responsesParameter in viewModel.Responses)
            {
                if (responsesParameter.ResponseId != 0)
                {
                    var foundResp = questionToEdit.Responses.Where(a => a.Id == responsesParameter.ResponseId).Single();
                    foundResp.Value = responsesParameter.Value;
                    foundResp.Score = responsesParameter.Score.GetValueOrDefault(0);
                    foundResp.IsActive = !responsesParameter.Remove;
                    foundResp.Order = responsesParameter.Order;
                    question.AddResponse(foundResp);
                }
                else
                {
                    var responseToAdd = new Response
                    {
                        Order = responsesParameter.Order,
                        IsActive = true,
                        Score = responsesParameter.Score.GetValueOrDefault(0),
                        Value = responsesParameter.Value
                    };

                    question.AddResponse(responseToAdd);
                }
            }
            question.Survey = questionToEdit.Survey;
            


            ModelState.Clear();
            question.TransferValidationMessagesTo(ModelState);

            foreach (var responsesParameter in cleanedResponse)
            {
                if (!responsesParameter.Score.HasValue)
                {
                    ModelState.AddModelError("Question", "All responses need a score");
                }
                if (question.IsOpenEnded && !responsesParameter.Remove)
                {
                    switch ((QuestionType)question.OpenEndedQuestionType)
                    {
                        case QuestionType.WholeNumber:
                            int number;
                            if (!int.TryParse(responsesParameter.Value, out number))
                            {
                                ModelState.AddModelError("Question", "Choices must be whole numbers");
                            }
                            break;
                        case QuestionType.Decimal:
                            float floatNumber;
                            if (!float.TryParse(responsesParameter.Value, out floatNumber))
                            {
                                ModelState.AddModelError("Question", "Choices must be numbers (decimal ok)");
                            }
                            break;
                        default:
                            ModelState.AddModelError("Question", "time and time range not supported yet");
                            break;
                    }
                }
            }

            foreach (var responseCheck in question.Responses)
            {
                if (string.IsNullOrWhiteSpace(responseCheck.Value))
                {
                    ModelState.AddModelError("Question", string.Format("Response {0} must have a choice.", responseCheck.Order + 1));
                }
            }
            if (question.Responses.Where(a => a.IsActive).Count() == 0)
            {
                ModelState.AddModelError("Question", "Active Responses are required.");
            }

            if (question.Category != null && !question.Category.IsCurrentVersion)
            {
                ModelState.AddModelError("Question.Category", "Selected Category is not current.");
            }

           

            if (ModelState.IsValid)
            {
                string extraMessage1;
                string extraMessage2;

                #region Original Has Changes and needs to be versioned
                if (originalHasChanges)
                {
                    var newOriginalCategory = _archiveService.ArchiveCategory(
                        Repository,
                        originalCategoryId, questionToEdit);
                    if (newCategoryId == 0)
                    {
                        var newQuestion = new Question(question.Survey);
                        newQuestion.Category = newOriginalCategory;
                        newQuestion.IsActive = question.IsActive;
                        newQuestion.IsOpenEnded = question.IsOpenEnded;
                        newQuestion.OpenEndedQuestionType = question.OpenEndedQuestionType;
                        newQuestion.Name = question.Name;
                        newQuestion.Order = questionToEdit.Order;
                        foreach (var responsesParameter in viewModel.Responses)
                        {
                            var responseToAdd = new Response
                            {
                                Order = responsesParameter.Order,
                                IsActive = !responsesParameter.Remove,
                                Score = responsesParameter.Score.GetValueOrDefault(0),
                                Value = responsesParameter.Value
                            };

                            newQuestion.AddResponse(responseToAdd);
                        }

                        question = newQuestion;
                        extraMessage1 = "Related Category Versioned";
                        extraMessage2 = string.Empty;

                        _questionRepository.EnsurePersistent(question);

                        Message = string.Format("Question Edited Successfully {0} {1}", extraMessage1, extraMessage2);

                        if (viewModel.Category != null)
                        {
                            return this.RedirectToAction<CategoryController>(a => a.Edit(newOriginalCategory.Id));
                        }
                        return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id));   
                    }
                    else
                    {
                        if (newHasChanges)
                        {
                            var newNewCategory = _archiveService.ArchiveCategory(Repository, newCategoryId, questionToEdit);

                            var newQuestion = new Question(question.Survey);
                            newQuestion.Category = newNewCategory;
                            newQuestion.IsActive = question.IsActive;
                            newQuestion.IsOpenEnded = question.IsOpenEnded;
                            newQuestion.OpenEndedQuestionType = question.OpenEndedQuestionType;
                            newQuestion.Name = question.Name;
                            newQuestion.Order = questionToEdit.Order;
                            foreach (var responsesParameter in viewModel.Responses)
                            {
                                var responseToAdd = new Response
                                {
                                    Order = responsesParameter.Order,
                                    IsActive = !responsesParameter.Remove,
                                    Score = responsesParameter.Score.GetValueOrDefault(0),
                                    Value = responsesParameter.Value
                                };

                                newQuestion.AddResponse(responseToAdd);
                            }

                            question = newQuestion;
                            extraMessage1 = "Previously Related Category Versioned";
                            extraMessage2 = "Newly Related Category Versioned";

                            _questionRepository.EnsurePersistent(question);

                            Message = string.Format("Question Edited Successfully {0} {1}", extraMessage1, extraMessage2);

                            if (viewModel.Category != null)
                            {
                                return this.RedirectToAction<CategoryController>(a => a.Edit(newOriginalCategory.Id));
                            }
                            return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id)); 
                        }
                        else
                        {
                            var newQuestion = new Question(question.Survey);
                            newQuestion.Category = question.Category;
                            newQuestion.IsActive = question.IsActive;
                            newQuestion.IsOpenEnded = question.IsOpenEnded;
                            newQuestion.OpenEndedQuestionType = question.OpenEndedQuestionType;
                            newQuestion.Name = question.Name;
                            newQuestion.Order = questionToEdit.Order;
                            foreach (var responsesParameter in viewModel.Responses)
                            {
                                var responseToAdd = new Response
                                {
                                    Order = responsesParameter.Order,
                                    IsActive = !responsesParameter.Remove,
                                    Score = responsesParameter.Score.GetValueOrDefault(0),
                                    Value = responsesParameter.Value
                                };

                                newQuestion.AddResponse(responseToAdd);
                            }

                            question = newQuestion;
                            extraMessage1 = "Previously Related Category Versioned";
                            extraMessage2 = "Newly Related Category Not Versioned";

                            _questionRepository.EnsurePersistent(question);

                            Message = string.Format("Question Edited Successfully {0} {1}", extraMessage1, extraMessage2);

                            if (viewModel.Category != null)
                            {
                                return this.RedirectToAction<CategoryController>(a => a.Edit(newOriginalCategory.Id));
                            }
                            return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id)); 
                        }

                        
                    }


                }
                #endregion Original Has Changes and needs to be versioned

                #region Original Is not Versioned, but new Category Is
                
                
                if (newHasChanges)
                {
                    var newNewCategory = _archiveService.ArchiveCategory(Repository, newCategoryId, questionToEdit);

                    var newQuestion = new Question(question.Survey);
                    newQuestion.Category = newNewCategory;
                    newQuestion.IsActive = question.IsActive;
                    newQuestion.IsOpenEnded = question.IsOpenEnded;
                    newQuestion.OpenEndedQuestionType = question.OpenEndedQuestionType;
                    newQuestion.Name = question.Name;
                    newQuestion.Order = questionToEdit.Order;
                    foreach (var responsesParameter in viewModel.Responses)
                    {
                        var responseToAdd = new Response
                        {
                            Order = responsesParameter.Order,
                            IsActive = !responsesParameter.Remove,
                            Score = responsesParameter.Score.GetValueOrDefault(0),
                            Value = responsesParameter.Value
                        };

                        newQuestion.AddResponse(responseToAdd);
                    }

                    question = newQuestion;

                    extraMessage1 = "Previously Related Category Not Versioned";
                    extraMessage2 = "Newly Related category Versioned";

                    _questionRepository.EnsurePersistent(question);

                    Message = string.Format("Question Edited Successfully {0} {1}", extraMessage1, extraMessage2);

                    if (viewModel.Category != null)
                    {
                        return this.RedirectToAction<CategoryController>(a => a.Edit(viewModel.Category.Id));
                    }
                    return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id)); 

                    
                }
                #endregion Original Is not Versioned, but new Category Is

                #region No Versioning, editing as normal                                

                Mapper.Map(question, questionToEdit);

                questionToEdit.Responses.Clear();

                foreach (var responsesParameter in viewModel.Responses)
                {
                    if (responsesParameter.ResponseId != 0)
                    {
                        var foundResp = question.Responses.Where(a => a.Id == responsesParameter.ResponseId).Single();
                        foundResp.Value = responsesParameter.Value;
                        foundResp.Score = responsesParameter.Score.GetValueOrDefault(0);
                        foundResp.IsActive = !responsesParameter.Remove;
                        foundResp.Order = responsesParameter.Order;
                        questionToEdit.AddResponse(foundResp);
                    }
                    else
                    {
                        var responseToAdd = new Response
                        {
                            Order = responsesParameter.Order,
                            IsActive = true,
                            Score = responsesParameter.Score.GetValueOrDefault(0),
                            Value = responsesParameter.Value
                        };

                        questionToEdit.AddResponse(responseToAdd);
                    }
                }

                _questionRepository.EnsurePersistent(questionToEdit);

                Message = "Question Edited Successfully";

                if (viewModel.Category != null)
                {
                    return this.RedirectToAction<CategoryController>(a => a.Edit(questionToEdit.Category.Id));
                }
                return this.RedirectToAction<SurveyController>(a => a.Edit(survey.Id));

                #endregion No Versioning, editing as normal

            }
            else
            {
                return View(viewModel);
            }
        }

        /// <summary>
        /// #6
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
        /// #7
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
        public int? Score { get; set; }
        public bool Remove { get; set; }
        public int ResponseId { get; set; }
        public int Order { get; set; }
    }
}
