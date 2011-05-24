using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace NuSurvey.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper)
        {
            var captchaControl = new Recaptcha.RecaptchaControl
            {
                ID = "recaptcha",
                Theme = "clean",
                PublicKey = ConfigurationManager.AppSettings["RecaptchaPublicKey"],
                PrivateKey = ConfigurationManager.AppSettings["RecaptchaPrivateKey"]
            };

            var htmlWriter = new HtmlTextWriter(new StringWriter());

            captchaControl.RenderControl(htmlWriter);

            return new MvcHtmlString(htmlWriter.InnerWriter.ToString());
        }
    }
}