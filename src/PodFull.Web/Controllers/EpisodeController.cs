using System.Web.Mvc;
using PodFull.Web.Business.Cache;
using PodFull.Web.Controllers.Base;
using PodFull.Web.Models.ViewModels;

namespace PodFull.Web.Controllers
{
    public class EpisodeController : BaseController
    {
        public ActionResult Index(EpisodePageViewModel currentPage, int? id = null)
        {
            //we want to allow just /episode being hit, and redirected to home page
            if (id == null || id <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //try get the post from memcache first
            var post = MemoryCache.GetPost((int)id);
            if (post != null)
            {
                currentPage.Id = post.Id;
                currentPage.Title = post.Title;
                currentPage.PostedTime = post.PostedTime;
                currentPage.BodyHtml = post.BodyHtml;        
                currentPage.EmbedUrl = post.EmbedUrl;        

                //replace defaults in base page
                currentPage.DontIndexPage = post.DontIndexEpisode;
                currentPage.WindowTitle = post.Title;
                if (!string.IsNullOrWhiteSpace(post.MetaDescEpisode))
                {
                    currentPage.MetaDescription = post.MetaDescEpisode;
                }

                return View("Episode", currentPage);
            }

            return RedirectToAction("NotFound", "Error");
        }
    }
}