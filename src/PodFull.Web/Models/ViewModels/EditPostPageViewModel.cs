using System;
using System.Web.Mvc;
using PodFull.Web.Models.ViewModels.Base;

namespace PodFull.Web.Models.ViewModels
{
	public class EditPostPageViewModel : BasePageViewModel
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public DateTime PostedTime { get; set; }

		[AllowHtml]
		public string BodyMarkdown { get; set; }

		public string EmbedUrl { get; set; }

		public bool DontIndexPost { get; set; }

		public string MetaDescPost { get; set; }
	}
}