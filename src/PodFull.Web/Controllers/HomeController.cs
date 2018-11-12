using System.Web.Mvc;
using PodFull.Web.Business.Cache;
using PodFull.Web.Controllers.Base;
using PodFull.Web.Models.ViewModels;

namespace PodFull.Web.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index(HomePageViewModel currentPage, int pager = 0)
		{
			currentPage.EpisodePosts = MemoryCache.GetTenPosts(pager);
			currentPage.Page = pager;

			return View("Home", currentPage);
		}
	}
}