using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Models
{
    public class CategoryBannerSearchModel : BaseSearchModel
    {
        public CategoryBannerSearchModel()
        {
            CategoryBanner = new CategoryBannerModel();
        }

        public int CategoryId { get; set; }

        public CategoryBannerModel CategoryBanner { get; set; }
    }
}
