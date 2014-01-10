using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NuSurvey.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.Linq;
using System.Linq.Expressions;

namespace NuSurvey.Web.Services
{
    public interface IPrintService
    {
        FileContentResult PrintSingle(int id, IRepository repository, HttpRequestBase request, UrlHelper url, bool useBackgroundImage = false, SurveyResponse publicSurveyResponse = null);
        FileContentResult PrintMultiple(int id, IRepository repository, HttpRequestBase request, UrlHelper url, DateTime? beginDate, DateTime? endDate);
        FileContentResult PrintPickList(int id, IRepository repository, HttpRequestBase request, UrlHelper url, int[] surveyResponseIds, bool useBackgroundImage = false);
    }

    public class PrintService : IPrintService
    {
        //public virtual FileContentResult PrintSingle(int id)
        //{
        //    var rview = new Microsoft.Reporting.WebForms.ReportViewer();
        //    rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServer"]);

        //    rview.ServerReport.ReportPath = @"/NuSurvey/Report_SurveyResponse";

        //    var paramList = new List<Microsoft.Reporting.WebForms.ReportParameter>();

        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SurveyResponseId", id.ToString()));
        //    //paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ProposalId", proposalId != null && proposalId.Value > 0 ? proposalId.Value.ToString() : string.Empty));
        //    //paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ShowComments", showComments.ToString().ToLower()));

        //    rview.ServerReport.SetParameters(paramList);

        //    string mimeType, encoding, extension;
        //    string[] streamids;
        //    Microsoft.Reporting.WebForms.Warning[] warnings;

        //    string format = "PDF";

        //    string deviceInfo = "<DeviceInfo>" +
        //                        "<SimplePageHeaders>True</SimplePageHeaders>" +
        //                        "<HumanReadablePDF>True</HumanReadablePDF>" +   // this line disables the compression done by SSRS 2008 so that it can be merged.
        //                        "</DeviceInfo>";

        //    byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

        //    return new FileContentResult(bytes, "application/pdf");// File(bytes, "application/pdf");
        //}


        ///// <summary>
        ///// Print Multiple
        ///// </summary>
        ///// <param name="id">Survey Id</param>
        ///// <param name="beginDate">Begin Date Filter</param>
        ///// <param name="endDate">End Date Filter</param>
        ///// <returns></returns>
        //public virtual FileContentResult PrintMultiple(int id, DateTime? beginDate, DateTime? endDate)
        //{
        //    var rview = new Microsoft.Reporting.WebForms.ReportViewer();
        //    rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServer"]);

        //    rview.ServerReport.ReportPath = @"/NuSurvey/Report_MultiSurveyResponse";

