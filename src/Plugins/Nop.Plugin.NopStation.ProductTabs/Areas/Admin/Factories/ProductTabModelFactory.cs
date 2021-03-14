using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Models;
using Nop.Plugin.NopStation.ProductTabs.Domains;
using Nop.Plugin.NopStation.ProductTabs.Helpers;
using Nop.Plugin.NopStation.ProductTabs.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Factories
{
    public partial class ProductTabModelFactory : IProductTabModelFactory
    {
        #region Fields

        private readonly IProductTabService _productTabService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly ILocalizedModelFactory _localizedModelFactory;

        #endregion

        #region Ctor

        public ProductTabModelFactory(IProductTabService productTabService,
            ILocalizationService localizationService,
            IProductService productService,
            IPictureService pictureService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IUrlRecordService urlRecordService,
            IDateTimeHelper dateTimeHelper,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            ILocalizedModelFactory localizedModelFactory)
        {
            _productTabService = productTabService;
            _localizationService = localizationService;
            _productService = productService;
            _pictureService = pictureService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _urlRecordService = urlRecordService;
            _dateTimeHelper = dateTimeHelper;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _localizedModelFactory = localizedModelFactory;
        }

        #endregion

        #region Utilities

        protected void PrepareCustomWidgetZones(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available activity log types
            var availableWidgetZones = ProductTabHelper.GetCustomWidgetZoneSelectList();
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

        protected void PrepareActiveOptions(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            items.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("NopStation.ProductTabs.ProductTabs.List.SearchActive.Active"),
                Value = "1"
            });
            items.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("NopStation.ProductTabs.ProductTabs.List.SearchActive.Inactive"),
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

        #region Product tabs

        public ProductTabSearchModel PrepareOCarouselSearchModel(ProductTabSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            PrepareCustomWidgetZones(searchModel.AvailableWidgetZones, true);
            PrepareActiveOptions(searchModel.AvailableActiveOptions, true);

            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            return searchModel;
        }

        public virtual ProductTabListModel PrepareProductTabListModel(ProductTabSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var widgetZoneIds = searchModel.SearchWidgetZones?.Contains(0) ?? true ? null : searchModel.SearchWidgetZones.ToList();

            bool? active = null;
            if (searchModel.SearchActiveId == 1)
                active = true;
            else if (searchModel.SearchActiveId == 2)
                active = false;

            //get productTabs
            var productTabs = _productTabService.GetAllProductTabs(widgetZoneIds, false,
                searchModel.SearchStoreId, active, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new ProductTabListModel().PrepareToGrid(searchModel, productTabs, () =>
            {
                return productTabs.Select(productTab =>
                {
                    return PrepareProductTabModel(null, productTab, true);
                });
            });

            return model;
        }

        public ProductTabModel PrepareProductTabModel(ProductTabModel model, ProductTab productTab,
            bool excludeProperties = false)
        {
            Action<ProductTabLocalizedModel, int> localizedModelConfiguration = null;

            if (productTab != null)
            {
                if (model == null)
                {
                    model = productTab.ToModel<ProductTabModel>();
                    model.WidgetZoneStr = ProductTabHelper.GetCustomWidgetZone(productTab.WidgetZoneId);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(productTab.CreatedOnUtc, DateTimeKind.Utc);
                    model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(productTab.UpdatedOnUtc, DateTimeKind.Utc);
                }

                if (!excludeProperties)
                {
                    model.ProductTabItemSearchModel = new ProductTabItemSearchModel()
                    {
                        ProductTabId = productTab.Id
                    };

                    localizedModelConfiguration = (locale, languageId) =>
                    {
                        locale.Name = _localizationService.GetLocalized(productTab, entity => entity.Name, languageId, false, false);
                        locale.TabTitle = _localizationService.GetLocalized(productTab, entity => entity.TabTitle, languageId, false, false);
                    };
                }
            }

            if (!excludeProperties)
            {
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
                PrepareCustomWidgetZones(model.AvailableWidgetZones, false);
                _storeMappingSupportedModelFactory.PrepareModelStores(model, productTab, excludeProperties);
            }

            return model;
        }

        #endregion

        #region Product tab items

        public ProductTabItemListModel PrepareProductTabItemListModel(ProductTabItemSearchModel searchModel, ProductTab productTab)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productTab == null)
                throw new ArgumentNullException(nameof(productTab));

            var productTabItems = _productTabService.GetProductTabItemsByProductTabId(productTab.Id).OrderBy(x => x.DisplayOrder).ToList().ToPagedList(searchModel);

            //prepare grid model
            var model = new ProductTabItemListModel().PrepareToGrid(searchModel, productTabItems, () =>
            {
                //fill in model values from the entity
                return productTabItems.Select(productTabItem =>
                {
                    var productTabItemModel = new ProductTabItemModel
                    {
                        Id = productTabItem.Id,
                        DisplayOrder = productTabItem.DisplayOrder,
                        Name = productTabItem.Name,
                        ProductTabId = productTabItem.ProductTabId
                    };
                    return PrepareProductTabItemModel(productTabItemModel, productTabItem, productTab, true);
                });
            });

            return model;
        }

        public ProductTabItemModel PrepareProductTabItemModel(ProductTabItemModel model, ProductTabItem productTabItem,
            ProductTab productTab, bool excludeProperties = false)
        {
            Action<ProductTabItemLocalizedModel, int> localizedModelConfiguration = null;

            if (productTabItem != null)
            {
                if (!excludeProperties)
                {
                    localizedModelConfiguration = (locale, languageId) =>
                    {
                        locale.Name = _localizationService.GetLocalized(productTabItem, entity => entity.Name, languageId, false, false);
                    };
                }
            }
            else
            {
                if (!excludeProperties)
                {
                    try
                    {
                        model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
                    }
                    catch (Exception ex)
                    {
                        string myException = ex.InnerException.Message;
                    }
                }
            }

            return model;
        }
        #endregion

        #region Product tab items

        public ProductTabItemProductListModel PrepareProductTabItemProductListModel(ProductTabItemProductSearchModel searchModel, ProductTabItem productTabItem)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productTabItem == null)
                throw new ArgumentNullException(nameof(productTabItem));

            var productTabItemProducts = _productTabService.GetProductTabItemProductsByProductTabItemId(productTabItem.Id).OrderBy(x => x.DisplayOrder).ToList().ToPagedList(searchModel);
            //productTabItem.ProductTabItemProducts.OrderBy(x => x.DisplayOrder).ToList().ToPagedList(searchModel);


            //prepare grid model
            var model = new ProductTabItemProductListModel().PrepareToGrid(searchModel, productTabItemProducts, () =>
            {
                //fill in model values from the entity
                return productTabItemProducts.Select(product =>
                {
                    return PrepareProductTabItemProductModel(null, product, productTabItem);
                });
            });

            return model;
        }

        protected ProductTabItemProductModel PrepareProductTabItemProductModel(ProductTabItemProductModel model, ProductTabItemProduct itemProduct,
            ProductTabItem productTabItem)
        {
            if (itemProduct != null)
            {
                if (model == null)
                {
                    model = itemProduct.ToModel<ProductTabItemProductModel>();
                    var product = _productService.GetProductById(itemProduct.ProductId);
                    model.ProductName = product?.Name;
                }
            }

            return model;
        }

        public AddProductToProductTabItemSearchModel PrepareAddProductToProductTabItemSearchModel(AddProductToProductTabItemSearchModel searchModel)
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

        public AddProductToProductTabItemListModel PrepareAddProductToProductTabItemListModel(AddProductToProductTabItemSearchModel searchModel)
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
            var model = new AddProductToProductTabItemListModel().PrepareToGrid(searchModel, products, () =>
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
