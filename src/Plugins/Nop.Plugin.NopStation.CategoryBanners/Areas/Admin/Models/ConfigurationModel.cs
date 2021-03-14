using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Models
{
    public class ConfigurationModel : BaseNopModel, ISettingsModel
    {
        [NopResourceDisplayName("Admin.NopStation.CategoryBanners.Configuration.Fields.HideInPublicStore")]
        public bool HideInPublicStore { get; set; }
        public bool HideInPublicStore_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CategoryBanners.Configuration.Fields.BannerPictureSize")]
        public int BannerPictureSize { get; set; }
        public bool BannerPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CategoryBanners.Configuration.Fields.Loop")]
        public bool Loop { get; set; }
        public bool Loop_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CategoryBanners.Configuration.Fields.Nav")]
        public bool Nav { get; set; }
        public bool Nav_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlay")]
        public bool AutoPlay { get; set; }
        public bool AutoPlay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlayTimeout")]
        public int AutoPlayTimeout { get; set; }
        public bool AutoPlayTimeout_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlayHoverPause")]
        public bool AutoPlayHoverPause { get; set; }
        public bool AutoPlayHoverPause_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}
