using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.NopStation.OCarousels.Areas.Admin.Models;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.NopStation.OCarousels.Domains;
using Nop.Plugin.NopStation.OCarousels.Services;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.NopStation.OCarousels.Areas.Admin.Factories;
using System;
using Nop.Web.Framework.Mvc;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Services.Security;
using Nop.Core;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Services.Stores;

namespace Nop.Plugin.NopStation.OCarousels.Areas.Admin.Controllers
{
    [NopStationLicense]
    public partial class OCarouselController : BaseAdminController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IOCarouselModelFactory _carouselModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly OCarouselSettings _carouselSetting;
        private readonly IOCarouselService _carouselService;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public OCarouselController(IStoreContext storeContext,
            ILocalizedEntityService localizedEntityService,
            IOCarouselModelFactory carouselModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            OCarouselSettings carouselSetting,
            IOCarouselService carouselService,
            ISettingService settingService,
            IProductService productService,
            IStoreService storeService)
        {
            _storeContext = storeContext;
            _localizedEntityService = localizedEntityService;
            _carouselModelFactory = carouselModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _storeMappingService = storeMappingService;
            _permissionService = permissionService;
            _carouselSetting = carouselSetting;
            _carouselService = carouselService;
            _settingService = settingService;
            _productService = productService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual void SaveStoreMappings(OCarousel carousel, OCarouselModel model)
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

        protected virtual void UpdateLocales(OCarousel oCarousel, OCarouselModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(oCarousel,
                        x => x.Title,
                        localized.Title,
                        localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        #region Configure

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            var model = _carouselModelFactory.PrepareConfigurationModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();
            
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var carouselSettings = _settingService.LoadSetting<OCarouselSettings>(storeScope);
            carouselSettings = model.ToSettings(carouselSettings);
            
            _settingService.SaveSettingOverridablePerStore(carouselSettings, x => x.EnableOCarousel, model.EnableOCarousel_OverrideForStore, storeScope, false);
            
            _settingService.ClearCache();
            
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.OCarousels.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion

        #region List

        public IActionResult List()
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            var model = _carouselModelFactory.PrepareOCarouselSearchModel(new OCarouselSearchModel());

            return View(model);
        }

        [HttpPost]
        public IActionResult List(OCarouselSearchModel searchModel)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedDataTablesJson();

            var model = _carouselModelFactory.PrepareOCarouselListModel(searchModel);
            return Json(model);
        }

        #endregion

        #region Create/update/delete

        public IActionResult Create()
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            var model = _carouselModelFactory.PrepareOCarouselModel(new OCarouselModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(OCarouselModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var carousel = model.ToEntity<OCarousel>();
                carousel.CreatedOnUtc = DateTime.UtcNow;
                carousel.UpdatedOnUtc = DateTime.UtcNow;

                _carouselService.InsertCarousel(carousel);

                UpdateLocales(carousel, model);

                SaveStoreMappings(carousel, model);

                _carouselService.UpdateCarousel(carousel);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.OCarousels.Created"));

                return continueEditing ?
                    RedirectToAction("Edit", new { id = carousel.Id }) :
                    RedirectToAction("List");
            }

            model = _carouselModelFactory.PrepareOCarouselModel(model, null);

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            var carousel = _carouselService.GetCarouselById(id);
            if (carousel == null || carousel.Deleted)
                return RedirectToAction("List");

            var model = _carouselModelFactory.PrepareOCarouselModel(null, carousel);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(OCarouselModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            var carousel = _carouselService.GetCarouselById(model.Id);
            if (carousel == null || carousel.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                carousel = model.ToEntity(carousel);
                carousel.UpdatedOnUtc = DateTime.UtcNow;

                _carouselService.UpdateCarousel(carousel);

                UpdateLocales(carousel, model);

                SaveStoreMappings(carousel, model);

                _carouselService.UpdateCarousel(carousel);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.OCarousels.Updated"));

                return continueEditing ?
                    RedirectToAction("Edit", new { id = model.Id }) :
                    RedirectToAction("List");
            }

            model = _carouselModelFactory.PrepareOCarouselModel(model, carousel);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(OCarouselModel model)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            var carousel = _carouselService.GetCarouselById(model.Id);
            if (carousel == null || carousel.Deleted)
                return RedirectToAction("List");

            _carouselService.DeleteCarousel(carousel);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.OCarousels.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Ocarousel items

        [HttpPost]
        public virtual IActionResult OCarouselItemList(OCarouselItemSearchModel searchModel)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedDataTablesJson();

            //try to get a carousel with the specified id
            var carousel = _carouselService.GetCarouselById(searchModel.OCarouselId);
            if (carousel == null || carousel.Deleted)
                return new NullJsonResult();

            //prepare model
            var model = _carouselModelFactory.PrepareOCarouselItemListModel(searchModel, carousel);

            return Json(model);
        }

        public virtual IActionResult OCarouselItemEdit(OCarouselItemModel model)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedDataTablesJson();

            var carousel = _carouselService.GetCarouselById(model.OCarouselId);
            if (carousel == null || carousel.Deleted)
                return new NullJsonResult();

            var carouselItem = _carouselService.GetOCarouselItemsByOCarouselId(carousel.Id).FirstOrDefault(x => x.Id == model.Id)
                ?? throw new ArgumentException("No carousel item found with the specified id", nameof(model.Id));

            //remove carousel item
            carouselItem.DisplayOrder = model.DisplayOrder;
            _carouselService.UpdateOCarouselItem(carouselItem);

            return new NullJsonResult();
        }

        public virtual IActionResult OCarouselItemDelete(int ocarouselId, int id)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedDataTablesJson();

            var carousel = _carouselService.GetCarouselById(ocarouselId);
            if (carousel == null || carousel.Deleted)
                return new NullJsonResult();

            var carouselItem = _carouselService.GetOCarouselItemsByOCarouselId(carousel.Id).FirstOrDefault(x => x.Id == id)
                ?? throw new ArgumentException("No carousel item found with the specified id", nameof(id));

            //remove carousel item
            _carouselService.DeleteOCarouselItem(carouselItem);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductAddPopup(int ocarouselId)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            var carousel = _carouselService.GetCarouselById(ocarouselId)
                ?? throw new ArgumentException("No carousel found with the specified id");

            if (carousel.Deleted)
                throw new ArgumentException("No carousel found with the specified id");

            var model = _carouselModelFactory.PrepareAddProductToOCarouselSearchModel(new AddProductToCarouselSearchModel());
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAddPopupList(AddProductToCarouselSearchModel searchModel)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedDataTablesJson();

            var model = _carouselModelFactory.PrepareAddProductToOCarouselListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult ProductAddPopup(AddProductToCarouselModel model)
        {
            if (!_permissionService.Authorize(OCarouselPermissionProvider.ManageOCarousels))
                return AccessDeniedView();

            var carousel = _carouselService.GetCarouselById(model.OCarouselId)
                ?? throw new ArgumentException("No carousel found with the specified id");

            if (carousel.Deleted)
                throw new ArgumentException("No carousel found with the specified id");

            var carouselItems = _carouselService.GetOCarouselItemsByOCarouselId(model.OCarouselId)
                ?? throw new ArgumentException("No carousel item found with the specified id", nameof(model.OCarouselId));

            var selectedProducts = _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (!carouselItems.Any(x => x.ProductId == product.Id))
                    {
                        _carouselService.InsertOCarouselItem(new OCarouselItem()
                        {
                            DisplayOrder = 0,
                            OCarouselId = model.OCarouselId,
                            ProductId = product.Id,
                        });
                    }
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToCarouselSearchModel());
        }

        #endregion

        #endregion
    }
}
