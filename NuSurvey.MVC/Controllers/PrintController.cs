using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MvcContrib;
using NuSurvey.Core.Domain;
using NuSurvey.MVC.Controllers.Filters;
using NuSurvey.MVC.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Attributes;


namespace NuSurvey.MVC.Controllers
{
    /// <summary>
    /// Controller for the Print class
    /// </summary>
    
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
        /// <param name="withBackground"> </param>
        /// <param name="publicGuid"> </param>
        /// <returns></returns>
        public ActionResult Result(int id, bool withBackground = false, Guid? publicGuid = null)
        {
            var surveyResponse = _surveyResponseRepository.GetNullableById(id);
            if (string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                surveyResponse = (SurveyResponse)Session[publicGuid.ToString()];
            }
            if (surveyResponse == null)
            {                
                return this.RedirectToAction("FileNotFound", "Error");
            }

            if (string.IsNullOrWhiteSpace(CurrentUser.Identity.Name))
            {
                return _printService.PrintSingle(id, Repository, Request, Url, true, surveyResponse);
            }

            if (!CurrentUser.IsInRole(RoleNames.Admin))
            {
                if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                {                    
                    return this.RedirectToAction("NotAuthorized", "Error");
                }
            }

            return _printService.PrintSingle(id, Repository, Request, Url, withBackground);
        }

        /// <summary>
        /// #2
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="beginDate">Filter for Begin Date</param>
        /// <param name="endDate">Filter for End Date</param>
        /// <returns></returns>
        [Admin]
        [User]
        public ActionResult Results(int id, DateTime? beginDate, DateTime? endDate)
        {
            var survey = _surveyRepository.GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction("FileNotFound", "Error");
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

            return _printService.PrintMultiple(id, Repository, Request, Url, beginDate, endDate);
        }

        /// <summary>
        /// #3
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="picked">Arry of SurveyResponse Ids</param>
        /// <returns></returns>
        [User]
        public ActionResult PickResults(int id, int[] picked, bool withBackground = false)
        {
            var survey = _surveyRepository.GetNullableById(id);
            if (survey == null)
            {
                return this.RedirectToAction("FileNotFound", "Error");
            }

            if (picked == null || picked.Length == 0)
            {
                Message = "No Survey Responses selected. Click on the rows of the table to select/deselect them.";
                return this.RedirectToAction("Index", "Error");
            }
            foreach (var i in picked)
            {
                var surveyResponse = Repository.OfType<SurveyResponse>().GetNullableById(i);
                if (surveyResponse == null)
                {
                    Message = string.Format("Selected Survey Response Not Found.'{0}'", i);
                    return this.RedirectToAction("Index", "Error");
                }
                if (!CurrentUser.IsInRole(RoleNames.Admin))
                {
                    if (surveyResponse.UserId.ToLower() != CurrentUser.Identity.Name.ToLower())
                    {
                        Message = string.Format("Selected Survey Response not yours.'{0}'", i);
                        return this.RedirectToAction("NotAuthorized", "Error");
                    }
                }
                Check.Require(surveyResponse.Survey.Id == survey.Id, string.Format("SurveyResponse's survey id does not match {0} -- {1}", surveyResponse.Survey.Id, survey.Id));
            }
                      

            return _printService.PrintPickList(survey.Id, Repository, Request, Url, picked, withBackground);
        }

        //[Admin]
        //public ActionResult TestPdf()
        //{
        //    var blah = GetAbsoluteUrl(Request, Url, "~/Images/pdfCheckbox.png");

        //    Image checkBoxImage = Image.GetInstance(blah);
        //    var doc1 = new Document(PageSize.LETTER, 80 /* left */, 36 /* right */, 62 /* top */, 0 /* bottom */);
        //    var ms = new MemoryStream();
        //    var writer = PdfWriter.GetInstance(doc1, ms);
        //    Font arial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.NORMAL, BaseColor.BLACK);
        //    Font arialBold = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.BOLD, BaseColor.BLACK);
            


        //    var table = new PdfPTable(2);
        //    //actual width of table in points
        //    table.TotalWidth = 454f;
        //    //fix the absolute width of the table
        //    table.LockedWidth = true;
            
        //    //table.DefaultCell.Border = 0;
        //    table.DefaultCell.PaddingTop = 10f;
           

