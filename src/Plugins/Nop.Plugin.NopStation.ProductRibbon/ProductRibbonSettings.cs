using Nop.Core.Configuration;

namespace Nop.Plugin.NopStation.ProductRibbon
{
    public partial class ProductRibbonSettings : ISettings
    {
        public string ProductDetailsPageWidgetZone { get; set; }

        public string ProductOverviewBoxWidgetZone { get; set; }

        public bool EnableNewRibbon { get; set; }

        public bool EnableDiscountRibbon { get; set; }

        public bool EnableBestSellerRibbon { get; set; }
    }
}
