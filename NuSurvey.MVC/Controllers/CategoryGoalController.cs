using System.Web.Mvc;
using AutoMapper;
using MvcContrib;
using NuSurvey.Core.Domain;
using NuSurvey.MVC.Controllers.Filters;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Helpers;

namespace NuSurvey.MVC.Controllers
{
    /// <summary>
    /// Controller for the CategoryGoal class
    /// </summary>
    [Admin]
    public class CategoryGoalController : ApplicationController
    {
	    private readonly IRepository<CategoryGoal> _categoryGoalRepository;

        public CategoryGoalController(IRepository<CategoryGoal> categoryGoalRepository)
        {
            _categoryGoalRepository = categoryGoalRepository;
        }

        /// <summary>
        /// #1
        /// GET: /CategoryGoal/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var categoryGoal = _categoryGoalRepository.GetNullableById(id);

            if (categoryGoal == null)
            {
                Message = "CategoryGoal Not Found";
                return this.RedirectToAction("Index", "Error");
            }

            var viewModel = CategoryGoalViewModel.Create(Repository, categoryGoal.Category);
            viewModel.CategoryGoal = categoryGoal;

            return View(viewModel);
        }

        /// <summary>
        /// #2
        /// GET: /CategoryGoal/Create
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var category = Repository.OfType<Category>().GetNullableById(id);
            if (category == null)
            {
                Message = "Category Not Found";
                return this.RedirectToAction("Index", "Error");
            }
            if (!category.IsCurrentVersion)
            {
                Message = "Category is not Current";
                return this.RedirectToAction("Index", "Error");
            }

			var viewModel = CategoryGoalViewModel.Create(Repository, category);
            
            return View(viewModel);
        } 

        /// <summary>
        /// #3
        /// POST: /CategoryGoal/Create
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <param name="categoryGoal"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int id, CategoryGoal categoryGoal)
        {
            var category = Repository.OfType<Category>().GetNullableById(id);
            if (category == null)
            {
                Message = "Category Not Found";
                return this.RedirectToAction("Index", "Error");
            }
            if (!category.IsCurrentVersion)
            {
                Message = "Category is not Current";
                return this.RedirectToAction("Index", "Error");
            }

            var categoryGoalToCreate = new CategoryGoal(category);

            Mapper.Map(categoryGoal, categoryGoalToCreate);

            ModelState.Clear();
            categoryGoalToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _categoryGoalRepository.EnsurePersistent(categoryGoalToCreate);

                Message = "CategoryGoal Created Successfully";

                return this.RedirectToAction("Edit", "Category", new{id = categoryGoalToCreate.Category.Id});
            }
            else
            {
				var viewModel = CategoryGoalViewModel.Create(Repository, categoryGoalToCreate.Category);
                viewModel.CategoryGoal = categoryGoalToCreate;

                return View(viewModel);
            }
        }

        /// <summary>
        /// #4
        /// GET: /CategoryGoal/Edit/5
        /// </summary>
        /// <param name="id">CategoryGoal id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var categoryGoal = _categoryGoalRepository.GetNullableById(id);

            if (categoryGoal == null)
            {
                Message = "CategoryGoal Not Found";
                return this.RedirectToAction("Index", "Error");
            }
            if (!categoryGoal.Category.IsCurrentVersion)
            {
                Message = "Related Category is not Current";
                return this.RedirectToAction("Index", "Error");
            }

			var viewModel = CategoryGoalViewModel.Create(Repository, categoryGoal.Category);
			viewModel.CategoryGoal = categoryGoal;

			return View(viewModel);
        }
        
        /// <summary>
        /// #5
        /// POST: /CategoryGoal/Edit/5
        /// </summary>
        /// <param name="id">CategoryGoal Id</param>
        /// <param name="categoryGoal"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, CategoryGoal categoryGoal)
        {
            var categoryGoalToEdit = _categoryGoalRepository.GetNullableById(id);

            if (categoryGoalToEdit == null)
            {
                Message = "CategoryGoal Not Found";
                return this.RedirectToAction("Index", "Error");
            }
            if (!categoryGoalToEdit.Category.IsCurrentVersion)
            {
                Message = "Related Category is not Current";
                return this.RedirectToAction("Index", "Error");
            }

            Mapper.Map(categoryGoal, categoryGoalToEdit);
            ModelState.Clear();
            categoryGoalToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _categoryGoalRepository.EnsurePersistent(categoryGoalToEdit);

                Message = "CategoryGoal Edited Successfully";
                
                return this.RedirectToAction("Edit", "Category", new {id = categoryGoalToEdit.Category.Id});
            }
            else
            {
				var viewModel = CategoryGoalViewModel.Create(Repository, categoryGoalToEdit.Category);
                viewModel.CategoryGoal = categoryGoalToEdit;

                return View(viewModel);
            }
        }

    }

	/// <summary>
    /// ViewModel for the CategoryGoal class
    /// </summary>
    public class CategoryGoalViewModel
	{
		public CategoryGoal CategoryGoal { get; set; }
        public Category Category { get; set; }
        public Survey Survey { get; set; }
 
		public static CategoryGoalViewModel Create(IRepository repository, Category category)
		{
			Check.Require(repository != null, "Repository must be supplied");
            Check.Require(category != null);
			
			var viewModel = new CategoryGoalViewModel
			                    {
			                        CategoryGoal = new CategoryGoal(category), 
                                    Category = category, 
// ReSharper disable PossibleNullReferenceException
                                    Survey = category.Survey
// ReSharper restore PossibleNullReferenceException
			                    };
 
			return viewModel;
		}
	}
}
