using System.ComponentModel.DataAnnotations;
using PodFull.Web.Models.ViewModels.Base;

namespace PodFull.Web.Models.ViewModels
{
    public class LoginPageViewModel : BasePageViewModel
    {
        [Required(ErrorMessage = "*Required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "*Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}