using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Error class
    /// </summary>
    public class ErrorController : ApplicationController
    {

        //
        // GET: /Error/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult FileNotFound()
        {
            return View();
        }
    }
}
