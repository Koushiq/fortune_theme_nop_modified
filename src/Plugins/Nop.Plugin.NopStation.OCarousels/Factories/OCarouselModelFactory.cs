using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Plugin.NopStation.OCarousels.Domains;
using Nop.Plugin.NopStation.OCarousels.Models;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Models.Media;
using Nop.Web.Factories;
using Nop.Core;
using Nop.Web.Infrastructure.Cache;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Security;
using Nop.Plugin.NopStation.OCarousels.Services;
using Nop.Services.Caching;
using Nop.Services.Customers;

namespace Nop.Plugin.NopStation.OCarousels.Factories
{
    public partial class OCarouselModelFactory : IOCarouselModelFactory
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ICategoryService _categoryService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizationService _localizationService;
        private readonly MediaSettings _mediaSettings;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderReportService _orderReportService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly IOCarouselService _carouselService;
        private readonly IWorkContext _workContext;
        private readonly OCarouselSettings _carouselSettings;
        private readonly ICacheKeyService _cacheKeyService;

        #endregion

        #region Ctor

        public OCarouselModelFactory(
            ICustomerService customerService,
            ICategoryService categoryService,
            IPictureService pictureService,
            IProductService productService,
            IUrlRecordService urlRecordService,
            ILocalizationService localizationService,
            MediaSettings mediaSettings,
            IStaticCacheManager staticCacheManager,
            IProductModelFactory productModelFactory,
            IManufacturerService manufacturerService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IStoreContext storeContext,
            IOrderReportService orderReportService,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            IOCarouselService carouselService,
            IWorkContext workContext,
            OCarouselSettings carouselSettings,
            ICacheKeyService cacheKeyService
            )
        {
            _customerService = customerService;
            _categoryService = categoryService;
            _pictureService = pictureService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _localizationService = localizationService;
            _mediaSettings = mediaSettings;
            _cacheManager = staticCacheManager;
            _productModelFactory = productModelFactory;
            _manufacturerService = manufacturerService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _storeContext = storeContext;
            _orderReportService = orderReportService;
            _storeMappingService = storeMappingService;
            _aclService = aclService;
            _carouselService = carouselService;
            _workContext = workContext;
            _carouselSettings = carouselSettings;
            _cacheKeyService = cacheKeyService;
        }

        #endregion

        #region Utlities 

