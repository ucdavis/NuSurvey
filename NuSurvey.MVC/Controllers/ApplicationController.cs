using NuSurvey.MVC.Controllers.Filters;
using NuSurvey.MVC.Helpers;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;

namespace NuSurvey.MVC.Controllers
{
    [ServiceMessage("NuSurvey", ViewDataKey = "ServiceMessages", MessageServiceAppSettingsKey = "MessageServer")]
    [LocVersion]
    public class ApplicationController : SuperController { }


}
