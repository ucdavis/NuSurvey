using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Helpers;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;

namespace NuSurvey.Web.Controllers
{
    [ServiceMessage("NuSurvey", ViewDataKey = "ServiceMessages", MessageServiceAppSettingsKey = "MessageServer")]
    [LocVersion]
    public class ApplicationController : SuperController { }


}
