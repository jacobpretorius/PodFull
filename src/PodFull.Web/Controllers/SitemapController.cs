using System.Linq;
using System.Text;
using System.Web.Mvc;
using PodFull.Web.Business.Helpers;

namespace PodFull.Web.Controllers
{
	public class SitemapController : Controller
	{
		/// <summary>
		///     Default root sitemap.
		/// </summary>
		public ActionResult GetSiteMap()
		{
			var baseUrl = "https://" + HttpContext?.Request?.Url?.Host;

			var result = new StringBuilder();
			result.Append("<?xml version='1.0' encoding='UTF-8'?>");
			result.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

			// Add homepage and blog page
			result.AppendFormat("<url><loc>{0}/</loc></url>", baseUrl);

			var posts = StorageHelper.ReadAllPosts();

			// Only add posts that should be indexed
			foreach (var post in posts.Where(w => w.DontIndexEpisode == false))
			{
				result.AppendFormat("<url><loc>{0}/episode/{1}</loc></url>", baseUrl, post.Id);
			}

			// Add XML feel
			// TODO

			result.AppendFormat("</urlset>");

			return Content(result.ToString(), "text/xml");
		}
	}
}