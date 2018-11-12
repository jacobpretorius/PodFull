using System;
using System.Web.Mvc;
using PodFull.Web.Models.ViewModels.Base;

namespace PodFull.Web.Models.ViewModels
{
	public class EpisodePageViewModel : BasePageViewModel
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public DateTime PostedTime { get; set; }

		[AllowHtml]
		public string BodyHtml { get; set; }

		public string EmbedUrl { get; set; }
	}
}