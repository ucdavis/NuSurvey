using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Web.Attributes;

namespace NuSurvey.Web.Controllers
{
    [HandleTransactionsManually]
    [Authorize]
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            //ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }
        [Admin]
        public ViewResult Sample()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
