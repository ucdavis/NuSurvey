using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MvcContrib;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Resources;
using NuSurvey.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Helpers;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the SurveyResponse class
    /// </summary>   
    public class SurveyResponseController : ApplicationController
    {
	    private readonly IRepository<SurveyResponse> _surveyResponseRepository;
        private readonly IScoreService _scoreService;
        private readonly IRepository<Photo> _photoRepository;
        private readonly IBlobStoargeService _blobStoargeService;

        public SurveyResponseController(IRepository<SurveyResponse> surveyResponseRepository, IScoreService scoreService, IRepository<Photo> photoRepository, IBlobStoargeService blobStoargeService)
        {
            _surveyResponseRepository = surveyResponseRepository;
            _scoreService = scoreService;
            _photoRepository = photoRepository;
            _blobStoargeService = blobStoargeService;
        }

        /// <summary>
        /// #1
        /// GET: /SurveyResponse/
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {            
            var isPublic = !(CurrentUser.IsInRole(RoleNames.User) || CurrentUser.IsInRole(RoleNames.Admin));
            if (isPublic)
            {
                return this.RedirectToAction<HomeController>(a => a.Index(false));
            }
            var viewModel = ActiveSurveyViewModel.Create(Repository, isPublic);

            return View(viewModel);
        }

        public ActionResult PublicSurveys()
        {
            var viewModel = ActiveSurveyViewModel.Create(Repository, true);
            

            return View(viewModel);
        }


        /// <summary>
        /// #2
        /// Called from the Survey Details.
        /// GET: /SurveyResponse/Details/5
        /// </summary>
        /// <param name="id">SurveyResponse Id</param>
        /// <param name="fromYourDetails">if set to <c>true</c> [from your details].</param>
        /// <returns></returns>
        [User]
        [Authorize]
        public ActionResult Details(int id, bool fromYourDetails = false)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);

            if (surveyResponse == null)
            {
                Message = "Survey Response Details Not Found.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
                //return RedirectToAction("Index");
            }
            if (!CurrentUser.IsInRole(RoleNames.Admin) && surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
            {
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }

            var viewModel = SurveyReponseDetailViewModel.Create(Repository, surveyResponse);
            viewModel.FromYourDetails = fromYourDetails;

            return View(viewModel);
        }

        public ActionResult FindAndStartSurvey(string shortName)
        {
            var survey = Repository.OfType<Survey>().Queryable.Single(a => a.ShortName == shortName);
            return this.RedirectToAction(a => a.StartSurvey(survey.Id));
        }

        /// <summary>
        /// #3
        /// Start or continue a survey with one question at a time
        /// GET: /SurveyResponse/StartSurvey/5
        /// </summary>
        /// <param name="id">Survey ID</param>
        /// <returns></returns>
        public ActionResult StartSurvey(int id)
        {
            var userId = string.Empty;
            var guid = Guid.NewGuid();
            if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                userId = CurrentUser.Identity.Name.ToLower();
            }
            else
            {
                userId = guid.ToString();
            }

            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null || !survey.IsActive)
            {
                Message = "Survey not found or not active.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            #region Check To See if there are enough available Categories
            if (GetCountActiveCategoriesWithScore(survey) < 3)
            {
                Message = "Survey does not have enough active categories to complete survey.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            #endregion Check To See if there are enough available Categories

            var cannotContinue = false;

            var pendingExists = _surveyResponseRepository.Queryable
                .Where(a => a.Survey.Id == id && a.IsPending && a.UserId == userId).FirstOrDefault();
            if (pendingExists != null)
            {
                foreach (var answer in pendingExists.Answers)
                {
                    if (!answer.Category.IsCurrentVersion)
                    {
                        Message =
                            "The unfinished survey's questions have been modified. Unable to continue. Delete survey and start again.";
                        cannotContinue = true;
                        break;
                    }
                }
                if (!cannotContinue)
                {
                    Message = "Unfinished survey found.";
                }
            }

            ViewBag.surveyimage = string.Format("{0}-survey", survey.ShortName.ToLower().Trim());

            var viewModel = SingleAnswerSurveyResponseViewModel.Create(Repository, survey, pendingExists);
            viewModel.CannotContinue = cannotContinue;
            if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                viewModel.PublicGuid = null;
            }
            else
            {
                viewModel.PublicGuid = guid;
            }

            return View(viewModel);

        }

        private int GetCountActiveCategoriesWithScore(Survey survey)
        {
            var count = 0;
            foreach (var category in survey.Categories.Where(a => !a.DoNotUseForCalculations && a.IsActive && a.IsCurrentVersion))
            {
                var totalMax = Repository.OfType<CategoryTotalMaxScore>().GetNullableById(category.Id);
                if (totalMax == null) //No Questions most likely
                {
                    continue;
                }
                count++;
                if (count > 3)
                {
                    break;
                }
            }
            return count;
        }

        /// <summary>
        /// #4
        /// Start or continue a survey with one question at a time
        /// POST: 
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="surveyResponse"></param>
        /// <param name="publicGuid"> </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StartSurvey(int id, SurveyResponse surveyResponse, Guid? publicGuid)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null || !survey.IsActive)
            {
                Message = "Survey not found or not active.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            ViewBag.surveyimage = string.Format("{0}-survey", survey.ShortName.ToLower().Trim());

            surveyResponse.IsPending = true;
            surveyResponse.Survey = survey;
            surveyResponse.UserId = !string.IsNullOrWhiteSpace(CurrentUser.Identity.Name) ? CurrentUser.Identity.Name.ToLower() : publicGuid.ToString();

            ModelState.Clear();
            surveyResponse.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
                {
                    _surveyResponseRepository.EnsurePersistent(surveyResponse);
                }
                else
                {
                    Session[publicGuid.ToString()] = surveyResponse;
                }

                
                return this.RedirectToAction(a => a.AnswerNext(surveyResponse.Id, publicGuid));
            }

            Message = "Please correct errors to continue";

            var viewModel = SingleAnswerSurveyResponseViewModel.Create(Repository, survey, null);
            viewModel.SurveyResponse = surveyResponse;
            viewModel.PublicGuid = publicGuid;

            return View(viewModel);
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

        /// <summary>
        /// #5
        /// Load the next available question, or finalize if all questions are answered.
        /// GET: /SurveyResponse/AnswerNext/5
        /// </summary>
        /// <param name="id">SurveyResponse Id</param>
        /// <returns></returns>
        public ActionResult AnswerNext(int id, Guid? publicGuid)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                surveyResponse = (SurveyResponse)Session[publicGuid.ToString()];
            }

            if (surveyResponse == null || !surveyResponse.IsPending)
            {
                Message = "Pending survey not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                {
                    Message = "Not your survey";
                    return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                }
            }
            else
            {
                if (surveyResponse.UserId.ToLower() != publicGuid.ToString().ToLower())
                {
                    Message = "Not your survey";
                    return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                }
            }
            var viewModel = SingleAnswerSurveyResponseViewModel.Create(Repository, surveyResponse.Survey, surveyResponse);
            viewModel.PublicGuid = publicGuid;
            
            if (viewModel.CurrentQuestion == null)
            {
                return this.RedirectToAction(a => a.FinalizePending(surveyResponse.Id, publicGuid));
            }

            ViewBag.surveyimage = string.Format("{0}-survey", surveyResponse.Survey.ShortName.ToLower().Trim());
            return View(viewModel);
        }

        /// <summary>
        /// #6
        /// POST:
        /// </summary>
        /// <param name="id">SurveyResponse Id</param>
        /// <param name="questions"></param>
        /// <param name="byPassAnswer"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AnswerNext(int id, QuestionAnswerParameter questions, string byPassAnswer, Guid? publicGuid)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                surveyResponse = (SurveyResponse)Session[publicGuid.ToString()];
            }
            if (surveyResponse == null || !surveyResponse.IsPending)
            {
                Message = "Pending survey not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                {
                    Message = "Not your survey";
                    return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                }
            }
            else
            {
                if (surveyResponse.UserId.ToLower() != publicGuid.ToString().ToLower())
                {
                    Message = "Not your survey";
                    return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                }
            }

            ViewBag.surveyimage = string.Format("{0}-survey", surveyResponse.Survey.ShortName.ToLower().Trim());

            var question = Repository.OfType<Question>().GetNullableById(questions.QuestionId);
            if (question == null)
            {
                Message = "Question survey not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }                           
            Answer answer;
            if (surveyResponse.Answers.Where(a => a.Question.Id == question.Id).Any())
            {
                answer = surveyResponse.Answers.Where(a => a.Question.Id == question.Id).First();
            }
            else
            {
                answer = new Answer(); 
            }

            if (!string.IsNullOrEmpty(byPassAnswer) && question.AllowBypass)
            {
                answer.OpenEndedAnswer = null;
                answer.Response = null;
                answer.Score = 0;
                answer.BypassScore = true;
                answer.OpenEndedStringAnswer = byPassAnswer;
                answer.Question = question;
                answer.Category = question.Category;
                surveyResponse.AddAnswers(answer);
                if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
                {
                    _surveyResponseRepository.EnsurePersistent(surveyResponse);
                }
                else
                {
                    Session[publicGuid.ToString()] = surveyResponse;
                }

                return this.RedirectToAction(a => a.AnswerNext(surveyResponse.Id, publicGuid));
            }
            else
            {
                questions = _scoreService.ScoreQuestion(surveyResponse.Survey.Questions.AsQueryable(), questions);
            }
            if (questions.Invalid)
            {
                ModelState.AddModelError("Questions", questions.Message);
            }
            else
            {
                //It is valid, add the answer.
                answer.Question = question;
                answer.Category = question.Category;
                answer.OpenEndedAnswer = questions.OpenEndedNumericAnswer;
                if (question.IsOpenEnded && question.OpenEndedQuestionType == (int)QuestionType.TimeRange)
                {
                    answer.OpenEndedStringAnswer = string.Format("{0}_{1}", questions.Answer, questions.AnswerRange);
                }
                else
                {
                    answer.OpenEndedStringAnswer = questions.Answer; // The actual answer they gave. 
                }                
                answer.Response = Repository.OfType<Response>().GetNullableById(questions.ResponseId);
                answer.Score = questions.Score;

                surveyResponse.AddAnswers(answer);
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
                {
                    _surveyResponseRepository.EnsurePersistent(surveyResponse);
                }
                else
                {
                    Session[publicGuid.ToString()] = surveyResponse;
                }
                return this.RedirectToAction(a => a.AnswerNext(surveyResponse.Id, publicGuid));
            }

            var viewModel = SingleAnswerSurveyResponseViewModel.Create(Repository, surveyResponse.Survey, surveyResponse);
            viewModel.PublicGuid = publicGuid;
            if(question.AllowBypass)
            {
                viewModel.DisplayBypass = true;
            }
            return View(viewModel);
            

        }

        /// <summary>
        /// #7
        /// Calculate the positive and two negative categories and set the pending flag to false
        /// </summary>
        /// <param name="id">SurveyResponse Id</param>
        /// <param name="publicGuid"> </param>
        /// <returns></returns>
        public ActionResult FinalizePending(int id, Guid? publicGuid)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                surveyResponse = (SurveyResponse)Session[publicGuid.ToString()];
            }
            if (surveyResponse == null || !surveyResponse.IsPending)
            {
                Message = "Pending survey not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                {
                    Message = "Not your survey";
                    return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                }
            }
            else
            {
                if (surveyResponse.UserId.ToLower() != publicGuid.ToString().ToLower())
                {
                    Message = "Not your survey";
                    return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                }
            }
            var viewModel = SingleAnswerSurveyResponseViewModel.Create(Repository, surveyResponse.Survey, surveyResponse);
            viewModel.PublicGuid = publicGuid;
            if (viewModel.CurrentQuestion == null)
            {
                _scoreService.CalculateScores(Repository, surveyResponse);
                surveyResponse.IsPending = false;
                if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
                {
                    _surveyResponseRepository.EnsurePersistent(surveyResponse);
                }
                else
                {
                    Session[publicGuid.ToString()] = surveyResponse;
                }
                return this.RedirectToAction(a => a.Results(surveyResponse.Id, publicGuid));
            }
            else
            {
                Message = "Error finalizing survey.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
        }


        /// <summary>
        /// #8
        /// GET:
        /// </summary>
        /// <param name="id">SurveyResponse Id</param>
        /// <param name="fromAdmin"></param>
        /// <returns></returns>
        public ActionResult DeletePending(int id, bool fromAdmin)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (surveyResponse == null || !surveyResponse.IsPending)
            {
                Message = "Pending survey not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!CurrentUser.IsInRole(RoleNames.Admin) && surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
            {
                Message = "Not your survey";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            var viewModel = SingleAnswerSurveyResponseViewModel.Create(Repository, surveyResponse.Survey, surveyResponse);
            viewModel.FromAdmin = fromAdmin;

            return View(viewModel);
        }

        /// <summary>
        /// #9
        /// POST:
        /// </summary>
        /// <param name="id">SurveyResponse Id</param>
        /// <param name="confirm"></param>
        /// <param name="fromAdmin"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePending(int id, bool confirm, bool fromAdmin)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (surveyResponse == null || !surveyResponse.IsPending)
            {
                Message = "Pending survey not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!CurrentUser.IsInRole(RoleNames.Admin) && surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
            {
                Message = "Not your survey";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }

            var surveyId = surveyResponse.Survey.Id;

            if (confirm == false)
            {
                if (fromAdmin)
                {
                    return this.RedirectToAction<SurveyController>(a => a.PendingDetails(surveyId));
                }
                return this.RedirectToAction(a => a.StartSurvey(surveyId));
            }

            _surveyResponseRepository.Remove(surveyResponse);

            if (fromAdmin)
            {
                return this.RedirectToAction<SurveyController>(a => a.PendingDetails(surveyId));
            }
            return this.RedirectToAction<HomeController>(a => a.Index(false));

        }

        /// <summary>
        /// #10
        /// GET: /SurveyResponse/Create
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <returns></returns>
        [User]
        [Authorize]
        public ActionResult Create(int id)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null || !survey.IsActive)
            {
                Message = "Survey not found or not active.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            #region Check To See if there are enough available Categories
            if (GetCountActiveCategoriesWithScore(survey) < 3)
            {
                Message = "Survey does not have enough active categories to complete survey.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            #endregion Check To See if there are enough available Categories

			var viewModel = SurveyResponseViewModel.Create(Repository, survey);
            
            return View(viewModel);
        } 


        /// <summary>
        /// #11
        /// POST: /SurveyResponse/Create
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="surveyResponse"></param>
        /// <param name="questions"></param>
        /// <returns></returns>
        [HttpPost]
        [User]
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

            ModelState.Clear();            
            //Set non-dynamic surveyResponse values
            surveyResponseToCreate.StudentId = surveyResponse.StudentId;
            surveyResponseToCreate.UserId = CurrentUser.Identity.Name.ToLower();

            //Check each question, create an answer for it if there isn't one.
            var length = questions.Length;
            for (int i = 0; i < length; i++)
            {
                var question = Repository.OfType<Question>().GetNullableById(questions[i].QuestionId);
                Check.Require(question != null, string.Format("Question not found.\n SurveyId: {0}\n QuestionId: {1}\n Question #: {2}", id, questions[i].QuestionId, i));
                Check.Require(question.Category.IsActive, string.Format("Related Category is not active for question Id {0}", questions[i].QuestionId));
                Check.Require(question.Category.IsCurrentVersion, string.Format("Related Category is not current version for question Id {0}", questions[i].QuestionId));
                Check.Require(question.Survey.Id == survey.Id, string.Format("Related Survey does not match passed survey {0}--{1}", question.Survey.Id, survey.Id));

                Answer answer;
                if (surveyResponseToCreate.Answers.Where(a => a.Question.Id == question.Id).Any())
                {
                    answer = surveyResponseToCreate.Answers.Where(a => a.Question.Id == question.Id).First();
                }
                else
                {
                    answer = new Answer();
                }

                //Score question and specify any errors
                questions[i] = _scoreService.ScoreQuestion(surveyResponseToCreate.Survey.Questions.AsQueryable(), questions[i]);

                if (questions[i].Invalid && !questions[i].BypassQuestion)
                {
                    ModelState.AddModelError(string.Format("Questions[{0}]", i), questions[i].Message);
                }

                if (questions[i].BypassQuestion)
                {
                    answer.OpenEndedAnswer = null;
                    answer.Response = null;
                    answer.Score = 0;
                    answer.BypassScore = true;
                    questions[i].Answer = string.Empty;
                    questions[i].ResponseId = 0;
                }
                else
                {
                    answer.OpenEndedAnswer = questions[i].OpenEndedNumericAnswer;
                    if (question.IsOpenEnded && question.OpenEndedQuestionType == (int)QuestionType.TimeRange)
                    {
                        answer.OpenEndedStringAnswer = string.Format("{0}_{1}", questions[i].Answer, questions[i].AnswerRange);
                    }
                    else
                    {
                        answer.OpenEndedStringAnswer = questions[i].Answer; // The actual answer they gave. 
                    }                       
                    answer.Response = Repository.OfType<Response>().GetNullableById(questions[i].ResponseId);
                    answer.Score = questions[i].Score;
                    answer.BypassScore = false;
                }

                answer.Question = question;
                answer.Category = question.Category;
                

                surveyResponseToCreate.AddAnswers(answer);
            }


            surveyResponseToCreate.TransferValidationMessagesTo(ModelState);

            if (survey.Questions.Where(a => a.IsActive && a.Category.IsActive && a.Category.IsCurrentVersion).Count() != questions.Count())
            {
                Message = "You must answer all survey questions.";
                ModelState.AddModelError("Question", "You must answer all survey questions.");
            }


            if (ModelState.IsValid)
            {
                _scoreService.CalculateScores(Repository, surveyResponseToCreate);

                _surveyResponseRepository.EnsurePersistent(surveyResponseToCreate);

                Message = "Below are the customized goals for the survey you entered. You can use the \"Print Results\" link below to print this individual goal sheet. If you would like to print a text-only version or goal sheets for multiple participants at one time go to the Educators Dashboard and select the Review & Print section for the survey you are working with.";

                return this.RedirectToAction(a => a.Results(surveyResponseToCreate.Id, null));
            }
            else
            {
                //foreach (var modelState in ModelState.Values.Where(a => a.Errors.Count() > 0))
                //{
                //    var x = modelState;
                //}
                if (string.IsNullOrWhiteSpace(Message))
                {
                    Message = "Please correct all errors and submit.";
                }
                var viewModel = SurveyResponseViewModel.Create(Repository, survey);
                viewModel.SurveyResponse = surveyResponse;
                viewModel.SurveyAnswers = questions;

                return View(viewModel);
            }
        }

        /// <summary>
        /// #12
        /// Get: /SurveyResponse/Results
        /// </summary>
        /// <param name="id">SurveyResponse ID</param>
        /// <param name="publicGuid"> </param>
        /// <returns></returns>
        public ActionResult Results(int id, Guid? publicGuid)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                surveyResponse = (SurveyResponse)Session[publicGuid.ToString()];
            }
            if (surveyResponse == null)
            {
                Message = "Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }            

            if (!CurrentUser.IsInRole(RoleNames.Admin))
            {
                if (!string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
                {
                    if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                    {
                        Message = "Not your survey";
                        return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                    }
                }
                else
                {
                    if (surveyResponse.UserId.ToLower() != publicGuid.ToString().ToLower())
                    {
                        Message = "Not your survey";
                        return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                    }
                }
            }

            var viewModel = ResultsViewModel.Create(surveyResponse, false);
            viewModel.PublicGuid = publicGuid;
            //if (CurrentUser.IsInRole(RoleNames.Admin) || CurrentUser.IsInRole(RoleNames.User))
            //{
                viewModel.ShowPdfPrint = true;
            //}

            return View(viewModel);
        }


    }

    public class ResultsViewModel
    {
        public SurveyResponse SurveyResponse { get; set; }
        public bool ShowPdfPrint { get; set; }
        public Guid? PublicGuid { get; set; }

        public static ResultsViewModel Create(SurveyResponse surveyResponse, bool showPdfPrint)
        {
            Check.Require(surveyResponse != null);
            var viewModel = new ResultsViewModel {SurveyResponse = surveyResponse, ShowPdfPrint = showPdfPrint};
            viewModel.PublicGuid = null;
            return viewModel;
        }
        
    }

    public class ActiveSurveyViewModel
    {
        public IEnumerable<Survey> Surveys { get; set; }
        public bool IsPublic { get; set; }

        public static ActiveSurveyViewModel Create(IRepository repository, bool isPublic)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new ActiveSurveyViewModel { IsPublic = isPublic};
            viewModel.Surveys = repository.OfType<Survey>().Queryable.Where(a => a.IsActive);

            return viewModel;
        }
    }

    public class SingleAnswerSurveyResponseViewModel
    {
        public Survey Survey { get; set; }
        public SurveyResponse PendingSurveyResponse { get; set; }
        [Display(Name = "Total Questions")]
        public int TotalActiveQuestions { get; set; }
        [Display(Name = "Answered")]
        public int AnsweredQuestions { get; set; }
        public IList<Question> Questions { get; set; }
        public Question CurrentQuestion { get; set; }
        public bool CannotContinue { get; set; }
        public bool PendingSurveyResponseExists { get; set; }
        public SurveyResponse SurveyResponse { get; set; }
        public QuestionAnswerParameter SurveyAnswer { get; set; }
        public bool FromAdmin { get; set; }
        public bool DisplayBypass { get; set; }
        public Guid? PublicGuid { get; set; }

        public static SingleAnswerSurveyResponseViewModel Create(IRepository repository, Survey survey, SurveyResponse pendingSurveyResponse)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null);

            var viewModel = new SingleAnswerSurveyResponseViewModel{Survey = survey, PendingSurveyResponse = pendingSurveyResponse, SurveyResponse = new SurveyResponse(survey)};
            viewModel.Questions = viewModel.Survey.Questions
                .Where(a => a.IsActive && a.Category != null && a.Category.IsActive && a.Category.IsCurrentVersion)
                .OrderBy(a => a.Order).ToList();
            viewModel.TotalActiveQuestions = viewModel.Questions.Count;
            if (viewModel.PendingSurveyResponse != null)
            {
                viewModel.PendingSurveyResponseExists = true;
                viewModel.AnsweredQuestions = viewModel.PendingSurveyResponse.Answers.Count;
                var answeredQuestionIds = viewModel.PendingSurveyResponse.Answers.Select(a => a.Question.Id).ToList();
                viewModel.CurrentQuestion = viewModel.Questions
                    .Where(a => !answeredQuestionIds.Contains(a.Id))
                    .OrderBy(a => a.Order)
                    .FirstOrDefault();
            }
            else
            {
                viewModel.PendingSurveyResponseExists = false;
                viewModel.AnsweredQuestions = 0;
                viewModel.CurrentQuestion = viewModel.Questions.FirstOrDefault();
            }

            viewModel.DisplayBypass = false;

            return viewModel;
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
                .Where(a => a.IsActive && a.Category != null && a.Category.IsActive && a.Category.IsCurrentVersion)
                .OrderBy(a => a.Order).ToList();            
			return viewModel;
		}
	}

    public class SurveyReponseDetailViewModel
    {
        //TODO: Look into removing ths score duplicate code to use the service instead.
        public SurveyResponse SurveyResponse { get; set; }
        public IList<Scores> Scores { get; set; }
        public bool FromYourDetails { get; set; }

        public static SurveyReponseDetailViewModel Create(IRepository repository, SurveyResponse surveyResponse)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(surveyResponse != null);

            var viewModel = new SurveyReponseDetailViewModel {SurveyResponse = surveyResponse};

            //Get all the related categories that had answers.
            var relatedCategoryIds = viewModel.SurveyResponse.Answers.Select(x => x.Category.Id).Distinct().ToList();
            var bypassedAnswers = viewModel.SurveyResponse.Answers.Where(a => a.BypassScore);
            viewModel.Scores = new List<Scores>();
            foreach (var category in viewModel.SurveyResponse.Survey.Categories.Where(a => !a.DoNotUseForCalculations && relatedCategoryIds.Contains(a.Id)))
            {
                var score = new Scores();
                score.Category = category;
                var totalMax = repository.OfType<CategoryTotalMaxScore>().GetNullableById(category.Id);
                if (totalMax == null) //No Questions most likely
                {
                    continue;
                }
                score.MaxScore = totalMax.TotalMaxScore;
                foreach (var bypassedAnswer in bypassedAnswers.Where(a => a.Category == category))
                {
                    score.MaxScore = score.MaxScore - bypassedAnswer.Question.Responses.Where(a => a.IsActive).Max(a => a.Score);
                }
                //score.MaxScore = repository.OfType<CategoryTotalMaxScore>().GetNullableById(category.Id).TotalMaxScore;
                score.TotalScore = viewModel.SurveyResponse.Answers.Where(a => a.Category == category).Sum(b => b.Score);
                if (score.MaxScore == 0)
                {
                    score.Percent = 100;
                }
                else
                {
                    score.Percent = (score.TotalScore / score.MaxScore) * 100m; 
                }
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
        public string AnswerRange { get; set; } //For use when a time range is used
        public int ResponseId { get; set; }
        public bool Invalid { get; set; } 
        public string Message { get; set; } //Error message when invalid
        public int Score { get; set; } //Score when Valid
        public int? OpenEndedNumericAnswer { get; set; } //When Open ended and could parse int (may need to change for time, or other types currently not supported)
        public bool BypassQuestion { get; set; }
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