        //    //relative col widths in proportions - 1/3 and 2/3

        //    var widths = new[] { 1f, 20f };
        //    table.SetWidths(widths);
        //    table.HorizontalAlignment = 0;
        //    //leave a gap before and after the table
        //    table.SpacingBefore = 20f;
        //    table.SpacingAfter = 0f;
            

        //    //0
        //    table.AddCell(string.Empty);
        //    table.AddCell(string.Empty);

        //    //1
        //    table.AddCell(string.Empty);
        //    table.AddCell(new Paragraph("Thank you Danielle for taking the time to complete the My Child at Meal Time quiz. We hope this feedback will help you and your family make healthy feeding choices.", arial));
            
        //    //2
        //    table.AddCell(string.Empty);
        //    table.AddCell(string.Empty);
            
        //    //3
        //    table.AddCell(string.Empty);
        //    table.AddCell(new Paragraph("\nGreat job!  You are not using food as a reward.", arialBold));
            
        //    //4
        //    table.AddCell(string.Empty);
        //    table.AddCell(string.Empty);

        //    //5
        //    table.AddCell(checkBoxImage);
        //    table.AddCell(new Paragraph("You may want to make food more child friendly.", arialBold));

        //    //6
        //    table.AddCell(string.Empty);
        //    table.AddCell(string.Empty);

        //    //7
        //    table.AddCell(checkBoxImage);
        //    table.AddCell(new Paragraph("Let your child pick the foods she wants to eat from foods already prepared 3 times this week.", arial));

        //    //8
        //    table.AddCell(checkBoxImage);
        //    table.AddCell(new Paragraph("Let your child help prepare 2 meals this week.", arial));

        //    //9
        //    table.AddCell(checkBoxImage);
        //    table.AddCell(new Paragraph("Make food fun for your child 2 times this week.  Try cutting sandwiches into shapes.  Try making faces with food.", arial));

        //    //10
        //    table.AddCell(string.Empty);
        //    table.AddCell(string.Empty);

        //    //11
        //    table.AddCell(checkBoxImage);
        //    table.AddCell(new Paragraph("You may want to talk with your child about food during mealtime.", arialBold));

        //    //12
        //    table.AddCell(string.Empty);
        //    table.AddCell(string.Empty);

        //    //13
        //    table.AddCell(checkBoxImage);
        //    table.AddCell(new Paragraph("Ask your child a question about the food he is eating at each meal this week.", arial));

        //    //14
        //    table.AddCell(checkBoxImage);
        //    table.AddCell(new Paragraph("Tell your child that a healthy food he is eating is good for him 2 times this week.  Try, “I love that you are drinking milk. It will make you strong!”", arial));

        //    //15
        //    table.AddCell(checkBoxImage);
        //    table.AddCell(new Paragraph("Praise your child for trying a new food 2 times this week. ", arial));


        //    table.GetRow(0).MaxHeights = 70f;
        //    table.GetRow(1).MaxHeights = 65f;
        //    table.GetRow(2).MaxHeights = 6f;
        //    table.GetRow(3).MaxHeights = 50f;            
        //    table.GetRow(4).MaxHeights = 77f;
        //    table.GetRow(5).MaxHeights = 48f;
        //    table.GetRow(6).MaxHeights = 32f;
        //    table.GetRow(7).MaxHeights = 40;
        //    table.GetRow(8).MaxHeights = 40;
        //    table.GetRow(9).MaxHeights = 40;
        //    table.GetRow(10).MaxHeights = 31;
        //    table.GetRow(11).MaxHeights = 44f;
        //    table.GetRow(12).MaxHeights = 38f;
        //    table.GetRow(13).MaxHeights = 40;
        //    table.GetRow(14).MaxHeights = 40;
        //    table.GetRow(15).MaxHeights = 40;


        //    doc1.Open();
        //    doc1.Add(table);
        //    doc1.NewPage();
        //    doc1.Add(table);
        //    doc1.Close();
        //    var bytes = ms.ToArray();
            

        //    return new FileContentResult(bytes, "application/pdf");
        //}

        //private string GetAbsoluteUrl(HttpRequestBase request, UrlHelper url, string relative)
        //{
        //    return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url.Content(relative));
        //}
    }

}
