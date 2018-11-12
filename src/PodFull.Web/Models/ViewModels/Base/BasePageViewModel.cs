namespace PodFull.Web.Models.ViewModels.Base
{
    public class BasePageViewModel
    {
        public string SiteName { get; set; }

        public string TagLine { get; set; }

        public string WindowTitle { get; set; }

        public string MetaDescription { get; set; }

        public bool DontIndexPage { get; set; }

		public string RssTitle { get; set; }

        public string RssUrl { get; set; }

		public string iTunesTitle { get; set; }

		public string iTunesUrl { get; set; }
    }
}