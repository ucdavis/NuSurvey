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
        private readonly IRepository<Survey> _surveyRepository;
        private readonly IRepository<SurveyResponse> _surveyResponseRepository;
        private readonly IPrintService _printService;


        public PrintController(IRepository<Survey> surveyRepository, IRepository<SurveyResponse> surveyResponseRepository, IPrintService printService)
        {
            _surveyRepository = surveyRepository;
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

        [Admin]
        public ActionResult Results(int id, DateTime? beginDate, DateTime? endDate)
        {
            var survey = _surveyRepository.GetNullableById(id);
            if (survey == null)
            {
                Message = "Not Found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (beginDate == null)
            {
                beginDate = new DateTime(2000,01,01);
            }
            if (endDate == null)
            {
                endDate = DateTime.Now.AddYears(1);
            }

            if (endDate.Value.Date <= beginDate.Value.Date)
            {
                endDate = beginDate.Value.Date.AddDays(1);
            }

            return _printService.PrintMultiple(id, beginDate, endDate);

            throw new NotImplementedException();
        }
    }

}
