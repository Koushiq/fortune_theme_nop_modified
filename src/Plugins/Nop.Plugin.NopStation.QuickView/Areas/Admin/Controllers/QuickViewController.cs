using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.QuickView.Areas.Admin.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Core;
using Nop.Services.Security;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Services.Plugins;

namespace Nop.Plugin.NopStation.QuickView.Areas.Admin.Controllers
{
    [NopStationLicense]
    public partial class QuickViewController : BaseAdminController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IPluginService _pluginService;

        #endregion

        #region Ctor

        public QuickViewController(IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IWorkContext workContext,
            IPluginService pluginService)
        {
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
            _pluginService = pluginService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(QuickViewPermissionProvider.ManageQuickView))
                return AccessDeniedView();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var quickViewSettings = _settingService.LoadSetting<QuickViewSettings>(storeId);

            var model = quickViewSettings.ToSettingsModel<ConfigurationModel>();

            var pluginDescriptor = _pluginService.GetPluginDescriptorBySystemName<IPlugin>("NopStation.PictureZoom",
                LoadPluginsMode.InstalledOnly, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
            model.PictureZoomPluginInstalled = pluginDescriptor != null;

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return View(model);

            model.ShowAlsoPurchasedProducts_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowAlsoPurchasedProducts, storeId);
            model.ShowRelatedProducts_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowRelatedProducts, storeId);
            model.ShowAddToWishlistButton_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowAddToWishlistButton, storeId);
            model.ShowAvailability_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowAvailability, storeId);
            model.ShowCompareProductsButton_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowCompareProductsButton, storeId);
            model.ShowDeliveryInfo_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowDeliveryInfo, storeId);
            model.ShowFullDescription_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowFullDescription, storeId);
            model.ShowProductEmailAFriendButton_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowProductEmailAFriendButton, storeId);
            model.ShowProductManufacturers_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowProductManufacturers, storeId);
            model.ShowProductReviewOverview_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowProductReviewOverview, storeId);
            model.ShowProductSpecifications_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowProductSpecifications, storeId);
            model.ShowProductTags_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowProductTags, storeId);
            model.ShowRelatedProducts_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowRelatedProducts, storeId);
            model.ShowShortDescription_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.ShowShortDescription, storeId);
            model.EnablePictureZoom_OverrideForStore = _settingService.SettingExists(quickViewSettings, x => x.EnablePictureZoom, storeId);

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(QuickViewPermissionProvider.ManageQuickView))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var quickViewSettings = _settingService.LoadSetting<QuickViewSettings>(storeScope);

            quickViewSettings = model.ToSettings(quickViewSettings);

            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowAlsoPurchasedProducts, model.ShowAlsoPurchasedProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowRelatedProducts, model.ShowRelatedProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowAddToWishlistButton, model.ShowAddToWishlistButton_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowAvailability, model.ShowAvailability_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowCompareProductsButton, model.ShowCompareProductsButton_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowDeliveryInfo, model.ShowDeliveryInfo_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowFullDescription, model.ShowFullDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowProductEmailAFriendButton, model.ShowProductEmailAFriendButton_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowProductManufacturers, model.ShowProductManufacturers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowProductReviewOverview, model.ShowProductReviewOverview_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowProductSpecifications, model.ShowProductSpecifications_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowProductTags, model.ShowProductTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowRelatedProducts, model.ShowRelatedProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.ShowShortDescription, model.ShowShortDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(quickViewSettings, x => x.EnablePictureZoom, model.EnablePictureZoom_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.QuickView.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion
    }
}
