using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.NopStation.CustomSlider.Models
{
    public class SliderModel : BaseNopEntityModel
    {
        public SliderModel()
        {
            Items = new List<SliderItemModel>();
        }

        public string Name { get; set; }

        public bool ShowBackgroundPicture { get; set; }

        public string BackgroundPictureUrl { get; set; }

        public int WidgetZoneId { get; set; }

        public bool Nav { get; set; }

        public bool AutoPlay { get; set; }

        public int AutoPlayTimeout { get; set; }

        public bool AutoPlayHoverPause { get; set; }

        public int StartPosition { get; set; }

        public bool LazyLoad { get; set; }

        public int LazyLoadEager { get; set; }

        public bool Video { get; set; }

        public bool Loop { get; set; }

        public string AnimateOut { get; set; }

        public string AnimateIn { get; set; }

        public bool RTL { get; set; }

        public IList<SliderItemModel> Items { get; set; }

    }
}
