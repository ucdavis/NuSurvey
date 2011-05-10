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
using UCDArch.Data.NHibernate;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;
using System.Linq.Expressions;
using System.Linq;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Category class
    /// </summary>
    [Admin]
    public class CategoryController : ApplicationController
    {
	    private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        public ActionResult ReOrder(int id)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction<SurveyController>(a => a.Index());
            }

            var viewModel = CategoryListViewModel.Create(Repository, survey);
            return View(viewModel);
        }

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
                var i1 = i;
                survey.Categories.Where(a => a.Id == tableOrder[i1]).Single().Rank = i + 1;
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
        //// GET: /Category/Details/5
        //public ActionResult Details(int id)
        //{
        //    var category = _categoryRepository.GetNullableById(id);

        //    if (category == null) return RedirectToAction("Index");

        //    return View(category);
        //}

        /// <summary>
        /// GET: /Category/Create
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction<SurveyController>(a => a.Index());
            }

			var viewModel = CategoryViewModel.Create(Repository, survey);
            
            return View(viewModel);
        } 

        /// <summary>
        /// POST: /Category/Create
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int id, Category category)
        {
            var survey = Repository.OfType<Survey>().GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction<SurveyController>(a => a.Index());
            }

            var categoryToCreate = new Category(survey);
            categoryToCreate.CreateDate = DateTime.Now;
            categoryToCreate.LastUpdate = categoryToCreate.CreateDate;

            Mapper.Map(category, categoryToCreate);
            

            ModelState.Clear();
            categoryToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _categoryRepository.EnsurePersistent(categoryToCreate);

                Message = "Category Created Successfully";

                return this.RedirectToAction(a => a.Edit(categoryToCreate.Id));
            }
            else
            {
				var viewModel = CategoryViewModel.Create(Repository, survey);
                viewModel.Category = categoryToCreate;

                return View(viewModel);
            }
        }

        
        /// <summary>
        /// GET: /Category/Edit/5
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var category = _categoryRepository.GetNullableById(id);

            if (category == null)
            {
                Message = "Category not found to edit.";
                return this.RedirectToAction<SurveyController>(a => a.Index());
            }

            var viewModel = CategoryViewModel.Create(Repository, category.Survey);
            viewModel.Category = category;

            return View(viewModel);
        }

        /// <summary>
        /// POST: /Category/Edit/5
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, Category category)
        {
            var categoryToEdit = _categoryRepository.GetNullableById(id);

            if (categoryToEdit == null)
            {
                Message = "Category not found to edit.";
                return this.RedirectToAction<SurveyController>(a => a.Index());
            }

            var isNewVersion = false;
            if (Repository.OfType<Answer>().Queryable.Where(a => a.Category.Id == categoryToEdit.Id).Any())
            {
                //Ok, there are related questions
                if (categoryToEdit.IsActive != category.IsActive || 
                    categoryToEdit.DoNotUseForCalculations != category.DoNotUseForCalculations)
                {
                    //ok, a field has been changed which could effect the score, so we want to version it.
                    isNewVersion = true;
                }
            }


            categoryToEdit.LastUpdate = DateTime.Now;
            category.CategoryGoals = categoryToEdit.CategoryGoals;
            category.Questions = categoryToEdit.Questions;

            Mapper.Map(category, categoryToEdit);
            


            ModelState.Clear();
            categoryToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                if (isNewVersion)
                {
                    var oldVersion = _categoryRepository.GetNullableById(id);
                    var newVersion = new Category(oldVersion.Survey);

                    newVersion.Rank = oldVersion.Rank;
                    newVersion.LastUpdate = DateTime.Now;
                    newVersion.CreateDate = newVersion.LastUpdate;                   

                    foreach (var categoryGoal in oldVersion.CategoryGoals)
                    {
                        var categoryGoalToDuplicate = new CategoryGoal();
                        Mapper.Map(categoryGoal, categoryGoalToDuplicate);
                        newVersion.AddCategoryGoal(categoryGoalToDuplicate);
                    }
                    foreach (var question in oldVersion.Questions)
                    {
                        var questionToDuplicate = new Question(oldVersion.Survey);
                        questionToDuplicate.Order = question.Order;
                        foreach (var response in question.Responses)
                        {
                            questionToDuplicate.AddResponse(response);
                        }
                        Mapper.Map(question, questionToDuplicate);
                        newVersion.AddQuestions(questionToDuplicate);
                    }


                    newVersion.IsActive = category.IsActive;
                    newVersion.Name = category.Name;
                    newVersion.Affirmation = category.Affirmation;
                    newVersion.Encouragement = category.Encouragement;
                    newVersion.DoNotUseForCalculations = category.DoNotUseForCalculations;

                    oldVersion.IsCurrentVersion = false;

                    _categoryRepository.EnsurePersistent(newVersion);
                    _categoryRepository.EnsurePersistent(oldVersion);
                }
                else
                {
                    _categoryRepository.EnsurePersistent(categoryToEdit); 
                }
                
                Message = "Category Edited Successfully";

                return this.RedirectToAction<SurveyController>(a => a.Edit(categoryToEdit.Survey.Id));
            }
            else
            {
                var viewModel = CategoryViewModel.Create(Repository, categoryToEdit.Survey);
                viewModel.Category = category;

                return View(viewModel);
            }
        }
               

    }

	/// <summary>
    /// ViewModel for the Category class
    /// </summary>
    public class CategoryViewModel
	{
        public Survey Survey { get; set; }
		public Category Category { get; set; }        
 
		public static CategoryViewModel Create(IRepository repository, Survey survey)
		{
			Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null);
			
			var viewModel = new CategoryViewModel {Survey = survey, Category = new Category(survey)};            
 
			return viewModel;
		}
	}

    /// <summary>
    /// ViewModel for the Category class
    /// </summary>
    public class CategoryListViewModel
    {
        public Survey Survey { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public static CategoryListViewModel Create(IRepository repository, Survey survey)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(survey != null);

            var viewModel = new CategoryListViewModel { Survey = survey };
            viewModel.Categories = viewModel.Survey.Categories.Where(a => a.IsCurrentVersion).OrderBy(a => a.Rank);

            return viewModel;
        }
    }
}
