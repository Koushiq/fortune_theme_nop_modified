using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.QuickView.Models;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.NopStation.QuickView.Controllers
{
    public class QuickViewController : BasePluginController
    {
        #region Fields

        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly QuickViewSettings _quickViewSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductService _productService;
        private readonly IAclService _aclService;

        #endregion

        #region Ctor

        public QuickViewController(IRecentlyViewedProductsService recentlyViewedProductsService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductModelFactory productModelFactory,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            QuickViewSettings quickViewSettings,
            IProductService productService,
            CatalogSettings catalogSettings,
            IAclService aclService)
        {
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _storeMappingService = storeMappingService;
            _productModelFactory = productModelFactory;
            _permissionService = permissionService;
            _quickViewSettings = quickViewSettings;
            _productService = productService;
            _catalogSettings = catalogSettings;
            _aclService = aclService;
        }

        #endregion

        #region Product details page

        //[HttpsRequirement(SslRequirement.No)]
        public IActionResult ProductDetails(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return NotFound();

            var notAvailable =
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                !_aclService.Authorize(product) ||
                !_storeMappingService.Authorize(product) ||
                !_productService.ProductIsAvailable(product);
            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageProducts);
            if (notAvailable && !hasAdminAccess)
                return NotFound();

            if (!product.VisibleIndividually)
            {
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    product = parentGroupedProduct;
                else
                    return NotFound();
            }

            var model = new QuickViewProductDetailsModel()
            {
                ProductDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false),
                ShowAlsoPurchasedProducts = _quickViewSettings.ShowAlsoPurchasedProducts,
                ShowRelatedProducts = _quickViewSettings.ShowRelatedProducts,
                ShowAddToWishlistButton = _quickViewSettings.ShowAddToWishlistButton,
                ShowAvailability = _quickViewSettings.ShowAvailability,
                ShowProductEmailAFriendButton = _quickViewSettings.ShowProductEmailAFriendButton,
                Id = product.Id,
                ShowCompareProductsButton = _quickViewSettings.ShowCompareProductsButton,
                ShowDeliveryInfo = _quickViewSettings.ShowDeliveryInfo,
                ShowFullDescription = _quickViewSettings.ShowFullDescription,
                ShowProductManufacturers = _quickViewSettings.ShowProductManufacturers,
                ShowProductReviewOverview = _quickViewSettings.ShowProductReviewOverview,
                ShowShortDescription = _quickViewSettings.ShowShortDescription,
                ShowProductSpecifications = _quickViewSettings.ShowProductSpecifications,
                ShowProductTags = _quickViewSettings.ShowProductTags,
            };

            var productTemplateViewPath = _productModelFactory.PrepareProductTemplateViewPath(product);

            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product);

            var html = RenderPartialViewToString($"QuickView{productTemplateViewPath}", model);
            return Json(new
            {
                html
            });
        }

        #endregion
    }
}
