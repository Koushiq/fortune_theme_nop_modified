using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.NopStation.ProductRibbon.Areas.Admin.Models
{
    public partial class ConfigurationModel : BaseNopModel, ISettingsModel
    {
        [NopResourceDisplayName("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductDetailsPageWidgetZone")]
        public string ProductDetailsPageWidgetZone { get; set; }
        public bool ProductDetailsPageWidgetZone_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductOverviewBoxWidgetZone")]
        public string ProductOverviewBoxWidgetZone { get; set; }
        public bool ProductOverviewBoxWidgetZone_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableNewRibbon")]
        public bool EnableNewRibbon { get; set; }
        public bool EnableNewRibbon_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableDiscountRibbon")]
        public bool EnableDiscountRibbon { get; set; }
        public bool EnableDiscountRibbon_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableBestSellerRibbon")]
        public bool EnableBestSellerRibbon { get; set; }
        public bool EnableBestSellerRibbon_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}