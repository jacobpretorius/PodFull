using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using CommonMark;
using PodFull.Web.Business.Helpers;
using PodFull.Web.Controllers.Base;
using PodFull.Web.Models.Data;
using PodFull.Web.Models.ViewModels;

namespace PodFull.Web.Controllers
{
	public class SubmitController : BaseController
	{
		public ActionResult Index(SubmitPageViewModel currentPage)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

			return View("Submit", currentPage);
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		[Authorize]
		public ActionResult Submit(SubmitPageViewModel currentPage)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

			if (ModelState.IsValid)
			{
				var now = DateTime.Now;

				var exists = StorageHelper.ReadPost(currentPage.Id);
				if (exists != null)
				{
					ModelState.AddModelError("Id", $"The episode number {currentPage.Id} is already in use.");
					currentPage.Id = 0;
					return View("Submit", currentPage);
				}

				StorageHelper.SavePost(new Episode
				{
					BodyHtml = CommonMarkConverter.Convert(currentPage.BodyText),
					BodyMarkdown = currentPage.BodyText,
					DontIndexEpisode = currentPage.DontIndexNewPost,
					MetaDescEpisode = currentPage.MetaDescNewPost,
					PostedTime = now,
					Id = currentPage.Id,
					Title = currentPage.Title,
					EmbedUrl = currentPage.EmbedUrl
				});

				return RedirectToAction("Index", "Home", new { currentPage.Id });
			}

			//not valid
			return View("Submit", currentPage);
		}

		[Authorize]
		public ActionResult Edit(EditPostPageViewModel currentPage, int id)
		{
			//auth only
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}			

			//try find the post
			var originalPost = StorageHelper.ReadPost(id, true);
			if (originalPost != null)
			{
				currentPage.Title = originalPost.Title;
				currentPage.PostedTime = originalPost.PostedTime;
				currentPage.BodyMarkdown = originalPost.BodyMarkdown;
				currentPage.Id = originalPost.Id;
				currentPage.EmbedUrl = originalPost.EmbedUrl;
				currentPage.DontIndexPost = originalPost.DontIndexEpisode;
				currentPage.MetaDescPost = originalPost.MetaDescEpisode;

				return View("Edit", currentPage);
			}

			return HttpNotFound();
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		[Authorize]
		public ActionResult SubmitEdit(EditPostPageViewModel currentPage)
		{
			//make sure we can do it
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

			if (ModelState.IsValid)
			{
				//get the original from file with markdown for image parsing
				var originalPost = StorageHelper.ReadPost(currentPage.Id, true);

				if (originalPost != null)
				{
					//first we delete abandoned images
					PostHelper.ProcessRemovedImages(originalPost.BodyMarkdown, currentPage.BodyMarkdown);

					//NOTICE the html markdown switcharoo
					//update with the fields we allow changing
					originalPost.BodyHtml = CommonMarkConverter.Convert(currentPage.BodyMarkdown);
					originalPost.BodyMarkdown = currentPage.BodyMarkdown;
					originalPost.DontIndexEpisode = currentPage.DontIndexPost;
					originalPost.MetaDescEpisode = currentPage.MetaDescPost;
					originalPost.Title = currentPage.Title;
					originalPost.EmbedUrl = currentPage.EmbedUrl;					

					//save the post (will update the cache as well)
					StorageHelper.SavePost(originalPost);

					return RedirectToAction("Index", "Episode", new { id = originalPost.Id });
				}
			}

			//not valid, return
			return View("Edit", currentPage);
		}

		[Authorize]
		public ActionResult DeletePost(int id)
		{
			//should you though?
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

			//read the old post first to remove images
			var originalPost = StorageHelper.ReadPost(id, true);
			if (originalPost != null)
			{
				PostHelper.ProcessRemovedImages(originalPost.BodyMarkdown, "");
			}

			//now delete the post
			if (StorageHelper.DeletePost(id))
			{
				return RedirectToAction("Index", "Home");
			}

			return HttpNotFound();
		}

		[HttpPost]
		public JsonResult Upload()
		{
			if (User.Identity.IsAuthenticated)
			{
				if (Request.Files.Count > 0)
				{
					var file = Request.Files[0];

					if (file != null && file.ContentLength > 0)
					{
						var newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
						var path = Path.Combine(Server.MapPath("~/episode-images/"), newFileName);
						file.SaveAs(path);

						return new JsonResult { Data = newFileName };
					}
				}
			}

			//something went wrong, session probably died
			//NOTE: We are manually returning 401 instead of using [Authorize] which redirects to the login page
			//		which wont work with an api call.
			Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

			return Json(new { ErrorMessage = "You need to be logged in to upload images." }); ;
		}
	}
}