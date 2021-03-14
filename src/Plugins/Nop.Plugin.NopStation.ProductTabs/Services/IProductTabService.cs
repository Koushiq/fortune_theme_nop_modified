using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.NopStation.ProductTabs.Domains;

namespace Nop.Plugin.NopStation.ProductTabs.Services
{
    public interface IProductTabService
    {
        void DeleteProductTab(ProductTab productTab);

        void InsertProductTab(ProductTab productTab);

        void UpdateProductTab(ProductTab productTab);

        ProductTab GetProductTabById(int productTabId);

        IPagedList<ProductTab> GetAllProductTabs(List<int> widgetZoneIds = null, bool hasItemsOnly = false,
            int storeId = 0, bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue);

        void DeleteProductTabItem(ProductTabItem productTabItem);

        void UpdateProductTabItem(ProductTabItem productTabItem);
         
        void InsertProductTabItemProduct(ProductTabItemProduct productTabItemProduct);
        void DeleteProductTabItemProduct(ProductTabItemProduct productTabItemProduct);

        void UpdateProductTabItemProduct(ProductTabItemProduct productTabItemProduct);

        void InsertProductTabItem(ProductTabItem productTabItem);

        ProductTabItem GetProductTabItemById(int productTabItemId);
     

        ProductTabItemProduct GetProductTabItemProductById(int productTabItemProductId);
        List<ProductTabItem> GetProductTabItemsByProductTabId(int productTabId);
        List<ProductTabItemProduct> GetProductTabItemProductsByProductTabItemId(int productTabItemId);
    }
}