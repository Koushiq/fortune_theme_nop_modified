using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.ProductRibbon.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Core;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.NopStation.ProductRibbon.Areas.Admin.Controllers
{
    [NopStationLicense]
    public partial class ProductRibbonController : BaseAdminController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ProductRibbonSettings _productRibbonSettings;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ProductRibbonController(IStoreContext storeContext,
            ProductRibbonSettings productRibbonSettings,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _storeContext = storeContext;
            _productRibbonSettings = productRibbonSettings;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(ProductRibbonPermissionProvider.ManageProductRibbon))
                return AccessDeniedView();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var productRibbonSettings = _settingService.LoadSetting<ProductRibbonSettings>(storeId);

            var model = productRibbonSettings.ToSettingsModel<ConfigurationModel>();

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return View(model);

            model.ProductDetailsPageWidgetZone_OverrideForStore = _settingService.SettingExists(productRibbonSettings, x => x.ProductDetailsPageWidgetZone, storeId);
            model.ProductOverviewBoxWidgetZone_OverrideForStore = _settingService.SettingExists(productRibbonSettings, x => x.ProductOverviewBoxWidgetZone, storeId);
            model.EnableBestSellerRibbon_OverrideForStore = _settingService.SettingExists(productRibbonSettings, x => x.EnableBestSellerRibbon, storeId);
            model.EnableNewRibbon_OverrideForStore = _settingService.SettingExists(productRibbonSettings, x => x.EnableNewRibbon, storeId);
            model.EnableDiscountRibbon_OverrideForStore = _settingService.SettingExists(productRibbonSettings, x => x.EnableDiscountRibbon, storeId);

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(ProductRibbonPermissionProvider.ManageProductRibbon))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var productRibbonSettings = _settingService.LoadSetting<ProductRibbonSettings>(storeScope);
            productRibbonSettings = model.ToSettings(productRibbonSettings);

            _settingService.SaveSettingOverridablePerStore(productRibbonSettings, x => x.ProductDetailsPageWidgetZone, model.ProductDetailsPageWidgetZone_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(productRibbonSettings, x => x.ProductOverviewBoxWidgetZone, model.ProductOverviewBoxWidgetZone_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(productRibbonSettings, x => x.EnableBestSellerRibbon, model.EnableBestSellerRibbon_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(productRibbonSettings, x => x.EnableDiscountRibbon, model.EnableDiscountRibbon_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(productRibbonSettings, x => x.EnableNewRibbon, model.EnableNewRibbon_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.ProductRibbon.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion
    }
}
