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

        public ActionResult Demo()
        {
            var timeList = new List<string>();
            var dateTime = DateTime.MinValue.Date;
            for (int i = 0; i < 48; i++)
            {
                var temp = dateTime.AddMinutes(15*i).ToString("h:mm   ");
                timeList.Add(temp);
            }

            var viewModel = DemoViewModel.Create();
            viewModel.Times = timeList;

            return View(viewModel);
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

        public class DemoViewModel
        {
            public List<string> Times { get; set;
            }
            public static DemoViewModel Create()
            {
                var viewModel = new DemoViewModel { };

                return viewModel;
            }
        }

    }
}
