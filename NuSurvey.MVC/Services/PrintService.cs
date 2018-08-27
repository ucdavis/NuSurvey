using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using NuSurvey.MVC.Resources;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NuSurvey.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.Linq;
using System.Linq.Expressions;
using NuSurvey.MVC.Helpers;

namespace NuSurvey.MVC.Services
{
    public interface IPrintService
    {
        FileContentResult PrintSingle(int id, IRepository repository, HttpRequestBase request, UrlHelper url, bool useBackgroundImage = false, SurveyResponse publicSurveyResponse = null);
        FileContentResult PrintMultiple(int id, IRepository repository, HttpRequestBase request, UrlHelper url, DateTime? beginDate, DateTime? endDate);
        FileContentResult PrintPickList(int id, IRepository repository, HttpRequestBase request, UrlHelper url, int[] surveyResponseIds, bool useBackgroundImage = false);

        FileContentResult PrintDirector(PrintedSurvey printedSurvey, HttpRequestBase request, UrlHelper url);
    }

    public class PrintService : IPrintService
    {
        private readonly IBlobStoargeService _blobStoargeService;

        public PrintService(IBlobStoargeService blobStoargeService)
        {
            _blobStoargeService = blobStoargeService;
        }

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

        public FileContentResult PrintDirector2(PrintedSurvey printedSurvey)
        {
            Check.Require(printedSurvey != null);


            var doc = new Document(PageSize.LETTER, 80 /* left */, 36 /* right */, 62 /* top */, 0 /* bottom */);
            var ms = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc, ms);
            Font arial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.NORMAL, BaseColor.BLACK);
            Font arialBold = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 12, Font.BOLD, BaseColor.BLACK);

            doc.Open();

            var table = new PdfPTable(2);
            table.TotalWidth = 454f;
            table.LockedWidth = true;
            var widths = new[] { 33f, 67f };
            table.SetWidths(widths);

            var questionCounter = 0;
            foreach (var psq in printedSurvey.PrintedSurveyQuestions)
            {
                questionCounter++;



                Image FakeImage = null;
                if(questionCounter % 2 == 0)
                {
                    FakeImage = new Jpeg(_blobStoargeService.GetPhoto(22, Resource.Thumb));
                }
                else
                {
                    FakeImage = new Jpeg(_blobStoargeService.GetPhoto(23, Resource.Original));
                }

                table.AddCell(FakeImage);
                var cell = new PdfPCell(new Paragraph(psq.Question.Name + "\n\n" + "Some radio" ));
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);


                if (questionCounter % 5 == 0)
                {
                    doc.Add(table);
                    doc.NewPage();
                    table = new PdfPTable(2);
                    table.TotalWidth = 454f;
                    table.LockedWidth = true;
                    table.SetWidths(widths);

                    //TODO: Background image
                }

                if (psq.Photo != null)
                {
                    //TODO: Non faked photos
                }
            }
            if (questionCounter % 5 != 0)
            {
                doc.Add(table);
            }
            doc.Close();
            var bytes = ms.ToArray();

            return new FileContentResult(bytes, "application/pdf");
        }


        public FileContentResult PrintDirector(PrintedSurvey printedSurvey, HttpRequestBase request, UrlHelper url)
        {
            Check.Require(printedSurvey != null);

            var pdfUnderPath = GetAbsoluteUrl(request, url, string.Format("~/Content/{0}-director-print.pdf", printedSurvey.Survey.ShortName.Trim().ToUpper()));
            var readerUnder = new PdfReader(pdfUnderPath);

            var doc = new Document(PageSize.LETTER, 80 /* left */, 36 /* right */, 62 /* top */, 0 /* bottom */);
            doc.SetPageSize(readerUnder.GetPageSize(1));
            var leftMargin = 58;
            if (printedSurvey.Survey.ShortName.Trim().ToUpper() == "HK19")
            {
                leftMargin = 36;
            }
            doc.SetMargins(leftMargin, 0, 0, 0);


            var ms = new MemoryStream();
            var writer = PdfWriter.GetInstance(doc, ms);

            doc.Open();

            var questions = printedSurvey.PrintedSurveyQuestions.OrderBy(a => a.Order).ToArray();

            switch (printedSurvey.Survey.ShortName.Trim().ToUpper())
            {
                case "HK":
                    ProcessHkPage1(doc, questions, request, url);
                    ProcessHkPage2(doc, questions, request, url);
                    ProcessHkMiddlePages(doc, questions, 9, request, url);
                    ProcessHkMiddlePages(doc, questions, 14, request, url);
                    ProcessHkMiddlePages(doc, questions, 19, request, url);
                    ProcessHkMiddlePages(doc, questions, 24, request, url);
                    ProcessHkMiddlePages(doc, questions, 29, request, url);
                    ProcessHkMiddlePages(doc, questions, 34, request, url);
                    ProcessHkMiddlePages(doc, questions, 39, request, url);
                    ProcessHkLastPage(doc, questions, request, url);
                    break;
                case "HK19":
                    ProcessHk19Page1(doc, questions, request, url);
                    ProcessHk19Page2(doc, questions, request, url);
                    break;
                case "MCMT":
                    ProcessMCMTPage1(doc, questions, request, url);
                    ProcessMCMTMiddlePages(doc, questions, 4, request, url);
                    ProcessMCMTMiddlePages(doc, questions, 9, request, url);
                    ProcessMCMTMiddlePages(doc, questions, 14, request, url);
                    ProcessMCMTMiddlePages(doc, questions, 19, request, url);
                    ProcessMCMTLastPage(doc, questions, request, url);
                    break;
            }


            doc.Close();

            var someBytes = ms.ToArray();

            var readerOver = new PdfReader(someBytes);
            
            //var reader = new PdfReader(ms.ToArray());
            

            var ms2 = new MemoryStream();
            var stamper = new PdfStamper(readerOver, ms2);



            int n = readerOver.NumberOfPages;
            PdfContentByte background;
            for (int i = 1; i <= n; i++)
            {
                PdfImportedPage page = stamper.GetImportedPage(readerUnder, i);
                background = stamper.GetUnderContent(i);
                background.AddTemplate(page, 0, 0);
            }
            // CLose the stamper
            stamper.Close();

            var bytes = ms2.ToArray();

            return new FileContentResult(bytes, "application/pdf");


        }

        #region MCMT Pages

        private void ProcessMCMTPage1(Document doc, PrintedSurveyQuestion[] questions, HttpRequestBase request, UrlHelper url)
        {
            var table = new PdfPTable(1);
            table.TotalWidth = 219f;

            table.LockedWidth = true;

            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.DefaultCell.Border = 0;
            table.DefaultCell.Padding = 0;
            table.DefaultCell.PaddingBottom = 13.5f;


            for (int i = 0; i < 4; i++)
            {
                var psq = questions[i];
                Image selectedImage = null;
                if (psq.Photo != null)
                {
                    try
                    {
                        selectedImage = new Jpeg(_blobStoargeService.GetPhoto(psq.Photo.Id, Resource.Original));
                        //selectedImage = new Jpeg(_blobStoargeService.GetPhoto(10, Resource.Original));
                    }
                    catch (Exception)
                    {
                        selectedImage = null;
                    }
                    
                }

                if (i == 0)
                {
                    table.DefaultCell.PaddingTop = 215.5f;
                    table.DefaultCell.PaddingBottom = 32.5f;
                }
                else
                {
                    table.DefaultCell.PaddingTop = 0;
                    table.DefaultCell.PaddingBottom = 35f;
                }

                if (i == 2)
                {
                    table.DefaultCell.PaddingTop = 1.5f;
                }

                if (i == 3)
                {
                    table.DefaultCell.PaddingTop = 0.5f;
                    table.DefaultCell.PaddingBottom = 0;
                }

                if (selectedImage == null)
                {
                    selectedImage = Image.GetInstance(GetAbsoluteUrl(request, url, "~/Images/NoImage.jpg"));
                }
                table.AddCell(selectedImage);

            }

            doc.Add(table);
            doc.NewPage();

        }

        private void ProcessMCMTMiddlePages(Document doc, PrintedSurveyQuestion[] questions, int firstQuestionOnPage, HttpRequestBase request, UrlHelper url)
        {
            var table = new PdfPTable(1);
            table.TotalWidth = 219f;
            table.LockedWidth = true;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.DefaultCell.Border = 0;
            table.DefaultCell.Padding = 0;
            table.DefaultCell.PaddingBottom = 13.5f;


            for (int i = 0; i < 5; i++)
            {
                var psq = questions[i + firstQuestionOnPage];
                Image selectedImage = null;
                if (psq.Photo != null)
                {
                    try
                    {
                        selectedImage = new Jpeg(_blobStoargeService.GetPhoto(psq.Photo.Id, Resource.Original));
                        //selectedImage = new Jpeg(_blobStoargeService.GetPhoto(10, Resource.Original));
                    }
                    catch (Exception)
                    {
                        selectedImage = null;
                    }
                    
                }

                if (i == 0)
                {
                    table.DefaultCell.PaddingTop = 63;
                    table.DefaultCell.PaddingBottom = 35f;
                }
                else
                {
                    table.DefaultCell.PaddingTop = 0;
                    table.DefaultCell.PaddingBottom = 34.5f;
                }

                if (i == 3)
                {
                    table.DefaultCell.PaddingTop = 2;
                    table.DefaultCell.PaddingBottom = 34.5f;
                }

                if (i == 4)
                {
                    table.DefaultCell.PaddingTop = 1.5f;
                    table.DefaultCell.PaddingBottom = 0;
                }
                if (selectedImage == null)
                {
                    selectedImage = Image.GetInstance(GetAbsoluteUrl(request, url, "~/Images/NoImage.jpg"));
                }
                table.AddCell(selectedImage);

            }

            doc.Add(table);
            doc.NewPage();
        }


        private void ProcessMCMTLastPage(Document doc, PrintedSurveyQuestion[] questions, HttpRequestBase request, UrlHelper url)
        {
            var table = new PdfPTable(1);
            table.TotalWidth = 219f;
            table.LockedWidth = true;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.DefaultCell.Border = 0;
            table.DefaultCell.Padding = 0;
            table.DefaultCell.PaddingBottom = 13.5f;


            for (int i = 0; i < 3; i++)
            {
                var psq = questions[i + 24];
                Image selectedImage = null;
                if (psq.Photo != null)
                {
                    try
                    {
                        selectedImage = new Jpeg(_blobStoargeService.GetPhoto(psq.Photo.Id, Resource.Original));
                    }
                    catch (Exception)
                    {
                        selectedImage = null;
                    }
                    
                }


                if (i == 0)
                {
                    table.DefaultCell.PaddingTop = 63;
                    table.DefaultCell.PaddingBottom = 35f;
                }
                else
                {
                    table.DefaultCell.PaddingTop = 0;
                    table.DefaultCell.PaddingBottom = 34.5f;
                }

                if (selectedImage == null)
                {
                    selectedImage = Image.GetInstance(GetAbsoluteUrl(request, url, "~/Images/NoImage.jpg"));
                }
                table.AddCell(selectedImage);

            }

            doc.Add(table);
            doc.NewPage();
        }
        #endregion MCMT Pages

        #region HK19 Pages
        
        private void ProcessHk19Page1(Document doc, PrintedSurveyQuestion[] questions, HttpRequestBase request, UrlHelper url)
        {
            var psq = questions[0];
            Image selectedImage = SelectedImage(request, url, psq);
            selectedImage.SetAbsolutePosition(45.0f, 444.2f); //(45.0f, 444.2f); 
            doc.Add(selectedImage);

            psq = questions[1];
            selectedImage = SelectedImage(request, url, psq);
            selectedImage.SetAbsolutePosition(324.0f, 444.2f); //(324.0f, 444.2f); 
            doc.Add(selectedImage);

            psq = questions[2];
            selectedImage = SelectedImage(request, url, psq);
            selectedImage.SetAbsolutePosition(45.0f, 166.4f); 
            doc.Add(selectedImage);

            psq = questions[3];
            selectedImage = SelectedImage(request, url, psq);
            selectedImage.SetAbsolutePosition(324.0f, 166.4f); 
            doc.Add(selectedImage);

            doc.NewPage();

        }

        private void ProcessHk19Page2(Document doc, PrintedSurveyQuestion[] questions, HttpRequestBase request, UrlHelper url)
        {
            var psq = questions[4];
            Image selectedImage = SelectedImage(request, url, psq);
            selectedImage.SetAbsolutePosition(45.0f, 626.6f); 
            doc.Add(selectedImage);

            psq = questions[5];
            selectedImage = SelectedImage(request, url, psq);
            selectedImage.SetAbsolutePosition(324.0f, 626.6f); 
            doc.Add(selectedImage);

            psq = questions[6];
            selectedImage = SelectedImage(request, url, psq);
            selectedImage.SetAbsolutePosition(45.0f, 349.2f);
            doc.Add(selectedImage);

            psq = questions[7];
            selectedImage = SelectedImage(request, url, psq);
            selectedImage.SetAbsolutePosition(324.0f, 349.2f);
            doc.Add(selectedImage);

            psq = questions[8];
            selectedImage = SelectedImage(request, url, psq);
            selectedImage.ScaleAbsoluteWidth(294.8f); //Override size that other images are using to 295
            selectedImage.ScaleAbsoluteHeight(155.6f); // and 155.6
            selectedImage.SetAbsolutePosition(45.0f, 55.3f);
            doc.Add(selectedImage);

            doc.NewPage();

        }

        //Just for the HK19 code as I have to do this for every question
        private Image SelectedImage(HttpRequestBase request, UrlHelper url, PrintedSurveyQuestion psq)
        {
            Image selectedImage = null;
            if (psq.Photo != null)
            {
                try
                {
                    selectedImage = new Jpeg(_blobStoargeService.GetPhoto(psq.Photo.Id, Resource.Original));
                }
                catch (Exception)
                {
                    selectedImage = null;
                }
            }

            if (selectedImage == null)
            {
                selectedImage = Image.GetInstance(GetAbsoluteUrl(request, url, "~/Images/NoImage.jpg"));
            }

            const float theWidth = 261.0f; //261.2f;
            const float theHeight = 137.8f;
            //var theHeight = (selectedImage.Height * theWidth) / selectedImage.Width;

            selectedImage.ScaleAbsoluteWidth(theWidth);
            selectedImage.ScaleAbsoluteHeight(theHeight);

            return selectedImage;
        }

        #endregion HK19 Pages

        #region HK Pages

        private void ProcessHkPage1(Document doc, PrintedSurveyQuestion[] questions, HttpRequestBase request, UrlHelper url)
        {
            var table = new PdfPTable(1);
            table.TotalWidth = 219f;

            table.LockedWidth = true;
 
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.DefaultCell.Border = 0;
            table.DefaultCell.Padding = 0;
            table.DefaultCell.PaddingBottom = 13.5f;


            for (int i = 0; i < 4; i++)
            {
                var psq = questions[i];
                Image selectedImage = null;
                if(psq.Photo != null)
                {
                    try
                    {
                        selectedImage = new Jpeg(_blobStoargeService.GetPhoto(psq.Photo.Id, Resource.Original));
                    }
                    catch (Exception)
                    {
                        selectedImage = null;
                    }
                    
                }
                //Image FakeImage = null;
                //selectedImage = new Jpeg(_blobStoargeService.GetPhoto(10, Resource.Original));

                if (i == 0)
                {
                    table.DefaultCell.PaddingTop = 215.5f;
                    table.DefaultCell.PaddingBottom = 32.5f;
                }
                else
                {
                    table.DefaultCell.PaddingTop = 0;
                    table.DefaultCell.PaddingBottom = 35f;
                }

                if (i == 2)
                {
                    table.DefaultCell.PaddingTop = 1.5f;
                }

                if (i == 3)
                {
                    table.DefaultCell.PaddingTop = 0.5f;
                    table.DefaultCell.PaddingBottom = 0;
                }

                if (selectedImage == null)
                {
                    selectedImage = Image.GetInstance(GetAbsoluteUrl(request, url, "~/Images/NoImage.jpg"));
                }
                table.AddCell(selectedImage);

            }

            doc.Add(table);
            doc.NewPage();

        }

        private void ProcessHkPage2(Document doc, PrintedSurveyQuestion[] questions, HttpRequestBase request, UrlHelper url)
        {
            var table = new PdfPTable(1);
            table.TotalWidth = 219f;
            table.LockedWidth = true;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.DefaultCell.Border = 0;
            table.DefaultCell.Padding = 0;
            table.DefaultCell.PaddingBottom = 13.5f;


            for (int i = 0; i < 4; i++)
            {
                var psq = questions[i+4];
                Image selectedImage = null;
                if (psq.Photo != null)
                {
                    try
                    {
                        selectedImage = new Jpeg(_blobStoargeService.GetPhoto(psq.Photo.Id, Resource.Original));
                    }
                    catch (Exception)
                    {
                        selectedImage = null;
                    }
                    
                }
                //Image FakeImage = null;
                //selectedImage = new Jpeg(_blobStoargeService.GetPhoto(10, Resource.Original));

                if (i == 0)
                {
                    table.DefaultCell.PaddingTop = 63;
                    table.DefaultCell.PaddingBottom = 35f;
                }
                else
                {
                    table.DefaultCell.PaddingTop = 0;
                    table.DefaultCell.PaddingBottom = 34.5f;
                }

                if (i == 3)
                {
                    table.DefaultCell.PaddingTop = 2;
                    table.DefaultCell.PaddingBottom = 0;
                }
                if (selectedImage == null)
                {
                    selectedImage = Image.GetInstance(GetAbsoluteUrl(request, url, "~/Images/NoImage.jpg"));
                }
                table.AddCell(selectedImage);

            }

            doc.Add(table);
            doc.NewPage();
        }

        private void ProcessHkMiddlePages(Document doc, PrintedSurveyQuestion[] questions, int firstQuestionOnPage, HttpRequestBase request, UrlHelper url)
        {
            var table = new PdfPTable(1);
            table.TotalWidth = 219f;
            table.LockedWidth = true;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.DefaultCell.Border = 0;
            table.DefaultCell.Padding = 0;
            table.DefaultCell.PaddingBottom = 13.5f;


            for (int i = 0; i < 5; i++)
            {
                var psq = questions[i + firstQuestionOnPage];
                Image selectedImage = null;
                if (psq.Photo != null)
                {
                    try
                    {
                        selectedImage = new Jpeg(_blobStoargeService.GetPhoto(psq.Photo.Id, Resource.Original));
                    }
                    catch (Exception)
                    {
                        selectedImage = null;
                    }
                    
                }
                //Image FakeImage = null;
                //selectedImage = new Jpeg(_blobStoargeService.GetPhoto(10, Resource.Original));

                if (i == 0)
                {
                    table.DefaultCell.PaddingTop = 63;
                    table.DefaultCell.PaddingBottom = 35f;
                }
                else
                {
                    table.DefaultCell.PaddingTop = 0;
                    table.DefaultCell.PaddingBottom = 34.5f;
                }

                if (i == 3)
                {
                    table.DefaultCell.PaddingTop = 2;
                    table.DefaultCell.PaddingBottom = 34.5f;
                }

                if (i == 4)
                {
                    table.DefaultCell.PaddingTop = 1.5f;
                    table.DefaultCell.PaddingBottom = 0;
                }
                if (selectedImage == null)
                {
                    selectedImage = Image.GetInstance(GetAbsoluteUrl(request, url, "~/Images/NoImage.jpg"));
                }
                table.AddCell(selectedImage);

            }

            doc.Add(table);
            doc.NewPage();
        }

        private void ProcessHkLastPage(Document doc, PrintedSurveyQuestion[] questions, HttpRequestBase request, UrlHelper url)
        {
            var table = new PdfPTable(1);
            table.TotalWidth = 219f;
            table.LockedWidth = true;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.DefaultCell.Border = 0;
            table.DefaultCell.Padding = 0;
            table.DefaultCell.PaddingBottom = 13.5f;

            var psq = questions[44];
            Image selectedImage = null;
            if (psq.Photo != null)
            {
                try
                {
                    selectedImage = new Jpeg(_blobStoargeService.GetPhoto(psq.Photo.Id, Resource.Original));
                }
                catch (Exception)
                {
                    selectedImage = null;
                }
                
            }

            //Image FakeImage = null;
            //selectedImage = new Jpeg(_blobStoargeService.GetPhoto(10, Resource.Original));

            table.DefaultCell.PaddingTop = 63;

            if (selectedImage == null)
            {
                selectedImage = Image.GetInstance(GetAbsoluteUrl(request, url, "~/Images/NoImage.jpg"));
            }
            table.AddCell(selectedImage);


            doc.Add(table);
            doc.NewPage();
        }
        #endregion HK Pages

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
            if (surveyResponse.Survey.ShortName.IsSpanish())
            {
                thankYou = new PdfPCell(new Paragraph(string.Format("Gracias {0} por tomar el tiempo para completar el examen de {1}. Esperemos que estas recomendaciones le ayude a usted y a su familia elegir alimentos y actividades sanos.", surveyResponse.StudentId, surveyResponse.Survey.Name), arial));
            }
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