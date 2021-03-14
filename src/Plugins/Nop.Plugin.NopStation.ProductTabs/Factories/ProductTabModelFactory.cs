using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Models.Media;
using Nop.Web.Factories;
using Nop.Core;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Security;
using Nop.Plugin.NopStation.ProductTabs.Services;
using Nop.Plugin.NopStation.ProductTabs.Domains;
using Nop.Plugin.NopStation.ProductTabs.Models;
using Nop.Plugin.NopStation.ProductTabs.Helpers;
using Nop.Services.Caching;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.ProductTabs.Infrastructure.Cache;
using Nop.Services.Customers;

namespace Nop.Plugin.NopStation.ProductTabs.Factories
{
    public class ProductTabModelFactory : IProductTabModelFactory
    {

        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICategoryService _categoryService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizationService _localizationService;
        private readonly MediaSettings _mediaSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderReportService _orderReportService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly IProductTabService _productTabService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ProductTabModelFactory(
            ICustomerService customerService,
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            ICategoryService categoryService,
            IPictureService pictureService,
            IProductService productService,
            IUrlRecordService urlRecordService,
            ILocalizationService localizationService,
            MediaSettings mediaSettings,
            IProductModelFactory productModelFactory,
            IManufacturerService manufacturerService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IStoreContext storeContext,
            IOrderReportService orderReportService,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            IProductTabService productTabService,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _categoryService = categoryService;
            _pictureService = pictureService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _localizationService = localizationService;
            _mediaSettings = mediaSettings;
            _productModelFactory = productModelFactory;
            _manufacturerService = manufacturerService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _storeContext = storeContext;
            _orderReportService = orderReportService;
            _storeMappingService = storeMappingService;
            _aclService = aclService;
            _productTabService = productTabService;
            _workContext = workContext;
        }

        #endregion

        #region Utlities 

        protected PictureModel PreparePictureModel(ProductTab productTab)
        {
            return new PictureModel()
            {
                ImageUrl = _pictureService.GetPictureUrl(productTab.PictureId),
                AlternateText = productTab.PictureAlt,
                Title = productTab.PictureTitle
            };
        }

        #endregion

        #region Methods

        public IList<ProductTabModel> PrepareProductTabListModel(List<ProductTab> productTabs)
        {
            if (productTabs == null)
                throw new ArgumentNullException(nameof(productTabs));

            var model = new List<ProductTabModel>();
            foreach (var productTab in productTabs)
            {
                model.Add(PrepareProductTabModel(productTab));
            }
            return model;
        }

        public IList<ProductTabModel> PrepareProductTabListModel(string widgetZone)
        {
            if (string.IsNullOrEmpty(widgetZone))
                throw new ArgumentNullException(nameof(widgetZone));

            if (!ProductTabHelper.TryGetWidgetZoneId(widgetZone, out var widgetZoneId))
                return new List<ProductTabModel>();

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.PRODUCT_TAB_MODEL_KEY,
                                            widgetZoneId,
                                            _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                                            _workContext.WorkingLanguage,
                                            _storeContext.CurrentStore);

            var productTabModels = _staticCacheManager.Get(cacheKey, () =>
            {
                var productTabs = _productTabService.GetAllProductTabs(
                                        new List<int> { widgetZoneId }, true, _storeContext.CurrentStore.Id, true).ToList();
                var productTabModelList = PrepareProductTabListModel(productTabs);

                return productTabModelList;
            });

            return productTabModels;
        }

        public ProductTabModel PrepareProductTabModel(ProductTab productTab)
        {
            if (productTab == null)
                throw new ArgumentNullException(nameof(productTab));

            var model = new ProductTabModel
            {
                Id = productTab.Id,
                AutoPlay = productTab.AutoPlay,
                RTL = _workContext.WorkingLanguage.Rtl,
                CustomCssClass = productTab.CustomCssClass,
                AutoPlayHoverPause = productTab.AutoPlayHoverPause,
                AutoPlayTimeout = productTab.AutoPlayTimeout,
                Center = productTab.Center,
                LazyLoad = productTab.LazyLoad,
                LazyLoadEager = productTab.LazyLoadEager,
                Loop = productTab.Loop,
                Margin = productTab.Margin,
                Nav = productTab.Nav,
                StartPosition = productTab.StartPosition,
                CustomUrl = productTab.CustomUrl
            };
            if (productTab.DisplayTitle)
            {
                model.DisplayTitle = productTab.DisplayTitle;
                model.Title = _localizationService.GetLocalized(productTab, x => x.TabTitle);
            }
            model.Picture = PreparePictureModel(productTab);

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.PRODUCT_TAB_ITEM_MODEL_KEY,
                productTab,
                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                _workContext.WorkingLanguage,
                _storeContext.CurrentStore);

            model.Items = _staticCacheManager.Get(cacheKey, () =>
            {
                var productTabItemModels = new List<ProductTabItemModel>();
                var productTabItems = _productTabService.GetProductTabItemsByProductTabId(productTab.Id);

                foreach (var item in productTabItems)
                {
                    productTabItemModels.Add(PrepareProductTabItemModel(item));
                }
                return productTabItemModels;
            });

            return model;
        }

        private ProductTabItemModel PrepareProductTabItemModel(ProductTabItem item)
        {
            var model = new ProductTabItemModel()
            {
                Name = _localizationService.GetLocalized(item, x => x.Name),
                Id = item.Id
            };

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.PRODUCT_TAB_ITEM_PRODUCT_MODEL_KEY,
                item,
                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                _workContext.CurrentVendor,
                _storeContext.CurrentStore);

            var productIds = _productTabService.GetProductTabItemProductsByProductTabItemId(item.Id).Select(x => x.ProductId).ToArray();
            var publishedProducts = _productService.GetProductsByIds(productIds).Where(p => p.Published).ToList();
            publishedProducts = publishedProducts.Where(p => 
                                                        _aclService.Authorize(p) 
                                                        && _storeMappingService.Authorize(p)
                                                        && _productService.ProductIsAvailable(p)).ToList();
            model.Products = _productModelFactory.PrepareProductOverviewModels(publishedProducts).ToList();

            return model;
        }

        #endregion
    }
}
