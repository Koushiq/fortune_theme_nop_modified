using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Plugin.NopStation.CategoryBanners.Models
{
    public class CategoryBannerModel : BaseNopEntityModel
    {
        public CategoryBannerModel()
        {
            Banners = new List<PictureModel>();
        }

        public bool Loop { get; set; }

        public bool Nav { get; set; }

        public bool AutoPlay { get; set; }

        public int AutoPlayTimeout { get; set; }

        public bool AutoPlayHoverPause { get; set; }

        public IList<PictureModel> Banners { get; set; }
    }
}
