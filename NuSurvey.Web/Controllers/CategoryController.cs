using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using NuSurvey.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.Helpers;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Category class
    /// </summary>
    public class CategoryController : ApplicationController
    {
	    private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
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
        
        ////
        //// POST: /Category/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, Category category)
        //{
        //    var categoryToEdit = _categoryRepository.GetNullableById(id);

        //    if (categoryToEdit == null) return RedirectToAction("Index");

        //    TransferValues(category, categoryToEdit);

        //    if (ModelState.IsValid)
        //    {
        //        _categoryRepository.EnsurePersistent(categoryToEdit);

        //        Message = "Category Edited Successfully";

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        var viewModel = CategoryViewModel.Create(Repository);
        //        viewModel.Category = category;

        //        return View(viewModel);
        //    }
        //}
        
        ////
        //// GET: /Category/Delete/5 
        //public ActionResult Delete(int id)
        //{
        //    var category = _categoryRepository.GetNullableById(id);

        //    if (category == null) return RedirectToAction("Index");

        //    return View(category);
        //}

        ////
        //// POST: /Category/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, Category category)
        //{
        //    var categoryToDelete = _categoryRepository.GetNullableById(id);

        //    if (categoryToDelete == null) return RedirectToAction("Index");

        //    _categoryRepository.Remove(categoryToDelete);

        //    Message = "Category Removed Successfully";

        //    return RedirectToAction("Index");
        //}
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Category source, Category destination)
        {
			//Recommendation: Use AutoMapper
            Mapper.Map(source, destination);
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
}
