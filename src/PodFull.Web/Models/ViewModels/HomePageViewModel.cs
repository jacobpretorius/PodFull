using System.Collections.Generic;
using PodFull.Web.Models.Data;
using PodFull.Web.Models.ViewModels.Base;

namespace PodFull.Web.Models.ViewModels
{
    public class HomePageViewModel : BasePageViewModel
    {
        public IEnumerable<Episode> EpisodePosts { get; set; }

        public int Page { get; set; }        
    }
}