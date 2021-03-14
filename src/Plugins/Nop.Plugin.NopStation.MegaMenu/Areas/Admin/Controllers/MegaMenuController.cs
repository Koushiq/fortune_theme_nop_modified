using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.NopStation.MegaMenu.Areas.Admin.Models;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Core;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.MegaMenu.Infrastructure.Cache;

namespace Nop.Plugin.NopStation.MegaMenu.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class MegaMenuController : BaseAdminController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly MegaMenuSettings _megaMenuSettings;
        private readonly ICategoryService _categoryService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _cachecManager;

        #endregion

        #region Ctor

        public MegaMenuController(IStoreContext storeContext,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            MegaMenuSettings megaMenuSettings,
            ICategoryService categoryService,
            ISettingService settingService,
            IStaticCacheManager cacheManager)
        {
            _storeContext = storeContext;
            _baseAdminModelFactory = baseAdminModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _manufacturerService = manufacturerService;
            _permissionService = permissionService;
            _megaMenuSettings = megaMenuSettings;
            _categoryService = categoryService;
            _settingService = settingService;
            _cachecManager = cacheManager;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(MegaMenuPermissionProvider.ManageMegaMenu))
                return AccessDeniedView();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(storeId);

            var model = megaMenuSettings.ToSettingsModel<ConfigurationModel>();

            _baseAdminModelFactory.PrepareCategories(model.AvailableCategories, false);
            _baseAdminModelFactory.PrepareManufacturers(model.AvailableManufacturers, false);

            if (!string.IsNullOrWhiteSpace(_megaMenuSettings.SelectedManufacturerIds))
                model.SelectedManufacturerIds = _megaMenuSettings.SelectedManufacturerIds.Split(',').Select(int.Parse).ToList();

            if (!string.IsNullOrWhiteSpace(_megaMenuSettings.SelectedCategoryIds))
                model.SelectedCategoryIds = _megaMenuSettings.SelectedCategoryIds.Split(',').Select(int.Parse).ToList();

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return View(model);

            model.EnableMegaMenu_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.EnableMegaMenu, storeId);
            model.HideManufacturers_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.HideManufacturers, storeId);
            model.MaxCategoryLevelsToShow_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.MaxCategoryLevelsToShow, storeId);
            model.SelectedCategoryIds_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.SelectedCategoryIds, storeId);
            model.SelectedManufacturerIds_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.SelectedManufacturerIds, storeId);
            model.ShowCategoryPicture_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.ShowCategoryPicture, storeId);
            model.ShowNumberOfCategoryProductsIncludeSubcategories_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.ShowNumberOfCategoryProductsIncludeSubcategories, storeId);
            model.ShowNumberofCategoryProducts_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.ShowNumberOfCategoryProducts, storeId);
            model.ShowManufacturerPicture_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.ShowManufacturerPicture, storeId);
            model.ShowSubcategoryPicture_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.ShowSubcategoryPicture, storeId);
            model.DefaultCategoryIconId_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.DefaultCategoryIconId, storeId);

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(MegaMenuPermissionProvider.ManageMegaMenu))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(storeScope);

            megaMenuSettings = model.ToSettings(megaMenuSettings);
            megaMenuSettings.SelectedCategoryIds = string.Join(",", model.SelectedCategoryIds);
            megaMenuSettings.SelectedManufacturerIds = string.Join(",", model.SelectedManufacturerIds);

            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.EnableMegaMenu, model.EnableMegaMenu_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.HideManufacturers, model.HideManufacturers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.MaxCategoryLevelsToShow, model.MaxCategoryLevelsToShow_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.SelectedCategoryIds, model.SelectedCategoryIds_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.SelectedManufacturerIds, model.SelectedManufacturerIds_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.ShowCategoryPicture, model.ShowCategoryPicture_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.ShowNumberOfCategoryProductsIncludeSubcategories, model.ShowNumberOfCategoryProductsIncludeSubcategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.ShowNumberOfCategoryProducts, model.ShowNumberofCategoryProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.ShowManufacturerPicture, model.ShowManufacturerPicture_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.ShowSubcategoryPicture, model.ShowSubcategoryPicture_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(megaMenuSettings, x => x.DefaultCategoryIconId, model.DefaultCategoryIconId_OverrideForStore, storeScope, false);

            _settingService.ClearCache();
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.MegaMenu.Configuration.Updated"));
            _cachecManager.RemoveByPrefix(MegaMenuModelCacheEventConsumer.MEGAMENU_PATERN_KEY);

            return RedirectToAction("Configure");
        }

        #endregion
    }
}
