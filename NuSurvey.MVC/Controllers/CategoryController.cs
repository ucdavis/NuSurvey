using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MvcContrib;
using NuSurvey.Core.Domain;
using NuSurvey.MVC.Controllers.Filters;
using NuSurvey.MVC.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;

namespace NuSurvey.MVC.Controllers
{
    /// <summary>
    /// Controller for the Category class
    /// </summary>
    [Admin]
    public class CategoryController : ApplicationController
    {
	    private readonly IRepository<Category> _categoryRepository;
        private readonly IArchiveService _archiveService;

        public CategoryController(IRepository<Category> categoryRepository, IArchiveService archiveService)
        {
            _categoryRepository = categoryRepository;
            _archiveService = archiveService;
        }

        /// <summary>
        /// #1
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

            var viewModel = CategoryListViewModel.Create(Repository, survey);
            return View(viewModel);
        }

        /// <summary>
        /// #2
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="tableOrder">Array of category Ids</param>
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
                var i1 = i;
                survey.Categories.Where(a => a.Id == tableOrder[i1]).Single().Rank = i + 1;
            }

            ModelState.Clear();
            survey.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                Repository.OfType<Survey>().EnsurePersistent(survey);
                return new JsonNetResult(true);
            }

            return new JsonNetResult(false);
        }

        /// <summary>
        /// #3
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
        /// #4
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
        /// #5
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
            if (!category.IsCurrentVersion)
            {
                Message = "Category is not Current";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = CategoryViewModel.Create(Repository, category.Survey);
            viewModel.Category = category;
            viewModel.HasRelatedAnswers = Repository.OfType<Answer>().Queryable.Where(a => a.Category.Id == viewModel.Category.Id).Any();

            return View(viewModel);
        }

        /// <summary>
        /// #6
        /// POST: /Category/Edit/5
        /// When the category is mapped to the categoryToEdit is very important with regards to the versioning of the old data
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
            if (!categoryToEdit.IsCurrentVersion)
            {
                Message = "Category is not Current";
                return this.RedirectToAction<ErrorController>(a => a.Index());
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
            category.PreviousVersion = categoryToEdit.PreviousVersion;
            category.Survey = categoryToEdit.Survey;
            
            ModelState.Clear();
            category.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                if (isNewVersion)
                {
                    //ArchiveCategory(id, category); //Don't care about the returned value here
                    _archiveService.ArchiveCategory(Repository, id, category);
                    Message = "Category Edited and Versioned Successfully";
                }
                else
                {
                    Mapper.Map(category, categoryToEdit);
                    _categoryRepository.EnsurePersistent(categoryToEdit);
                    Message = "Category Edited Successfully";
                }
                
                
                //return this.RedirectToAction<SurveyController>(a => a.Edit(categoryToEdit.Survey.Id));
                return this.RedirectToAction(a => a.Edit(categoryToEdit.Id));
            }
            else
            {
                Message = "Unable to Edit Category";
                var viewModel = CategoryViewModel.Create(Repository, categoryToEdit.Survey);
                viewModel.Category = category;
                viewModel.HasRelatedAnswers = Repository.OfType<Answer>().Queryable.Where(a => a.Category.Id == viewModel.Category.Id).Any();

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
        [Display(Name = "Has Related Answers")]
        public bool HasRelatedAnswers { get; set; }
 
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
