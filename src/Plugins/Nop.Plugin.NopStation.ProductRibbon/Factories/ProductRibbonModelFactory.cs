using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.ProductRibbon.Infrastructure.Cache;
using Nop.Plugin.NopStation.ProductRibbon.Models;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;

namespace Nop.Plugin.NopStation.ProductRibbon.Factories
{
    public class ProductRibbonModelFactory : IProductRibbonModelFactory
    {
        private readonly ICustomerService _customerService;
        #region Fields

        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ProductRibbonSettings _productRibbonSettings;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IOrderReportService _orderReportService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly ICacheKeyService _cacheKeyService;

        #endregion

        #region Ctor

        public ProductRibbonModelFactory(
            ICustomerService customerService,
            ITaxService taxService,
            IPriceCalculationService priceCalculationService,
            ProductRibbonSettings productRibbonSettings,
            IOrderReportService orderReportService,
            IStaticCacheManager staticCacheManger,
            IPermissionService permissionService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IProductService productService,
            ICacheKeyService cacheKeyService
            )
        {
            _customerService = customerService;
            _taxService = taxService;
            _priceCalculationService = priceCalculationService;
            _productRibbonSettings = productRibbonSettings;
            _staticCacheManager = staticCacheManger;
            _orderReportService = orderReportService;
            _permissionService = permissionService;
            _workContext = workContext;
            _localizationService = localizationService;
            _productService = productService;
            _cacheKeyService = cacheKeyService;
        }

        #endregion

        public ProductRibbonModel PrepareProductRibbonModel(Product product)
        {
            if (product == null)
                return null;

            var key = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.PRODUCT_RIBBON_MODEL_KEY, 
                product, 
                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer), 
                _workContext.WorkingLanguage);

            var cachedModel = _staticCacheManager.Get(key, () =>
            {
                var model = new ProductRibbonModel();

                if (_productRibbonSettings.EnableBestSellerRibbon)
                {
                    var dateTime = DateTime.UtcNow.AddDays(-30);
                    var bestSellerReport = _orderReportService.BestSellersReport(createdFromUtc: dateTime);
                    model.IsBestSeller = bestSellerReport.Any(x => x.ProductId == product.Id);
                }

                if (_productRibbonSettings.EnableNewRibbon)
                {
                    model.IsNew = product.MarkAsNew 
                                    && (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) 
                                    && (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow);
                }

                if (_productRibbonSettings.EnableDiscountRibbon 
                        && product.ProductType == ProductType.SimpleProduct 
                        && _permissionService.Authorize(StandardPermissionProvider.DisplayPrices) 
                        && !product.CustomerEntersPrice && product.CallForPrice)
                {
                    var finalPriceWithDiscountBase = _taxService.GetProductPrice(product, 
                        _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: true), out decimal taxRate);
                    var specialPrice = _productService.GetTierPricesByProduct(product.Id).FirstOrDefault(x => x.Quantity == 0 && x.CustomerRoleId == null);
                    var mRP = product.OldPrice > 0 ? product.OldPrice : product.Price;
                    var salePrice = specialPrice != null ? specialPrice.Price : finalPriceWithDiscountBase;

                    if (mRP > 0)
                    {
                        var save = mRP - salePrice;
                        if (save > 0)
                        {
                            var productPrice = (int)Math.Ceiling(save * 100 / mRP);
                            model.Discount = string.Format(_localizationService.GetResource("NopStation.ProductRibbon.RibbonText.Discount"), productPrice);
                        }
                    }
                }

                return model;
            });

            return cachedModel;
        }
    }
}
