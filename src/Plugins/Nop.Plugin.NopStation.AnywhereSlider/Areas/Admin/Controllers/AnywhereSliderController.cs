using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.NopStation.AnywhereSlider.Areas.Admin.Models;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.NopStation.AnywhereSlider.Domains;
using Nop.Plugin.NopStation.AnywhereSlider.Services;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.NopStation.AnywhereSlider.Areas.Admin.Factories;
using Nop.Services.Media;
using Nop.Web.Framework.Mvc;
using System;
using Nop.Services.Configuration;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core;
using System.Linq;
using Nop.Services.Stores;

namespace Nop.Plugin.NopStation.AnywhereSlider.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class AnywhereSliderController : BaseAdminController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ISliderModelFactory _sliderModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly AnywhereSliderSettings _sliderSettings;
        private readonly ISliderService _sliderService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public AnywhereSliderController(IStoreContext storeContext,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IStoreMappingService storeMappingService,
            ISliderModelFactory sliderModelFactory,
            IPermissionService permissionService,
            IPictureService pictureService,
            ISettingService settingService,
            AnywhereSliderSettings sliderSettings,
            ISliderService sliderService,
            IStoreService storeService)
        {
            _storeContext = storeContext;
            _localizedEntityService = localizedEntityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _storeMappingService = storeMappingService;
            _sliderModelFactory = sliderModelFactory;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _sliderSettings = sliderSettings;
            _settingService = settingService;
            _sliderService = sliderService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual void SaveStoreMappings(Slider slider, SliderModel model)
        {
            slider.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(slider);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(slider, store.Id);
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

        protected virtual void UpdateLocales(SliderItem sliderItem, SliderItemModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(sliderItem,
                    x => x.Title,
                    localized.Title,
                    localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(sliderItem,
                    x => x.ShortDescription,
                    localized.ShortDescription,
                    localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(sliderItem,
                    x => x.Link,
                    localized.Link,
                    localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(sliderItem,
                    x => x.ImageAltText,
                    localized.ImageAltText,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        #region Configuration

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            var model = _sliderModelFactory.PrepareConfigurationModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var sliderSettings = _settingService.LoadSetting<AnywhereSliderSettings>(storeScope);
            sliderSettings = model.ToSettings(sliderSettings);

            _settingService.SaveSettingOverridablePerStore(sliderSettings, x => x.EnableSlider, model.EnableSlider_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AnywhereSlider.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion

        #region List

        public IActionResult List()
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            var model = _sliderModelFactory.PrepareSliderSearchModel(new SliderSearchModel());

            return View(model);
        }

        [HttpPost]
        public IActionResult List(SliderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedDataTablesJson();

            var sliders = _sliderModelFactory.PrepareSliderListModel(searchModel);
            return Json(sliders);
        }

        #endregion

        #region Create / update / delete

        public IActionResult Create()
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            var model = _sliderModelFactory.PrepareSliderModel(new SliderModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Create(SliderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var slider = model.ToEntity<Slider>();
                slider.CreatedOnUtc = DateTime.UtcNow;
                slider.UpdatedOnUtc = DateTime.UtcNow;

                _sliderService.InsertSlider(slider);

                SaveStoreMappings(slider, model);

                _sliderService.UpdateSlider(slider);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AnywhereSlider.Sliders.Created"));

                return continueEditing
                    ? RedirectToAction("Edit", new { id = slider.Id })
                    : RedirectToAction("List");
            }
            model = _sliderModelFactory.PrepareSliderModel(model, null);
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            var slider = _sliderService.GetSliderById(id);
            if (slider == null || slider.Deleted)
                return RedirectToAction("List");

            var model = _sliderModelFactory.PrepareSliderModel(null, slider);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(SliderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            var slider = _sliderService.GetSliderById(model.Id);
            if (slider == null || slider.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                slider = model.ToEntity(slider);
                slider.UpdatedOnUtc = DateTime.UtcNow;

                _sliderService.UpdateSlider(slider);

                SaveStoreMappings(slider, model);

                _sliderService.UpdateSlider(slider);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AnywhereSlider.Sliders.Updated"));

                return continueEditing
                    ? RedirectToAction("Edit", new { id = model.Id })
                    : RedirectToAction("List");
            }

            model = _sliderModelFactory.PrepareSliderModel(model, slider);
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(SliderModel model)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            var slider = _sliderService.GetSliderById(model.Id);
            if (slider == null || slider.Deleted)
                return RedirectToAction("List");

            _sliderService.DeleteSlider(slider);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.AnywhereSlider.Sliders.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Slider items

        public virtual IActionResult SliderItemCreatePopup(int sliderId)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            var slider = _sliderService.GetSliderById(sliderId)
                ?? throw new ArgumentException("No slider found with the specified id", nameof(sliderId));

            //prepare model
            var model = _sliderModelFactory.PrepareSliderItemModel(new SliderItemModel(), slider, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult SliderItemCreatePopup(SliderItemModel model)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            //try to get a slider with the specified id
            var slider = _sliderService.GetSliderById(model.SliderId)
                ?? throw new ArgumentException("No slider found with the specified id");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var sliderItem = model.ToEntity<SliderItem>();

                _sliderService.InsertSliderItem(sliderItem);
                UpdateLocales(sliderItem, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _sliderModelFactory.PrepareSliderItemModel(model, slider, null);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult SliderItemEditPopup(int id)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            //try to get a predefined slider value with the specified id
            var sliderItem = _sliderService.GetSliderItemById(id)
                ?? throw new ArgumentException("No slider item found with the specified id");

            //try to get a slider with the specified id
            var slider = _sliderService.GetSliderById(sliderItem.SliderId)
                ?? throw new ArgumentException("No slider found with the specified id");

            //prepare model
            var model = _sliderModelFactory.PrepareSliderItemModel(null, slider, sliderItem);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult SliderItemEditPopup(SliderItemModel model)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedView();

            //try to get a predefined slider value with the specified id
            var sliderItem = _sliderService.GetSliderItemById(model.Id)
                ?? throw new ArgumentException("No slider item found with the specified id");

            //try to get a slider with the specified id
            var slider = _sliderService.GetSliderById(sliderItem.SliderId)
                ?? throw new ArgumentException("No slider found with the specified id");

            if (ModelState.IsValid)
            {
                sliderItem = model.ToEntity(sliderItem);
                _sliderService.UpdateSliderItem(sliderItem);

                UpdateLocales(sliderItem, model);
                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _sliderModelFactory.PrepareSliderItemModel(model, slider, sliderItem, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public IActionResult SliderItemDelete(int id)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedDataTablesJson();

            var sliderItem = _sliderService.GetSliderItemById(id)
                ?? throw new ArgumentException("No slider item found with the specified id");

            var slider = _sliderService.GetSliderById(sliderItem.SliderId);

            if (slider.Deleted)
                return new NullJsonResult();

            sliderItem.Slider = slider;
            _sliderService.DeleteSliderItem(sliderItem);

            return new NullJsonResult();
        }

        [HttpPost]
        public IActionResult SliderItemList(SliderItemSearchModel searchModel)
        {
            if (!_permissionService.Authorize(AnywhereSliderPermissionProvider.ManageSliders))
                return AccessDeniedDataTablesJson();

            var listModel = _sliderModelFactory.PrepareSliderItemListModel(searchModel);
            return Json(listModel);
        }

        #endregion   

        #endregion        
    }
}