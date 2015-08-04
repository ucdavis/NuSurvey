using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NuSurvey.MVC.Controllers.Filters;
using UCDArch.Web.Attributes;
//using Elmah;
using MvcContrib;

namespace NuSurvey.MVC.Controllers
{
    [HandleTransactionsManually]
    public class HomeController : ApplicationController
    {
        /// <summary>
        /// #1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(bool parent = false)
        {
            var viewModel = HomeViewModel.Create(CurrentUser.IsInRole(RoleNames.Admin), CurrentUser.IsInRole(RoleNames.User), CurrentUser.IsInRole(RoleNames.ProgramDirector));

            ViewBag.OnlyParent = parent;
            
            return View(viewModel);
        }

        /// <summary>
        /// #2
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// #3
        /// </summary>
        /// <returns></returns>
        [Admin]
        public ActionResult Administration()
        {
            return View();
        }

        /// <summary>
        /// #4
        /// </summary>
        /// <returns></returns>
        [Admin]
        public ViewResult Sample()
        {
            return View();
        }

        /// <summary>
        /// #5
        /// </summary>
        /// <returns></returns>
        [Admin]
        public ActionResult ResetCache()
        {
            HttpContext.Cache.Remove("ServiceMessages");
            var cache = ControllerContext.HttpContext.Cache["ServiceMessages"];
            var messsages = ViewData["ServiceMessages"];

            HttpContext.Cache.Remove("ServiceMessages");

            //ControllerContext.HttpContext.Cache.Remove("ServiceMessages");

            return this.RedirectToAction(a => a.Index(false));
        }


        public class HomeViewModel
        {

            public bool Admin { get; set; }
            public bool ProgramDirector { get; set; }
            public bool User { get; set; }

            public static HomeViewModel Create(bool isAdmin, bool isUser, bool isProgramDirector)
            {
                var viewModel = new HomeViewModel { Admin = isAdmin, User = isUser, ProgramDirector = isProgramDirector};

                return viewModel;
            }
        }

    }
}
