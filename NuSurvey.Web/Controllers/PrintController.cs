using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Helpers;


namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Print class
    /// </summary>
    [Authorize]
    public class PrintController : ApplicationController
    {
        private readonly IRepository<SurveyResponse> _surveyResponseRepository;
        private readonly IPrintService _printService;


        public PrintController(IRepository<SurveyResponse> surveyResponseRepository, IPrintService printService)
        {
            _surveyResponseRepository = surveyResponseRepository;
            _printService = printService;
        }

        public ActionResult Result(int id)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (surveyResponse == null)
            {
                Message = "Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!CurrentUser.IsInRole(RoleNames.Admin))
            {
                if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                {
                    return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                }
            }

            return _printService.PrintSingle(id);
        }
    }

}
