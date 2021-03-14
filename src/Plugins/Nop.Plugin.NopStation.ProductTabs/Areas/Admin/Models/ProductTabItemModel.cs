using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Models
{
    public class ProductTabItemModel: BaseNopEntityModel, ILocalizedModel<ProductTabItemLocalizedModel>
    {
        public ProductTabItemModel()
        {
            ProductSearchModel = new ProductTabItemProductSearchModel();
            Locales = new List<ProductTabItemLocalizedModel>();
        }

        [NopResourceDisplayName("NopStation.ProductTabs.ProductTabItems.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("NopStation.ProductTabs.ProductTabItems.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("NopStation.ProductTabs.ProductTabItems.Fields.ProductTab")]
        public int ProductTabId { get; set; }
        
        public IList<ProductTabItemLocalizedModel> Locales { get; set; }

        public ProductTabItemProductSearchModel ProductSearchModel { get; set; }
    }

    public class ProductTabItemLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("NopStation.ProductTabs.ProductTabItems.Fields.Name")]
        public string Name { get; set; }
    }
}
