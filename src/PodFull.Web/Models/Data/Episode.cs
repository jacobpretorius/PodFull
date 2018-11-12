using System;
using System.Web.Mvc;

namespace PodFull.Web.Models.Data
{
	public class Episode
	{
		public int Id { get; set; }

		public bool DontIndexEpisode { get; set; }

		public string MetaDescEpisode { get; set; }

		public string Title { get; set; }

		public DateTime PostedTime { get; set; }

		[AllowHtml]
		public string BodyHtml { get; set; }

		[AllowHtml]
		public string BodyMarkdown { get; set; }

		public string EmbedUrl { get; set; }
	}
}