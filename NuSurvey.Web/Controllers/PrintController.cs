﻿using System;
using System.Web.Mvc;
using MvcContrib;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
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

        public ActionResult PickResults2(int id, int[] picked)
        {
            var survey = _surveyRepository.GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction<ErrorController>(a => a.FileNotFound());
            }

            if (picked == null || picked.Length == 0)
            {
                Message = "No Survey Responses selected. Click on the rows of the table to select/deselect them.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            foreach (var i in picked)
            {
                var surveyResponse = Repository.OfType<SurveyResponse>().GetNullableById(i);
                if (surveyResponse == null)
                {
                    Message = string.Format("Selected Survey Response Not Found.'{0}'", i);
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
                if (!CurrentUser.IsInRole(RoleNames.Admin))
                {
                    if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                    {
                        Message = string.Format("Selected Survey Response not yours.'{0}'", i);
                        return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                    }
                }
                Check.Require(surveyResponse.Survey.Id == survey.Id, string.Format("SurveyResponse's survey id does not match {0} -- {1}", surveyResponse.Survey.Id, survey.Id));
            }

            var selectedAsString = string.Join(",", picked);
            

            return _printService.PrintPickList(3, selectedAsString);
        }
    }

}
