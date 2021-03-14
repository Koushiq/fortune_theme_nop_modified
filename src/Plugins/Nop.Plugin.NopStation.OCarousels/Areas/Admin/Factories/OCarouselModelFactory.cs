using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.OCarousels.Areas.Admin.Models;
using Nop.Plugin.NopStation.OCarousels.Domains;
using Nop.Plugin.NopStation.OCarousels.Helpers;
using Nop.Plugin.NopStation.OCarousels.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.NopStation.OCarousels.Areas.Admin.Factories
{
    public partial class OCarouselModelFactory : IOCarouselModelFactory
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IOCarouselService _carouselService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public OCarouselModelFactory(IStoreContext storeContext,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            ILocalizedModelFactory localizedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            IOCarouselService carouselService,
            IProductService productService,
            IPictureService pictureService,
            ISettingService settingService,
            IDateTimeHelper dateTimeHelper)
        {
            _storeContext = storeContext;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _localizedModelFactory = localizedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _carouselService = carouselService;
            _productService = productService;
            _pictureService = pictureService;
            _settingService = settingService;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        protected void PrepareCustomWidgetZones(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available activity log types
            var availableWidgetZones = OCarouselHelper.GetCustomWidgetZoneSelectList();
            foreach (var zone in availableWidgetZones)
            {
                items.Add(zone);
            }

            if (withSpecialDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });
        }

        protected void PrepareDataSourceTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availableDataSourceTypes = DataSourceTypeEnum.BestSellers.ToSelectList(false).ToList();
            foreach (var source in availableDataSourceTypes)
            {
                items.Add(source);
            }

            if (withSpecialDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });
        }

        protected void PrepareActiveOptions(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            items.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("Admin.NopStation.OCarousels.OCarousels.List.SearchActive.Active"),
                Value = "1"
            });
            items.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("Admin.NopStation.OCarousels.OCarousels.List.SearchActive.Inactive"),
                Value = "2"
            });

            if (withSpecialDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });
        }

        #endregion

        #region Methods

        #region Configuration

        public ConfigurationModel PrepareConfigurationModel()
        {
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var carouselSettings = _settingService.LoadSetting<OCarouselSettings>(storeId);

            var model = carouselSettings.ToSettingsModel<ConfigurationModel>();

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            model.EnableOCarousel_OverrideForStore = _settingService.SettingExists(carouselSettings, x => x.EnableOCarousel, storeId);

            return model;
        }

        #endregion

        #region OCarousel

        public virtual OCarouselSearchModel PrepareOCarouselSearchModel(OCarouselSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            PrepareCustomWidgetZones(searchModel.AvailableWidgetZones, true);
            PrepareDataSourceTypes(searchModel.AvailableDataSources, true);
            PrepareActiveOptions(searchModel.AvailableActiveOptions, true);

            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            return searchModel;
        }

        public virtual OCarouselListModel PrepareOCarouselListModel(OCarouselSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var widgetZoneIds = searchModel.SearchWidgetZones?.Contains(0) ?? true ? null : searchModel.SearchWidgetZones.ToList();
            var dataSources = searchModel.SearchDataSources?.Contains(0) ?? true ? null : searchModel.SearchDataSources.ToList();

            bool? active = null;
            if (searchModel.SearchActiveId == 1)
                active = true;
            else if (searchModel.SearchActiveId == 2)
                active = false;

            //get carousels
            var carousels = _carouselService.GetAllCarousels(widgetZoneIds, dataSources, searchModel.SearchStoreId, 
                active, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new OCarouselListModel().PrepareToGrid(searchModel, carousels, () =>
            {
                return carousels.Select(carousel =>
                {
                    return PrepareOCarouselModel(null, carousel, true);
                });
            });

            return model;
        }

        public OCarouselModel PrepareOCarouselModel(OCarouselModel model, OCarousel carousel, bool excludeProperties = false)
        {
            Action<OCarouselLocalizedModel, int> localizedModelConfiguration = null;

            if (carousel != null)
            {
                if (model == null)
                {
                    model = carousel.ToModel<OCarouselModel>();

                    model.DataSourceTypeStr = _localizationService.GetLocalizedEnum(carousel.DataSourceTypeEnum);
                    model.WidgetZoneStr = OCarouselHelper.GetCustomWidgetZone(carousel.WidgetZoneId);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(carousel.CreatedOnUtc, DateTimeKind.Utc);
                    model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(carousel.UpdatedOnUtc, DateTimeKind.Utc);
                }

                if (!excludeProperties)
                {
                    model.OCarouselItemSearchModel = new OCarouselItemSearchModel()
                    {
                        OCarouselId = carousel.Id
                    };

                    localizedModelConfiguration = (locale, languageId) =>
                    {
                        locale.Title = _localizationService.GetLocalized(carousel, entity => entity.Title, languageId, false, false);
                    };
                }
            }

            if (!excludeProperties)
            {
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
                PrepareCustomWidgetZones(model.AvailableWidgetZones, false);
                PrepareDataSourceTypes(model.AvailableDataSources, false);
                _storeMappingSupportedModelFactory.PrepareModelStores(model, carousel, excludeProperties);
            }

            return model;
        }

        #endregion

        #region OCarousel items

        public OCarouselItemListModel PrepareOCarouselItemListModel(OCarouselItemSearchModel searchModel, OCarousel carousel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (carousel == null)
                throw new ArgumentNullException(nameof(carousel));

            var carouselItems = _carouselService.GetOCarouselItemsByOCarouselId(carousel.Id, searchModel.Page - 1, searchModel.PageSize);

            //prepare grid model
            var model = new OCarouselItemListModel().PrepareToGrid(searchModel, carouselItems, () =>
            {
                //fill in model values from the entity
                return carouselItems.Select(carouselItem =>
                {
                    var product = _productService.GetProductById(carouselItem.ProductId);
                    var defaultProductPicture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();

                    var carouselItemModel = carouselItem.ToModel<OCarouselItemModel>();
                    carouselItemModel.ProductName = product.Name;
                    carouselItemModel.PictureUrl = _pictureService.GetPictureUrl(defaultProductPicture.Id, 75);

                    return carouselItemModel;
                });
            });

            return model;
        }

        public AddProductToCarouselSearchModel PrepareAddProductToOCarouselSearchModel(AddProductToCarouselSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare available product types
            _baseAdminModelFactory.PrepareProductTypes(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        public AddProductToCarouselListModel PrepareAddProductToOCarouselListModel(AddProductToCarouselSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerId: searchModel.SearchManufacturerId,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddProductToCarouselListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        #endregion

        #endregion
    }
}
