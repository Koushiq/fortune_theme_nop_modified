using Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Models;
using Nop.Plugin.NopStation.ProductTabs.Domains;

namespace Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Factories
{
    public interface IProductTabModelFactory
    {
        ProductTabSearchModel PrepareOCarouselSearchModel(ProductTabSearchModel searchModel);

        ProductTabListModel PrepareProductTabListModel(ProductTabSearchModel searchModel);

        ProductTabModel PrepareProductTabModel(ProductTabModel model, ProductTab productTab, 
            bool excludeProperties = false);
        
        ProductTabItemListModel PrepareProductTabItemListModel(ProductTabItemSearchModel searchModel, ProductTab productTab);

        ProductTabItemModel PrepareProductTabItemModel(ProductTabItemModel model, ProductTabItem productTabItem,
            ProductTab productTab, bool excludeProperties = false);

        ProductTabItemProductListModel PrepareProductTabItemProductListModel(ProductTabItemProductSearchModel searchModel, 
            ProductTabItem productTabItem);

        AddProductToProductTabItemSearchModel PrepareAddProductToProductTabItemSearchModel(AddProductToProductTabItemSearchModel searchModel);

        AddProductToProductTabItemListModel PrepareAddProductToProductTabItemListModel(AddProductToProductTabItemSearchModel searchModel);
    }
}