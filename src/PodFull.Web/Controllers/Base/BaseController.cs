using System.Linq;
using System.Web.Mvc;
using PodFull.Web.Business.Cache;
using PodFull.Web.Models.ViewModels.Base;

namespace PodFull.Web.Controllers.Base
{
    public class BaseController : Controller
    {
        public BasePageViewModel Model { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            Model = (BasePageViewModel) context.ActionParameters?.FirstOrDefault(a => a.Value is BasePageViewModel).Value;

            //only continue if we have a model
            if (Model != null)
            {
                //load settings from cache
                var settings = SettingsCache.GetSettings();

                //set defaults
                Model.SiteName = settings.SiteName;
                Model.TagLine = settings.TagLine;
                Model.WindowTitle = settings.DefaultWindowTitle;
                Model.MetaDescription = settings.DefaultMetaDescription;

				Model.RssTitle = settings.RssTitle;
				Model.RssUrl = settings.RssUrl;
				Model.iTunesTitle = settings.iTunesTitle;
				Model.iTunesUrl = settings.iTunesUrl;
            }
        }
    }
}