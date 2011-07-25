using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
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

        /// <summary>
        /// #3
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="picked">Arry of SurveyResponse Ids</param>
        /// <returns></returns>
        public ActionResult PickResults(int id, int[] picked)
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
            

            return _printService.PrintPickList(survey.Id, selectedAsString);
        }

        [Admin]
        public ActionResult TestPdf()
        {
            var blah = GetAbsoluteUrl(Request, Url, "~/Images/pdfCheckbox.png");

            Image checkBoxImage = Image.GetInstance(blah);
            var doc1 = new Document(PageSize.LETTER, 36 /* left */, 36 /* right */, 62 /* top */, 52 /* bottom */);
            var ms = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc1, ms);
            Font arial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.NORMAL, BaseColor.BLACK);
            Font arialBold = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.BOLD, BaseColor.BLACK);
            


            var table = new PdfPTable(2);
            //actual width of table in points
            table.TotalWidth = 454f;
            //fix the absolute width of the table
            table.LockedWidth = true;
            
            //table.DefaultCell.Border = 0;
            table.DefaultCell.PaddingTop = 10f;
           

            //relative col widths in proportions - 1/3 and 2/3

            var widths = new[] { 1f, 20f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;
            //leave a gap before and after the table
            table.SpacingBefore = 20f;
            table.SpacingAfter = 0f;

            //0
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);

            //1
            table.AddCell(string.Empty);
            table.AddCell(new Paragraph("Thank you Danielle for taking the time to complete the My Child at Meal Time quiz. We hope this feedback will help you and your family make healthy feeding choices.", arial));
            
            //2
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);
            
            //3
            table.AddCell(string.Empty);
            table.AddCell(new Paragraph("Great job!  You are not using food as a reward.", arialBold));
            
            //4
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);

            //5
            table.AddCell(checkBoxImage);
            table.AddCell(new Paragraph("You may want to make food more child friendly.", arialBold));

            //6
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);

            table.GetRow(0).MaxHeights = 70f;
            table.GetRow(1).MaxHeights = 65f;
            table.GetRow(2).MaxHeights = 6f;
            table.GetRow(3).MaxHeights = 50f;
            table.GetRow(4).MaxHeights = 77f;
            table.GetRow(5).MaxHeights = 48f;
            table.GetRow(6).MaxHeights = 32f;

            doc1.Open();
            doc1.Add(table);
            doc1.NewPage();
            doc1.Add(table);
            doc1.Close();
            var bytes = ms.ToArray();
            

            return new FileContentResult(bytes, "application/pdf");
        }

        private string GetAbsoluteUrl(HttpRequestBase request, UrlHelper url, string relative)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url.Content(relative));
        }
    }

}
