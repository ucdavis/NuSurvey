using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NuSurvey.Web.Services
{
    public interface IPrintService
    {
        FileContentResult PrintSingle(int id);
    }

    public class PrintService : IPrintService
    {
        public virtual FileContentResult PrintSingle(int id)
        {
            var rview = new Microsoft.Reporting.WebForms.ReportViewer();
            rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServer"]);

            rview.ServerReport.ReportPath = @"/NuSurvey/Report_SurveyResponse";

            var paramList = new List<Microsoft.Reporting.WebForms.ReportParameter>();

            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("SurveyResponseId", id.ToString()));
            //paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ProposalId", proposalId != null && proposalId.Value > 0 ? proposalId.Value.ToString() : string.Empty));
            //paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ShowComments", showComments.ToString().ToLower()));

            rview.ServerReport.SetParameters(paramList);

            string mimeType, encoding, extension;
            string[] streamids;
            Microsoft.Reporting.WebForms.Warning[] warnings;

            string format = "PDF";

            string deviceInfo = "<DeviceInfo>" +
                                "<SimplePageHeaders>True</SimplePageHeaders>" +
                                "<HumanReadablePDF>True</HumanReadablePDF>" +   // this line disables the compression done by SSRS 2008 so that it can be merged.
                                "</DeviceInfo>";

            byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

            return new FileContentResult(bytes, "application/pdf");// File(bytes, "application/pdf");
        }
    }
}