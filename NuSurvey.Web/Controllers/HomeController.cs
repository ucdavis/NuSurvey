using System.Web.Mvc;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Web.Attributes;
using Elmah;

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
        [Authorize]
        public ActionResult About()
        {
            return View();
        }

        [Admin]
        public ActionResult Administration()
        {
            return View();
        }
    }
}
