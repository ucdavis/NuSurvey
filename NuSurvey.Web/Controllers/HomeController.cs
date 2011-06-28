using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Web.Attributes;
//using Elmah;
using MvcContrib;

namespace NuSurvey.Web.Controllers
{
    [HandleTransactionsManually]
    [Authorize]
    public class HomeController : ApplicationController
    {
        /// <summary>
        /// #1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var viewModel = HomeViewModel.Create(CurrentUser.IsInRole(RoleNames.Admin), CurrentUser.IsInRole(RoleNames.User));
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

        [Admin]
        public ActionResult ResetCache()
        {
            HttpContext.Cache.Remove("ServiceMessages");
            var cache = ControllerContext.HttpContext.Cache["ServiceMessages"];
            var messsages = ViewData["ServiceMessages"];

            HttpContext.Cache.Remove("ServiceMessages");

            //ControllerContext.HttpContext.Cache.Remove("ServiceMessages");

            return this.RedirectToAction(a => a.Index());
        }


        public class HomeViewModel
        {

            public bool Admin { get; set; }
            public bool User { get; set; }

            public static HomeViewModel Create(bool isAdmin, bool isUser)
            {
                var viewModel = new HomeViewModel { Admin = isAdmin, User = isUser};

                return viewModel;
            }
        }

    }
}
