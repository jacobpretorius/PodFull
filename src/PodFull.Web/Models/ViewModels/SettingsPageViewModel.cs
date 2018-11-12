using System.ComponentModel.DataAnnotations;
using PodFull.Web.Models.ViewModels.Base;

namespace PodFull.Web.Models.ViewModels
{
    public class SettingsPageViewModel : BasePageViewModel
    {
        [Required(ErrorMessage = "*Required")]
        public string SiteName { get; set; }
		
        [Required(ErrorMessage = "*Required")]
        public string TagLine { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string DefaultWindowTitle { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string DefaultMetaDescription { get; set; }

		public string RssTitle { get; set; }

		public string RssUrl { get; set; }

		public string iTunesTitle { get; set; }

		public string iTunesUrl { get; set; }
    }
}