using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using System.Xml.Linq;
using PodFull.Web.Business.Cache;
using PodFull.Web.Models.Data;

namespace PodFull.Web.Business.Helpers
{
	public static class StorageHelper
	{
		private static readonly string _folder = HostingEnvironment.MapPath("~/episodes/");

		static StorageHelper()
		{
			if (!Directory.Exists(_folder))
			{
				Directory.CreateDirectory(_folder);
			}

			//also ensure the images folder is there
			var imageFolder = HostingEnvironment.MapPath("~/episode-images/");
			if (!Directory.Exists(imageFolder))
			{
				Directory.CreateDirectory(imageFolder);
			}
		}

		/// <summary>
		/// Save a new post to file. Replaces existing Posts.
		/// </summary>
		/// <param name="post">The post to save</param>
		/// <returns>The bool result of the save operation</returns>
		public static bool SavePost(Episode post)
		{
			try
			{
				var filePath = Path.Combine(_folder + post.Id + ".webinfo");

				var savePost = new XDocument(
					new XElement("post",
						new XElement("title", post.Title),
						new XElement("id", post.Id),
						new XElement("published_date", post.PostedTime.ToString("yyyy-MM-dd HH:mm:ss")),
						new XElement("excerpt", post.MetaDescEpisode),
						new XElement("content", post.BodyHtml),
						new XElement("markdown_content", post.BodyMarkdown),
						new XElement("dont_index", post.DontIndexEpisode.ToString()),
						new XElement("embed_url", post.EmbedUrl)
					));

				//save to file
				savePost.Save(filePath);

				//add to cache (or update if exists)
				MemoryCache.AddPost(post);

				return true;
			}
			catch
			{
				//dont care really, will go into below
			}

			return false;
		}

		/// <summary>
		/// Delete a specific post
		/// </summary>
		/// <param name="year">The year of the post</param>
		/// <param name="slug">The slug of the post</param>
		/// <returns></returns>
		public static bool DeletePost(int id)
		{
			try
			{
				var file = Path.Combine(_folder + id + ".webinfo");
				if (File.Exists(file))
				{
					//delete the file
					File.Delete(file);
				}

				//remove from cache
				MemoryCache.RemovePost(id);

				//all good
				return true;
			}
			catch
			{
				//todo log it maybe
			}

			return false;
		}

		/// <summary>
		/// Get a specific slug Post from disk if available.
		/// </summary>
		/// <param name="year">The year to search in</param>
		/// <param name="slug">The slug to search for</param>
		/// <param name="includeMarkdown">Include the plain markdown in the object</param>
		/// <returns>The Post, or Null if none found.</returns>
		public static Episode ReadPost(int id, bool includeMarkdown = false)
		{
			try
			{
				var file = XElement.Load(Path.Combine(_folder + id + ".webinfo"));
				if (file.HasElements)
				{
					return new Episode
					{
						Title = XmlHelper.ReadValue(file, "title"),
						Id = id,
						PostedTime = DateTime.Parse(XmlHelper.ReadValue(file, "published_date")),
						MetaDescEpisode = XmlHelper.ReadValue(file, "excerpt"),
						BodyHtml = XmlHelper.ReadValue(file, "content"),
						DontIndexEpisode = XmlHelper.ReadBool(file, "dont_index"),
						EmbedUrl = XmlHelper.ReadValue(file, "embed_url"),

						//now we actually get it
						BodyMarkdown = includeMarkdown ? XmlHelper.ReadValue(file, "markdown_content") : null
					};
				}
			}
			catch
			{
				//dont actually care, this happens when a post couldnt be read aka 404
			}

			return null;
		}

