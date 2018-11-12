using System.Web.Mvc;
using System.Web.Routing;

namespace PodFull.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.LowercaseUrls = true;

			routes.MapRoute(
				"Episode",
				"episode/{id}",
				new { controller = "Episode", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				"homePaging",
				"episodes/{pager}",
				new { controller = "Home", action = "Index" }
			);

			routes.MapRoute(
				"Sitemap",
				"sitemap.xml",
				new { controller = "Sitemap", action = "GetSiteMap" }
			);

			routes.MapRoute(
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}