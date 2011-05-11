using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.Helpers;

namespace NuSurvey.Web.Controllers
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
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = CategoryGoalViewModel.Create(Repository, categoryGoal.Category);
            viewModel.CategoryGoal = categoryGoal;

            return View(viewModel);
        }

        /// <summary>
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
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!category.IsCurrentVersion)
            {
                Message = "Category is not Current";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

			var viewModel = CategoryGoalViewModel.Create(Repository, category);
            
            return View(viewModel);
        } 

        /// <summary>
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
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!category.IsCurrentVersion)
            {
                Message = "Category is not Current";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var categoryGoalToCreate = new CategoryGoal(category);

            Mapper.Map(categoryGoal, categoryGoalToCreate);

            ModelState.Clear();
            categoryGoalToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _categoryGoalRepository.EnsurePersistent(categoryGoalToCreate);

                Message = "CategoryGoal Created Successfully";

                return this.RedirectToAction<CategoryController>(a => a.Edit(categoryGoalToCreate.Category.Id));
            }
            else
            {
				var viewModel = CategoryGoalViewModel.Create(Repository, categoryGoalToCreate.Category);
                viewModel.CategoryGoal = categoryGoalToCreate;

                return View(viewModel);
            }
        }

        /// <summary>
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
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!categoryGoal.Category.IsCurrentVersion)
            {
                Message = "Related Category is not Current";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

			var viewModel = CategoryGoalViewModel.Create(Repository, categoryGoal.Category);
			viewModel.CategoryGoal = categoryGoal;

			return View(viewModel);
        }
        
        //
        // POST: /CategoryGoal/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CategoryGoal categoryGoal)
        {
            var categoryGoalToEdit = _categoryGoalRepository.GetNullableById(id);

            if (categoryGoalToEdit == null)
            {
                Message = "CategoryGoal Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!categoryGoalToEdit.Category.IsCurrentVersion)
            {
                Message = "Related Category is not Current";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            Mapper.Map(categoryGoal, categoryGoalToEdit);
            ModelState.Clear();
            categoryGoalToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _categoryGoalRepository.EnsurePersistent(categoryGoalToEdit);

                Message = "CategoryGoal Edited Successfully";

                return this.RedirectToAction<CategoryController>(a => a.Edit(categoryGoalToEdit.Category.Id));
            }
            else
            {
				var viewModel = CategoryGoalViewModel.Create(Repository, categoryGoalToEdit.Category);
                viewModel.CategoryGoal = categoryGoalToEdit;

                return View(viewModel);
            }
        }
        
        ////
        //// GET: /CategoryGoal/Delete/5 
        //public ActionResult Delete(int id)
        //{
        //    var categoryGoal = _categoryGoalRepository.GetNullableById(id);

        //    if (categoryGoal == null) return RedirectToAction("Index");

        //    return View(categoryGoal);
        //}

        ////
        //// POST: /CategoryGoal/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, CategoryGoal categoryGoal)
        //{
        //    var categoryGoalToDelete = _categoryGoalRepository.GetNullableById(id);

        //    if (categoryGoalToDelete == null) return RedirectToAction("Index");

        //    _categoryGoalRepository.Remove(categoryGoalToDelete);

        //    Message = "CategoryGoal Removed Successfully";

        //    return RedirectToAction("Index");
        //}
              

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
                                    Survey = category.Survey
			                    };
 
			return viewModel;
		}
	}
}