        //    var paramList = new List<Microsoft.Reporting.WebForms.ReportParameter>();

        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Id", id.ToString()));
        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("BeginDate", beginDate.ToString()));
        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("EndDate", endDate.ToString()));

        //    rview.ServerReport.SetParameters(paramList);

        //    string mimeType, encoding, extension;
        //    string[] streamids;
        //    Microsoft.Reporting.WebForms.Warning[] warnings;

        //    string format = "PDF";

        //    string deviceInfo = "<DeviceInfo>" +
        //                        "<SimplePageHeaders>True</SimplePageHeaders>" +
        //                        "<HumanReadablePDF>True</HumanReadablePDF>" +   // this line disables the compression done by SSRS 2008 so that it can be merged.
        //                        "</DeviceInfo>";

        //    byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

        //    return new FileContentResult(bytes, "application/pdf");// File(bytes, "application/pdf");
        //}

        //public virtual FileContentResult PrintPickList(int id, string delimitedList)
        //{
        //    var rview = new Microsoft.Reporting.WebForms.ReportViewer();
        //    rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServer"]);

        //    rview.ServerReport.ReportPath = @"/NuSurvey/Report_PickListSurveyResponse";

        //    var paramList = new List<Microsoft.Reporting.WebForms.ReportParameter>();

        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("Id", id.ToString()));
        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SurveyResponseIds", delimitedList));


        //    rview.ServerReport.SetParameters(paramList);

        //    string mimeType, encoding, extension;
        //    string[] streamids;
        //    Microsoft.Reporting.WebForms.Warning[] warnings;

        //    string format = "PDF";

        //    string deviceInfo = "<DeviceInfo>" +
        //                        "<SimplePageHeaders>True</SimplePageHeaders>" +
        //                        "<HumanReadablePDF>True</HumanReadablePDF>" +   // this line disables the compression done by SSRS 2008 so that it can be merged.
        //                        "</DeviceInfo>";

        //    byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

        //    return new FileContentResult(bytes, "application/pdf");// File(bytes, "application/pdf");
        //}

        /// <summary>
        /// Print a single PDF
        /// </summary>
        /// <param name="id"></param>
        /// <param name="repository"></param>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <param name="useBackgroundImage"> </param>
        /// <returns></returns>
        public virtual FileContentResult PrintSingle(int id, IRepository repository, HttpRequestBase request, UrlHelper url, bool useBackgroundImage = false, SurveyResponse publicSurveyResponse = null)
        {
            var surveyResponse = repository.OfType<SurveyResponse>().GetNullableById(id);

            if (publicSurveyResponse != null)
            {
                surveyResponse = publicSurveyResponse;
            }

            Check.Require(surveyResponse != null);

            var checkboxPath = GetAbsoluteUrl(request, url, "~/Images/pdfCheckbox.png");

            Image checkBoxImage = Image.GetInstance(checkboxPath);
            var doc1 = new Document(PageSize.LETTER, 80 /* left */, 36 /* right */, 62 /* top */, 0 /* bottom */);
            var ms = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc1, ms);
            Font arial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.NORMAL, BaseColor.BLACK);
            Font arialBold = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.BOLD, BaseColor.BLACK);


            doc1.Open();
            if (useBackgroundImage)
            {
                try
                {
                    //Example to print a background image
                    var goalPath = GetAbsoluteUrl(request, url, string.Format("~/Images/{0}_pdf.jpg", surveyResponse.Survey.ShortName.Trim().ToUpper()));

                    Image goalImg = Image.GetInstance(goalPath);
                    goalImg.ScaleToFit(doc1.PageSize.Width, doc1.PageSize.Height);
                    goalImg.SetAbsolutePosition(0, 0);
                    goalImg.Alignment = Image.UNDERLYING;

                    doc1.Add(goalImg);
                }
                catch (Exception)
                {
                    //nothing
                }
            }


            var table = BuildTable(surveyResponse, arial, arialBold, checkBoxImage);      
            doc1.Add(table);
            doc1.Close();
            var bytes = ms.ToArray();


            return new FileContentResult(bytes, "application/pdf");

        }


        /// <summary>
        /// Print Multiple
        /// </summary>
        /// <param name="id">Survey Id</param>
        /// <param name="url"></param>
        /// <param name="beginDate">Begin Date Filter</param>
        /// <param name="endDate">End Date Filter</param>
        /// <param name="repository"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual FileContentResult PrintMultiple(int id, IRepository repository, HttpRequestBase request, UrlHelper url, DateTime? beginDate, DateTime? endDate)
        {
            var survey = repository.OfType<Survey>().GetNullableById(id);
            Check.Require(survey != null);
            var surveyResponses = survey.SurveyResponses.Where(a => beginDate.Value <= a.DateTaken && endDate.Value >= a.DateTaken);


            var checkboxPath = GetAbsoluteUrl(request, url, "~/Images/pdfCheckbox.png");

            Image checkBoxImage = Image.GetInstance(checkboxPath);
            var doc1 = new Document(PageSize.LETTER, 80 /* left */, 36 /* right */, 62 /* top */, 0 /* bottom */);
            var ms = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc1, ms);
            Font arial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.NORMAL, BaseColor.BLACK);
            Font arialBold = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.BOLD, BaseColor.BLACK);




            doc1.Open();
            
            var firstTime = true;
            foreach (var surveyResponse in surveyResponses)
            {
                if (!firstTime)
                {
                    doc1.NewPage();
                }
                
                var table = BuildTable(surveyResponse, arial, arialBold, checkBoxImage);
                doc1.Add(table);
                firstTime = false;
            }
            doc1.Close();
            var bytes = ms.ToArray();

            return new FileContentResult(bytes, "application/pdf");
        }

        public virtual FileContentResult PrintPickList(int id, IRepository repository, HttpRequestBase request, UrlHelper url, int[] delimitedList, bool useBackgroundImage = false)
        {
            var survey = repository.OfType<Survey>().GetNullableById(id);
            Check.Require(survey != null);
            


            var checkboxPath = GetAbsoluteUrl(request, url, "~/Images/pdfCheckbox.png");

            Image checkBoxImage = Image.GetInstance(checkboxPath);
            var doc1 = new Document(PageSize.LETTER, 80 /* left */, 36 /* right */, 62 /* top */, 0 /* bottom */);
            var ms = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc1, ms);
            Font arial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.NORMAL, BaseColor.BLACK);
            Font arialBold = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.BOLD, BaseColor.BLACK);

            Image goalImg = null;
            if (useBackgroundImage)
            {
                try
                {
                    //Example to print a background image
                    var goalPath = GetAbsoluteUrl(request, url, string.Format("~/Images/{0}_pdf.jpg", survey.ShortName.Trim().ToUpper()));

                    goalImg = Image.GetInstance(goalPath);
                    goalImg.ScaleToFit(doc1.PageSize.Width, doc1.PageSize.Height);
                    goalImg.SetAbsolutePosition(0, 0);
                    goalImg.Alignment = Image.UNDERLYING;

                }
                catch (Exception)
                {
                    //nothing
                }
            }

            doc1.Open();
            

            var firstTime = true;
            foreach (var surveyResponseId in delimitedList)
            {
                var surveyResponse = repository.OfType<SurveyResponse>().GetNullableById(surveyResponseId);
                if (surveyResponse == null || surveyResponse.Survey.Id != id)
                {
                    continue;
                }
                if (!firstTime)
                {
                    doc1.NewPage();
                }
                if(goalImg != null)
                {
                    doc1.Add(goalImg);
                }
                var table = BuildTable(surveyResponse, arial, arialBold, checkBoxImage);
                doc1.Add(table);
                firstTime = false;
            }
            doc1.Close();
            var bytes = ms.ToArray();

            return new FileContentResult(bytes, "application/pdf");
        }


        private PdfPTable BuildTable(SurveyResponse surveyResponse, Font arial, Font arialBold, Image checkBoxImage)
        {
            var table = new PdfPTable(2);
            //actual width of table in points
            table.TotalWidth = 454f;
            //fix the absolute width of the table
            table.LockedWidth = true;

            table.DefaultCell.Border = 0;
            table.DefaultCell.PaddingTop = 10f;


            

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
            var thankYou = new PdfPCell(new Paragraph(string.Format("Thank you {0} for taking the time to complete the {1} quiz. We hope this feedback will help you and your family make healthy feeding choices.", surveyResponse.StudentId, surveyResponse.Survey.Name), arial));
            thankYou.Colspan = 2;
            thankYou.Border = 0;
            thankYou.PaddingTop = 10f;
            table.AddCell(thankYou);
            //table.AddCell(new Paragraph(string.Format("Thank you {0} for taking the time to complete the {1} quiz. We hope this feedback will help you and your family make healthy feeding choices.", surveyResponse.StudentId, surveyResponse.Survey.Name), arial));

            //2
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);

            //3
            var positiveAffirm = new PdfPCell(new Paragraph(string.Format("{0}{1}", "\n", surveyResponse.PositiveCategory.Affirmation), arialBold));
            positiveAffirm.Colspan = 2;
            positiveAffirm.Border = 0;
            positiveAffirm.PaddingTop = 10f;
            table.AddCell(positiveAffirm);
            //table.AddCell(new Paragraph(string.Format("{0}{1}", "\n", surveyResponse.PositiveCategory.Affirmation), arialBold));

            //4
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);

            //5
            table.AddCell(checkBoxImage);
            table.AddCell(new Paragraph(surveyResponse.NegativeCategory1.Encouragement, arialBold));

            //6
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);

            //7, 8, 9
            for (int i = 0; i < 3; i++)
            {
                table.AddCell(checkBoxImage);
                if (surveyResponse.NegativeCategory1.CategoryGoals.ElementAtOrDefault(i) != null)
                {
                    table.AddCell(new Paragraph(surveyResponse.NegativeCategory1.CategoryGoals[i].Name, arial));
                }
                else
                {
                    table.AddCell(string.Empty);
                }
            }


            //10
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);

            //11
            table.AddCell(checkBoxImage);
            table.AddCell(new Paragraph(surveyResponse.NegativeCategory2.Encouragement, arialBold));

            //12
            table.AddCell(string.Empty);
            table.AddCell(string.Empty);


            //13, 14, 15
            for (int i = 0; i < 3; i++)
            {
                table.AddCell(checkBoxImage);
                if (surveyResponse.NegativeCategory2.CategoryGoals.ElementAtOrDefault(i) != null)
                {
                    table.AddCell(new Paragraph(surveyResponse.NegativeCategory2.CategoryGoals[i].Name, arial));
                }
                else
                {
                    table.AddCell(string.Empty);
                }
            }


            table.GetRow(0).MaxHeights = 70f;
            table.GetRow(1).MaxHeights = 65f;
            table.GetRow(2).MaxHeights = 6f;
            table.GetRow(3).MaxHeights = 50f;
            table.GetRow(4).MaxHeights = 77f;
            table.GetRow(5).MaxHeights = 48f;
            table.GetRow(6).MaxHeights = 32f;
            table.GetRow(7).MaxHeights = 40f;
            table.GetRow(8).MaxHeights = 40f;
            table.GetRow(9).MaxHeights = 40f;
            table.GetRow(10).MaxHeights = 31f;
            table.GetRow(11).MaxHeights = 44f;
            table.GetRow(12).MaxHeights = 38f;
            table.GetRow(13).MaxHeights = 40f;
            table.GetRow(14).MaxHeights = 40f;
            table.GetRow(15).MaxHeights = 40f;
            return table;
        }

        private string GetAbsoluteUrl(HttpRequestBase request, UrlHelper url, string relative)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, url.Content(relative));
        }
    }
}