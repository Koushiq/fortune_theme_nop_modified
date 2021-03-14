using Nop.Web.Framework.Models;

namespace Nop.Plugin.NopStation.AnywhereSlider.Models
{
    public class SliderItemModel : BaseNopEntityModel
    {
        public string Title { get; set; }

        public string Link { get; set; }

        public string PictureUrl { get; set; }

        public string ImageAltText { get; set; }

        public string Url { get; set; }

        public string ShortDescription { get; set; }
    }
}