        protected IList<OCarouselModel.OCarouselManufacturerModel> PrepareManufacturerListModel(List<Manufacturer> manufacturers)
        {
            var listModel = new List<OCarouselModel.OCarouselManufacturerModel>();

            foreach (var manufacturer in manufacturers)
            {
                var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(Infrastructure.Cache.ModelCacheEventConsumer.OCAROUSEL_MANUFACTURER_MODEL_KEY , 
                    manufacturer.Id,
                    _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer), 
                    _workContext.WorkingLanguage,
                    _storeContext.CurrentStore);
                var cachedModel = _cacheManager.Get(cacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(manufacturer.PictureId);
                    var cm = new OCarouselModel.OCarouselManufacturerModel()
                    {
                        Name = _localizationService.GetLocalized(manufacturer, x => x.Name),
                        SeName = _urlRecordService.GetSeName(manufacturer),
                        PictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(ref picture, _mediaSettings.ProductThumbPictureSize),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                            Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                                            ? picture.TitleAttribute
                                            : string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), manufacturer.Name),
                            AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                                            ? picture.AltAttribute
                                            : string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), manufacturer.Name)
                        },
                    };
                    return cm;
                });

                listModel.Add(cachedModel);
            }

            return listModel;
        }

        protected IList<OCarouselModel.OCarouselCategoryModel> PrepareCategoryListModel(IList<Category> categories)
        {
            var listModel = new List<OCarouselModel.OCarouselCategoryModel>();
            foreach (var category in categories)
            {
                var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(Infrastructure.Cache.ModelCacheEventConsumer.OCAROUSEL_CATEGORY_MODEL_KEY,
                    category,
                    _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
                    _workContext.WorkingLanguage,
                    _storeContext.CurrentStore);

                var cachedModel = _cacheManager.Get(cacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(category.PictureId);
                    var cm = new OCarouselModel.OCarouselCategoryModel()
                    {
                        Name = _localizationService.GetLocalized(category, x => x.Name),
                        SeName = _urlRecordService.GetSeName(category),
                        PictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(ref picture, _mediaSettings.ProductThumbPictureSize),
                            FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                            Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                                            ? picture.TitleAttribute
                                            : string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), category.Name),
                            AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                                            ? picture.AltAttribute
                                            : string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), category.Name)
                        },
                    };
                    return cm;
                });

                listModel.Add(cachedModel);
            }

            return listModel;
        }

        #endregion

        #region Methods

        public IList<OCarouselModel> PrepareCarouselListModel(IList<OCarousel> carousels)
        {
            if (carousels == null)
                throw new ArgumentNullException(nameof(carousels));

            var model = new List<OCarouselModel>();
            foreach (var carousel in carousels)
            {
                model.Add(PrepareCarouselModel(carousel));
            }
            return model;
        }

        public OCarouselModel PrepareCarouselModel(OCarousel carousel)
        {
            if (carousel == null)
                throw new ArgumentNullException(nameof(carousel));

            var model = new OCarouselModel
            {
                Id = carousel.Id,
                AutoPlay = carousel.AutoPlay,
                RTL = _workContext.WorkingLanguage.Rtl,
                CustomCssClass = carousel.CustomCssClass,
                AutoPlayHoverPause = carousel.AutoPlayHoverPause,
                AutoPlayTimeout = carousel.AutoPlayTimeout,
                Center = carousel.Center,
                LazyLoad = carousel.LazyLoad,
                LazyLoadEager = carousel.LazyLoadEager,
                Loop = carousel.Loop,
                Nav = carousel.Nav,
                StartPosition = carousel.StartPosition,
                DataSourceTypeEnum = carousel.DataSourceTypeEnum
            };

            if (carousel.ShowBackgroundPicture)
            {
                model.ShowBackgroundPicture = carousel.ShowBackgroundPicture;
                var backgroundPictureCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(Infrastructure.Cache.ModelCacheEventConsumer.OCAROUSEL_BACKGROUND_PICTURE_MODEL_KEY, 
                    carousel, 
                    _storeContext.CurrentStore);
                model.BackgroundPictureUrl = _cacheManager.Get(backgroundPictureCacheKey, () =>
                {
                    var pictureUrl = _pictureService.GetPictureUrl(carousel.BackgroundPictureId);
                    return pictureUrl;
                });
            }

            if (carousel.DisplayTitle)
            {
                model.DisplayTitle = true;
                model.Title = _localizationService.GetLocalized(carousel, x => x.Title);
            }
            if (carousel.DataSourceTypeEnum == DataSourceTypeEnum.HomePageCategories)
            {
                var categories = _categoryService.GetAllCategoriesDisplayedOnHomepage();

                categories = categories.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p))
                    .Take(carousel.NumberOfItemsToShow).ToList();

                model.Categories = PrepareCategoryListModel(categories);
            }
            else if (carousel.DataSourceTypeEnum == DataSourceTypeEnum.HomePageProducts)
            {
                var products = _productService.GetAllProductsDisplayedOnHomepage();
                //ACL and store mapping
                products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
                //availability dates
                products = products.Where(p => _productService.ProductIsAvailable(p)).Take(carousel.NumberOfItemsToShow).ToList();

                model.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            }
            else if (carousel.DataSourceTypeEnum == DataSourceTypeEnum.Manufacturers)
            {
                var manufacturers = _manufacturerService.GetAllManufacturers().ToList();

                manufacturers = manufacturers.Where(m => _storeMappingService.Authorize(m)).Take(carousel.NumberOfItemsToShow).ToList();

                model.Manufacturers = PrepareManufacturerListModel(manufacturers);
            }
            else if (carousel.DataSourceTypeEnum == DataSourceTypeEnum.NewProducts)
            {
                var products = _productService.SearchProducts(
                        storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        markedAsNewOnly: true,
                        orderBy: ProductSortingEnum.CreatedOn,
                        pageSize: carousel.NumberOfItemsToShow
                        ).ToList();
                model.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            }
            else if (carousel.DataSourceTypeEnum == DataSourceTypeEnum.RecentlyViewedProducts)
            {
                var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(carousel.NumberOfItemsToShow);
                model.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            }
            else if (carousel.DataSourceTypeEnum == DataSourceTypeEnum.BestSellers)
            {
                var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.HomepageBestsellersIdsKey, _storeContext.CurrentStore.Id);
                var report = _cacheManager.Get( cacheKey , () =>
                    _orderReportService.BestSellersReport(storeId: _storeContext.CurrentStore.Id, pageSize: 10)
                .ToList());

                //load products
                var products = _productService.GetProductsByIds(report.Select(x => x.ProductId).ToArray());
                //ACL and store mapping
                products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
                //availability dates
                products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

                model.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            }
            else if (carousel.DataSourceTypeEnum == DataSourceTypeEnum.Custom)
            {
                var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(Infrastructure.Cache.ModelCacheEventConsumer.OCAROUSEL_CUSTOMRODUCTIDS_MODEL_KEY, carousel.Id);
                var productIds = _cacheManager.Get(cacheKey, () =>
                {
                    return _carouselService.GetOCarouselItemsByOCarouselId(carousel.Id)
                        .Select(ci => ci.ProductId)
                        .ToArray();
                });

                var sp = _productService.GetProductsByIds(productIds).Where(p => p.Published).ToList();
                sp = sp.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
                var products = sp.Where(p => _productService.ProductIsAvailable(p)).ToList();
                model.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            }
            return model;
        }

        #endregion
    }
}