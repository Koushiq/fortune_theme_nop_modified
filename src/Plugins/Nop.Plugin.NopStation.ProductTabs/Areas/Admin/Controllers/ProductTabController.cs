using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Factories;
using Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Models;
using Nop.Plugin.NopStation.ProductTabs.Domains;
using Nop.Plugin.NopStation.ProductTabs.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.NopStation.ProductTabs.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class ProductTabController : BaseAdminController
    {
        #region Fields

        private readonly ISettingService _settingsService;
        private readonly ProductTabSettings _productTabSetting;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IProductTabService _productTabService;
        private readonly IProductTabModelFactory _productTabModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizedEntityService _localizedEntityService;

        #endregion

        #region Ctor

        public ProductTabController(ISettingService settingsService,
            ProductTabSettings productTabSetting,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IProductTabService productTabService,
            IProductTabModelFactory productTabModelFactory,
            IPermissionService permissionService,
            IProductService productService,
            IStoreContext storeContext,
            ISettingService settingService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ILocalizedEntityService localizedEntityService)
        {
            _settingsService = settingsService;
            _productTabSetting = productTabSetting;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _productTabService = productTabService;
            _productTabModelFactory = productTabModelFactory;
            _permissionService = permissionService;
            _productService = productService;
            _storeContext = storeContext;
            _settingService = settingService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _localizedEntityService = localizedEntityService;
        }

        #endregion

        #region Utilities

        protected virtual void SaveStoreMappings(ProductTab carousel, ProductTabModel model)
        {
            carousel.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(carousel);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(carousel, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        protected virtual void UpdateLocales(ProductTab productTab, ProductTabModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productTab,
                        x => x.Name,
                        localized.Name,
                        localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(productTab,
                        x => x.TabTitle,
                        localized.TabTitle,
                        localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(ProductTabItem productTabItem, ProductTabItemModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productTabItem,
                        x => x.Name,
                        localized.Name,
                        localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        #region Configure

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var carouselSettings = _settingService.LoadSetting<ProductTabSettings>(storeId);

            var model = carouselSettings.ToSettingsModel<ConfigurationModel>();

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return View(model);

            model.EnableProductTab_OverrideForStore = _settingService.SettingExists(carouselSettings, x => x.EnableProductTab, storeId);

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var productTabSettings = _settingService.LoadSetting<ProductTabSettings>(storeScope);
            productTabSettings = model.ToSettings(productTabSettings);

            _settingService.SaveSettingOverridablePerStore(productTabSettings, x => x.EnableProductTab, model.EnableProductTab_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("NopStation.ProductTabs.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion

        #region List

        public IActionResult List()
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var model = _productTabModelFactory.PrepareOCarouselSearchModel(new ProductTabSearchModel());

            return View(model);
        }

        [HttpPost]
        public IActionResult List(ProductTabSearchModel searchModel)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedDataTablesJson();

            var model = _productTabModelFactory.PrepareProductTabListModel(searchModel);
            return Json(model);
        }

        #endregion

        #region Create/update/delete

        public IActionResult Create()
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var model = _productTabModelFactory.PrepareProductTabModel(new ProductTabModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ProductTabModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {


                var productTab = model.ToEntity<ProductTab>();
                model.CreatedOn = DateTime.UtcNow;
                model.UpdatedOn = DateTime.UtcNow;
                productTab.CreatedOnUtc = DateTime.UtcNow;
                productTab.UpdatedOnUtc = DateTime.UtcNow;

                _productTabService.InsertProductTab(productTab);
                UpdateLocales(productTab, model);

                SaveStoreMappings(productTab, model);
                _productTabService.UpdateProductTab(productTab);

                _notificationService.SuccessNotification(_localizationService.GetResource("NopStation.ProductTabs.ProductTabs.Created"));

                return continueEditing ?
                    RedirectToAction("Edit", new { id = productTab.Id }) :
                    RedirectToAction("List");
            }

            model = _productTabModelFactory.PrepareProductTabModel(model, null);
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTab = _productTabService.GetProductTabById(id);
            if (productTab == null || productTab.Deleted)
                return RedirectToAction("List");

            var model = _productTabModelFactory.PrepareProductTabModel(null, productTab);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductTabModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTab = _productTabService.GetProductTabById(model.Id);
            if (productTab == null || productTab.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                model.UpdatedOn = DateTime.UtcNow;
                productTab = model.ToEntity(productTab);
                productTab.UpdatedOnUtc = DateTime.UtcNow;
                _productTabService.UpdateProductTab(productTab);
                UpdateLocales(productTab, model);

                SaveStoreMappings(productTab, model);
                _productTabService.UpdateProductTab(productTab);

                _notificationService.SuccessNotification(_localizationService.GetResource("NopStation.ProductTabs.ProductTabs.Updated"));

                return continueEditing ?
                    RedirectToAction("Edit", new { id = model.Id }) :
                    RedirectToAction("List");
            }

            model = _productTabModelFactory.PrepareProductTabModel(model, productTab);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(ProductTabModel model)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTab = _productTabService.GetProductTabById(model.Id);
            if (productTab == null || productTab.Deleted)
                return RedirectToAction("List");

            _productTabService.DeleteProductTab(productTab);
            return RedirectToAction("List");
        }

        #endregion

        #region Product tab items

        [HttpPost]
        public virtual IActionResult ItemList(ProductTabItemSearchModel searchModel)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedDataTablesJson();

            //try to get a productTab with the specified id
            var productTab = _productTabService.GetProductTabById(searchModel.ProductTabId);
            if (productTab == null || productTab.Deleted)
                return new NullJsonResult();

            //prepare model
            var model = _productTabModelFactory.PrepareProductTabItemListModel(searchModel, productTab);

            return Json(model);
        }

        public virtual IActionResult ItemCreate(int productTabId)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTab = _productTabService.GetProductTabById(productTabId);
            if (productTab == null || productTab.Deleted)
                return RedirectToAction("List");

            var model = new ProductTabItemModel();
            if (productTab != null)
            {
                model.Name = productTab.Name;
                model.ProductTabId = productTab.Id;
            }
            var viewModel = _productTabModelFactory.PrepareProductTabItemModel(model, null, productTab);

            return View(viewModel);

        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ItemCreate(ProductTabItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTab = _productTabService.GetProductTabById(model.ProductTabId);
            if (productTab == null || productTab.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //var tabItem = model.ToEntity<ProductTabItem>();
                try
                {
                    ProductTabItem tabItem = new ProductTabItem();
                    tabItem.Name = model.Name;
                    tabItem.DisplayOrder = model.DisplayOrder;
                    tabItem.ProductTabId = model.ProductTabId;
                    
                    _productTabService.InsertProductTabItem(tabItem);
                    UpdateLocales(tabItem, model);

                    _notificationService.SuccessNotification(_localizationService.GetResource("NopStation.ProductTabs.ProductTabItems.Created"));

                    return continueEditing ?
                        RedirectToAction("ItemEdit", new { id = tabItem.Id }) :
                        RedirectToAction("Edit", new { id = productTab.Id });
                }

                catch (Exception ex)
                {
                    string helloException = ex.InnerException.Message;
                }
            }

            model = _productTabModelFactory.PrepareProductTabItemModel(model, null, productTab);

            return View(model);
        }

        public virtual IActionResult ItemEdit(int id)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTabItem = _productTabService.GetProductTabItemById(id);
            if (productTabItem == null)
                return RedirectToAction("List");

            var productTabItemModel=new ProductTabItemModel();

            productTabItemModel.Id =id;
            productTabItemModel.Name = productTabItem.Name;
            productTabItemModel.DisplayOrder = productTabItem.DisplayOrder;
            productTabItemModel.ProductTabId = productTabItem.ProductTabId;
            var model = _productTabModelFactory.PrepareProductTabItemModel(productTabItemModel, productTabItem, productTabItem.ProductTab);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ItemEdit(ProductTabItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTab = _productTabService.GetProductTabById(model.ProductTabId);
            if (productTab == null || productTab.Deleted)
                return RedirectToAction("List");

            var productTabItem = _productTabService.GetProductTabItemById(model.Id);
            if (productTabItem == null)
                return RedirectToAction("Edit", new { id = productTab.Id });

            if (ModelState.IsValid)
            {
                productTabItem = model.ToEntity(productTabItem);

                _productTabService.UpdateProductTab(productTab);
                _productTabService.UpdateProductTabItem(productTabItem);

                UpdateLocales(productTabItem, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("NopStation.ProductTabs.ProductTabItems.Updated"));

                return continueEditing ?
                    RedirectToAction("ItemEdit", new { id = model.Id }) :
                    RedirectToAction("Edit", new { id = productTab.Id });
            }

            model = _productTabModelFactory.PrepareProductTabItemModel(model, productTabItem, productTab);

            return View(model);
        }

        public virtual IActionResult ItemDelete(int id)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedDataTablesJson();

            var productTabItem = _productTabService.GetProductTabItemById(id);
            if (productTabItem == null)
                return new NullJsonResult();

            _productTabService.DeleteProductTabItem(productTabItem);

            return new NullJsonResult();
        }

        #endregion

        #region Tab products

        public virtual IActionResult ItemProductDelete(int id)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedDataTablesJson();

            var productTabItemProduct = _productTabService.GetProductTabItemProductById(id);
            if (productTabItemProduct == null)
                return new NullJsonResult();

            _productTabService.DeleteProductTabItemProduct(productTabItemProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ItemProductUpdate(ProductTabItemProductModel model)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedDataTablesJson();

            var productTabItemProduct = _productTabService.GetProductTabItemProductById(model.Id);
            if (productTabItemProduct == null)
                return new NullJsonResult();

            productTabItemProduct.DisplayOrder = model.DisplayOrder;

            _productTabService.UpdateProductTabItemProduct(productTabItemProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ItemProductList(ProductTabItemProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedDataTablesJson();

            //try to get a productTab with the specified id
            var productTabItem = _productTabService.GetProductTabItemById(searchModel.ProductTabItemId);
            if (productTabItem == null)
                return new NullJsonResult();

            //prepare model
            var model = _productTabModelFactory.PrepareProductTabItemProductListModel(searchModel, productTabItem);

            return Json(model);
        }

        public virtual IActionResult ProductAddPopup(int productTabItemId)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTabItem = _productTabService.GetProductTabItemById(productTabItemId)
                ?? throw new ArgumentException("No product tab item found with the specified id");

            var model = _productTabModelFactory.PrepareAddProductToProductTabItemSearchModel(new AddProductToProductTabItemSearchModel());
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAddPopupList(AddProductToProductTabItemSearchModel searchModel)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedDataTablesJson();

            var model = _productTabModelFactory.PrepareAddProductToProductTabItemListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(AddProductToProductTabItemModel model)
        {
            if (!_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
                return AccessDeniedView();

            var productTabItem = _productTabService.GetProductTabItemById(model.ProductTabItemId)
                ?? throw new ArgumentException("No product tab item found with the specified id");

            var itemProducts = productTabItem.ProductTabItemProducts.ToList();

            var selectedProducts = _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (!itemProducts.Any(x => x.ProductId == product.Id))
                    {
                        var productTabItemProduct = new ProductTabItemProduct()
                        {
                            ProductTabItemId = productTabItem.Id,
                            DisplayOrder = 0,
                            ProductId = product.Id,
                        };
                        _productTabService.InsertProductTabItemProduct(productTabItemProduct);
                    }
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToProductTabItemSearchModel());
        }

        #endregion

        #endregion
    }
}