		/// <summary>
		/// Reads all posts
		/// </summary>
		/// <returns>An ordered list of all Posts</returns>
		public static List<Episode> ReadAllPosts()
		{
			var list = new List<Episode>();

			// Can this be done in parallel to speed it up?
			foreach (var file in Directory.EnumerateFiles(_folder, "*.webinfo", SearchOption.AllDirectories))
			{
				var doc = XElement.Load(file);
				var post = new Episode
				{
					Title = XmlHelper.ReadValue(doc, "title"),
					Id = XmlHelper.ReadInt(doc, "id"),
					PostedTime = DateTime.Parse(XmlHelper.ReadValue(doc, "published_date")),
					MetaDescEpisode = XmlHelper.ReadValue(doc, "excerpt"),
					BodyHtml = XmlHelper.ReadValue(doc, "content"),
					DontIndexEpisode = XmlHelper.ReadBool(doc, "dont_index"),
					EmbedUrl = XmlHelper.ReadValue(doc, "embed_url"),

					//we dont care for having this in the cache, as edit post reads from disk regardless
					BodyMarkdown = null
				};

				list.Add(post);
			}

			if (list.Count > 0)
			{
				list.Sort((p1, p2) => p2.Id.CompareTo(p1.Id));
			}

			return list;
		}

		#region Settings

		/// <summary>
		/// Read the setting file on disk and return the parsed object
		/// </summary>
		/// <returns>The settings object</returns>
		public static Settings ReadSettings()
		{
			try
			{
				var file = XElement.Load(Path.Combine(_folder + "settings.config"));
				if (file.HasElements)
				{
					return new Settings
					{
						SiteName = XmlHelper.ReadValue(file, "site_name"),
						TagLine = XmlHelper.ReadValue(file, "tag_line"),
						DefaultWindowTitle = XmlHelper.ReadValue(file, "window_title"),
						DefaultMetaDescription = XmlHelper.ReadValue(file, "post_meta"),
						RssTitle = XmlHelper.ReadValue(file, "rss_title"),
						RssUrl = XmlHelper.ReadValue(file, "rss_url"),
						iTunesTitle = XmlHelper.ReadValue(file, "itunes_title"),
						iTunesUrl = XmlHelper.ReadValue(file, "itunes_url")
					};
				}
			}
			catch
			{
				//something wrong with the settings file, recreate it
				SaveSettings();
			}

			return null;
		}

		/// <summary>
		/// Save a settings object to disk and update the settings cache. An empty object means create default settings file.
		/// </summary>
		/// <param name="settings">The settings object to save</param>
		public static void SaveSettings(Settings settings = null)
		{
			//check if we got the obj
			if (settings == null)
			{
				settings = new Settings
				{
					SiteName = "PodFull",
					TagLine = "Built for listening",
					DefaultMetaDescription = "Powered by https://github.com/jacobpretorius/PodFull",
					DefaultWindowTitle = $"PodFull | {ConfigurationManager.AppSettings["site-url"]}",
					RssTitle = "RSS FEED",
					RssUrl = "https://jcpretorius.com",
					iTunesTitle = "Listen on iTunes",
					iTunesUrl = "https://jcpretorius.com"
				};
			}

			//save it
			try
			{
				var filePath = Path.Combine(_folder + "settings.config");

				var saveSettings = new XDocument(
					new XElement("settings",
						new XElement("site_name", settings.SiteName),
						new XElement("tag_line", settings.TagLine),
						new XElement("window_title", settings.DefaultWindowTitle),
						new XElement("post_meta", settings.DefaultMetaDescription),
						new XElement("rss_title", settings.RssTitle),
						new XElement("rss_url", settings.RssUrl),
						new XElement("itunes_title", settings.iTunesTitle),
						new XElement("itunes_url", settings.iTunesUrl)
					));

				//save to file
				saveSettings.Save(filePath);

				//add to cache (or update if exists)
				SettingsCache.SetSettings(settings);
			}
			catch
			{
				//todo log it
			}
		}

		#endregion Settings

		#region image

		/// <summary>
		/// Delete an image file from post-images on disk
		/// </summary>
		/// <param name="filename">the full filename to delete</param>
		public static void DeleteImage(string filename)
		{
			try
			{
				var file = Path.Combine(HostingEnvironment.MapPath("~/episode-images/") + filename);
				if (File.Exists(file))
				{
					//delete the file
					File.Delete(file);
				}
			}
			catch
			{
				//todo log it maybe
			}
		}

		#endregion image
	}
}