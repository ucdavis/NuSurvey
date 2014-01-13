using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;

namespace NuSurvey.Web.Services
{
    public interface IEmailService
    {
        void SendPasswordReset(string userName, string tempPass);
        void SendNewUser(HttpRequestBase request, UrlHelper url, string userName, string tempPass);
    }

    public class EmailService : IEmailService
    {
        public void SendPasswordReset(string userName, string tempPass)
        {
            var mail = new MailMessage("automatedemail@caes.ucdavis.edu", userName, "Nutrition Survey Password Reset", "");
            mail.IsBodyHtml = true;
            mail.Body = string.Format("Your Password has been reset to {0}<br />{1}<br />{2}", tempPass, "We recommend changing your password once you have logged on." ,"Please do not reply to this email. It was automatically generated."); 
            var client = new SmtpClient();
            client.Credentials = new NetworkCredential(CloudConfigurationManager.GetSetting("SmtpAccount"), CloudConfigurationManager.GetSetting("SmtpPassword"));
            client.Send(mail);
        }

        public void SendNewUser(HttpRequestBase request, UrlHelper url, string userName, string tempPass)
        {
            var mail = new MailMessage("automatedemail@caes.ucdavis.edu", userName, "Nutrition Survey Account Created", "");
            mail.IsBodyHtml = true;
            mail.Body = string.Format("{0}<br />{1} {2}<br />{3} {4}<br />{5} {6}<br />{7}<br />{8}",
                                      "An account has been created for you for the Nutrition Survey.",
                                      "You can access it by logging in here: ",
                                      GetAbsoluteUrl(request, url, "~/Account/LogOn"),
                                      "With your email address: ",
                                      userName,
                                      "And this password: ",
                                      tempPass,
                                      "We recommend changing your password once you have logged on.",
                                      "Please do not reply to this email. It was automatically generated.");

            var client = new SmtpClient();
            client.Credentials = new NetworkCredential(CloudConfigurationManager.GetSetting("SmtpAccount"), CloudConfigurationManager.GetSetting("SmtpPassword"));
            client.Send(mail);

        }

        private string GetAbsoluteUrl(HttpRequestBase request, UrlHelper url, string relative)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Host, url.Content(relative));
        }
    }
}