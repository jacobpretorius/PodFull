using System.Web.Mvc;
using PodFull.Web.Controllers.Base;
using PodFull.Web.Models.ViewModels.Base;

namespace PodFull.Web.Controllers
{
	public class ErrorController : BaseController
	{
		public ActionResult NotFound(BasePageViewModel currentPage)
        {
            //TODO log something here

			//strip the awful error path
	        if (Request["aspxerrorpath"] != null)
	        {
		        return RedirectToAction("NotFound");
	        }

            return View("404", currentPage);
        }
	}
}