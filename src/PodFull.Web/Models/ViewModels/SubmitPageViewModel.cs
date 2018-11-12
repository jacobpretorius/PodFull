using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PodFull.Web.Models.ViewModels.Base;

namespace PodFull.Web.Models.ViewModels
{
    public class SubmitPageViewModel : BasePageViewModel
    {
        [Required(ErrorMessage = "*")]
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Title { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "*")]
        public string BodyText { get; set; }

        public string EmbedUrl { get; set; }

        //SEO
        public bool DontIndexNewPost { get; set; }

        //also shown on the /home view as summary
        [AllowHtml]
        [Required(ErrorMessage = "*")]
        public string MetaDescNewPost { get; set; }
    }
}