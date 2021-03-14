using Nop.Web.Framework.Models;

namespace Nop.Plugin.NopStation.ProductRibbon.Models
{
    public class ProductRibbonModel : BaseNopModel
    {
        public string Discount { get; set; }

        public bool IsNew { get; set; }

        public bool IsBestSeller { get; set; }
    }
}