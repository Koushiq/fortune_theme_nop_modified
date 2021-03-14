using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Models
{
    public class ProductTabItemProductModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("NopStation.ProductTabs.ProductTabItemProducts.Fields.ProductTabItem")]
        public int ProductTabItemId { get; set; }

        [NopResourceDisplayName("NopStation.ProductTabs.ProductTabItemProducts.Fields.Product")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("NopStation.ProductTabs.ProductTabItemProducts.Fields.Product")]
        public string ProductName { get; set; }

        [NopResourceDisplayName("NopStation.ProductTabs.ProductTabItemProducts.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
