using System.Web.Mvc;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Web.Attributes;
//using Elmah;
//using MvcContrib;

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
            //HttpContext.Cache.Remove("ServiceMessage");
            //var cache = ControllerContext.HttpContext.Cache["ServiceMessage"];

            //var messsages = ViewData["ServiceMessage"];
            return View();
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

    }
}
