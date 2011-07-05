﻿using System;
using System.Web.Mvc;
using MvcContrib;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;


namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Print class
    /// </summary>
    [User]
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

        /// <summary>
        /// #1
        /// </summary>
        /// <param name="id">SurveyResponse Id</param>
        /// <returns></returns>
        public ActionResult Result(int id)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (surveyResponse == null)
            {
                return this.RedirectToAction<ErrorController>(a => a.FileNotFound());
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

        /// <summary>
        /// #2
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="beginDate">Filter for Begin Date</param>
        /// <param name="endDate">Filter for End Date</param>
        /// <returns></returns>
        [Admin]
        public ActionResult Results(int id, DateTime? beginDate, DateTime? endDate)
        {
            var survey = _surveyRepository.GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction<ErrorController>(a => a.FileNotFound());
            }

            if (beginDate == null)
            {
                beginDate = new DateTime(2000,01,01);
            }
            if (endDate == null)
            {
                endDate = DateTime.Now.AddYears(1);
            }

            if (endDate.Value.Date < beginDate.Value.Date)
            {
                endDate = beginDate.Value.Date;
            }

            beginDate = beginDate.Value.Date;
            endDate = endDate.Value.Date.AddDays(1).AddMinutes(-1);

            return _printService.PrintMultiple(id, beginDate, endDate);
        }

        [Admin] //This will be user, but they have to have access to the surveyResponses chosen
        public ActionResult PickResults(int id, PickedParameter[] picked)
        {
            var survey = _surveyRepository.GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction<ErrorController>(a => a.FileNotFound());
            }

            return _printService.PrintPickList(3, "1,2,3");
        }

        [Admin] //This will be user, but they have to have access to the surveyResponses chosen
        [BypassAntiForgeryToken]
        public ActionResult PickResults2(int id, int[] picked)
        {
            var survey = _surveyRepository.GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction<ErrorController>(a => a.FileNotFound());
            }

            return _printService.PrintPickList(3, "1,2,3");
        }
    }

}
