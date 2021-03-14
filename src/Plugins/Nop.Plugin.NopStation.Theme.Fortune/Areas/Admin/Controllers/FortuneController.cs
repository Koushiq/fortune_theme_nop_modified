using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.Theme.Fortune.Areas.Admin.Models;
using Nop.Plugin.NopStation.Theme.Fortune.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Themes;

namespace Nop.Plugin.NopStation.Theme.Fortune.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class FortuneController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly IThemeContext _themeContext;
        private readonly INopFileProvider _nopFileProvider;

        #endregion

        #region Ctor

        public FortuneController(IPermissionService permissionService,
            IWorkContext workContext,
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ILogger logger,
            IThemeContext themeContext,
            INopFileProvider nopFileProvider)
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _settingService = settingService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _logger = logger;
            _themeContext = themeContext;
            _nopFileProvider = nopFileProvider;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var fortuneSettings = _settingService.LoadSetting<FortuneSettings>(storeId);
            var model = fortuneSettings.ToSettingsModel<ConfigurationModel>();

            var defaultColor = fortuneSettings.CustomThemeColor;
            if (string.IsNullOrEmpty(defaultColor))
            {
                model.CustomThemeColor = FortuneThemeDefaults.DefaultThemeColor;
            }

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return View(model);

            model.CustomThemeColor_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.CustomThemeColor, storeId);
            model.CustomCss_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.CustomCss, storeId);
            model.EnableImageLazyLoad_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.EnableImageLazyLoad, storeId);
            model.FooterEmail_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.FooterEmail, storeId);
            model.FooterLogoPictureId_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.FooterLogoPictureId, storeId);
            model.LazyLoadPictureId_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.LazyLoadPictureId, storeId);
            model.ShowLogoAtPageFooter_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.ShowLogoAtPageFooter, storeId);
            model.ShowSupportedCardsPictureAtPageFooter_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.ShowSupportedCardsPictureAtPageFooter, storeId);
            model.SupportedCardsPictureId_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.SupportedCardsPictureId, storeId);

            #region  Description Box Settings
            model.EnableDescriptionBoxOne_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.EnableDescriptionBoxOne, storeId);
            model.DescriptionBoxOneTitle_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxOneTitle, storeId);
            model.DescriptionBoxOneText_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxOneText, storeId);
            model.DescriptionBoxOneUrl_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxOneUrl, storeId);
            model.DescriptionBoxOnePictureId_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxOnePictureId, storeId);

            model.EnableDescriptionBoxTwo_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.EnableDescriptionBoxTwo, storeId);
            model.DescriptionBoxTwoTitle_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxTwoTitle, storeId);
            model.DescriptionBoxTwoText_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxTwoText, storeId);
            model.DescriptionBoxTwoUrl_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxTwoUrl, storeId);
            model.DescriptionBoxTwoPictureId_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxTwoPictureId, storeId);

            model.EnableDescriptionBoxThree_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.EnableDescriptionBoxThree, storeId);
            model.DescriptionBoxThreeTitle_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxThreeTitle, storeId);
            model.DescriptionBoxThreeText_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxThreeText, storeId);
            model.DescriptionBoxThreeUrl_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxThreeUrl, storeId);
            model.DescriptionBoxThreePictureId_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxThreePictureId, storeId);

            model.EnableDescriptionBoxFour_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.EnableDescriptionBoxFour, storeId);
            model.DescriptionBoxFourTitle_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxFourTitle, storeId);
            model.DescriptionBoxFourText_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxFourText, storeId);
            model.DescriptionBoxFourUrl_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxFourUrl, storeId);
            model.DescriptionBoxFourPictureId_OverrideForStore = _settingService.SettingExists(fortuneSettings, x => x.DescriptionBoxFourPictureId, storeId);
            #endregion

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var fortuneSettings = _settingService.LoadSetting<FortuneSettings>(storeScope);

            if (fortuneSettings.CustomThemeColor != model.CustomThemeColor)
            {
                if (string.IsNullOrEmpty(model.CustomThemeColor))
                {
                    ReplaceThemeColor(FortuneThemeDefaults.DefaultThemeColor);
                }
                else
                {
                    var hexPattern = new Regex("^#([A-Fa-f0-9]{6})$");
                    var color = hexPattern.Match(model.CustomThemeColor);
                    if (color.Length > 0)
                    {
                        var colorHexCode = color.Value;
                        ReplaceThemeColor(colorHexCode);
                    }
                }
            }
            
            fortuneSettings = model.ToSettings(fortuneSettings);

            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.CustomThemeColor, model.CustomThemeColor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.CustomCss, model.CustomCss_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.EnableImageLazyLoad, model.EnableImageLazyLoad_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.FooterEmail, model.FooterEmail_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.FooterLogoPictureId, model.FooterLogoPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.LazyLoadPictureId, model.LazyLoadPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.ShowLogoAtPageFooter, model.ShowLogoAtPageFooter_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.ShowSupportedCardsPictureAtPageFooter, model.ShowSupportedCardsPictureAtPageFooter_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.SupportedCardsPictureId, model.SupportedCardsPictureId_OverrideForStore, storeScope, false);

            #region Footer Description Box Settings
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.EnableDescriptionBoxOne, model.EnableDescriptionBoxOne_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxOneTitle, model.DescriptionBoxOneTitle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxOneText, model.DescriptionBoxOneText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxOneUrl, model.DescriptionBoxOneUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxOnePictureId, model.DescriptionBoxOnePictureId_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.EnableDescriptionBoxTwo, model.EnableDescriptionBoxTwo_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxTwoTitle, model.DescriptionBoxTwoTitle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxTwoText, model.DescriptionBoxTwoText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxTwoUrl, model.DescriptionBoxTwoUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxTwoPictureId, model.DescriptionBoxTwoPictureId_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.EnableDescriptionBoxThree, model.EnableDescriptionBoxThree_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxThreeTitle, model.DescriptionBoxThreeTitle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxThreeText, model.DescriptionBoxThreeText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxThreeUrl, model.DescriptionBoxThreeUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxThreePictureId, model.DescriptionBoxThreePictureId_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.EnableDescriptionBoxFour, model.EnableDescriptionBoxFour_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxFourTitle, model.DescriptionBoxFourTitle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxFourText, model.DescriptionBoxFourText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxFourUrl, model.DescriptionBoxFourUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(fortuneSettings, x => x.DescriptionBoxFourPictureId, model.DescriptionBoxFourPictureId_OverrideForStore, storeScope, false);
            #endregion

            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.Theme.Fortune.Configuration.Updated"));

            UpdateLocales(model);

            return RedirectToAction("Configure");
        }

        private void UpdateLocales(ConfigurationModel model)
        {
            var storeScope = _storeContext.CurrentStore.Id;
            if (model.Locales.Count == 0)
            {
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTitleValue", model.DescriptionBoxOneTitle, storeScope, model.DescriptionBoxOneTitle_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTextValue", model.DescriptionBoxOneText, storeScope, model.DescriptionBoxOneText_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTitleValue", model.DescriptionBoxTwoTitle, storeScope, model.DescriptionBoxTwoTitle_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTextValue", model.DescriptionBoxTwoText, storeScope, model.DescriptionBoxTwoText_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTitleValue", model.DescriptionBoxThreeTitle, storeScope, model.DescriptionBoxThreeTitle_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTextValue", model.DescriptionBoxThreeText, storeScope, model.DescriptionBoxThreeText_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTitleValue", model.DescriptionBoxFourTitle, storeScope, model.DescriptionBoxFourTitle_OverrideForStore);
                SetDefaultLocaleStringResource("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTextValue", model.DescriptionBoxFourText, storeScope, model.DescriptionBoxFourText_OverrideForStore);
            }
            foreach (var locale in model.Locales)
            {
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTitleValue", locale.DescriptionBoxOneTitle, model.DescriptionBoxOneTitle, storeScope, model.DescriptionBoxOneTitle_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTextValue", locale.DescriptionBoxOneText, model.DescriptionBoxOneText, storeScope, model.DescriptionBoxOneText_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTitleValue", locale.DescriptionBoxTwoTitle, model.DescriptionBoxTwoTitle, storeScope, model.DescriptionBoxTwoTitle_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTextValue", locale.DescriptionBoxTwoText, model.DescriptionBoxTwoText, storeScope, model.DescriptionBoxTwoText_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTitleValue", locale.DescriptionBoxThreeTitle, model.DescriptionBoxThreeTitle, storeScope, model.DescriptionBoxThreeTitle_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTextValue", locale.DescriptionBoxThreeText, model.DescriptionBoxThreeText, storeScope, model.DescriptionBoxThreeText_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTitleValue", locale.DescriptionBoxFourTitle, model.DescriptionBoxFourTitle, storeScope, model.DescriptionBoxFourTitle_OverrideForStore);
                SetLocaleStringResource(locale.LanguageId, "Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTextValue", locale.DescriptionBoxFourText, model.DescriptionBoxFourText, storeScope, model.DescriptionBoxFourText_OverrideForStore);
            }
        }

        private void SetLocaleStringResource(int languageId, string resourceName, string resourceValue, string settingValue, int storeScope = 0, bool saveStoreSpecificValue = false)
        {
            if (storeScope > 0)
            {
                if (saveStoreSpecificValue)
                {
                    resourceName = $"{resourceName}-{storeScope}";
                }
                else
                {
                    var localeString = _localizationService.GetLocaleStringResourceByName($"{resourceName}-{storeScope}", languageId, false);
                    if (localeString != null && localeString.Id > 0)
                    {
                        _localizationService.DeleteLocaleStringResource(localeString);
                    }
                }
            }
            var localizedResource = _localizationService.GetLocaleStringResourceByName(resourceName, languageId, false);
            if (string.IsNullOrEmpty(settingValue))
            {
                settingValue = string.Empty;
            }
            if (string.IsNullOrEmpty(resourceValue))
            {
                resourceValue = settingValue;
            }
            if (localizedResource == null)
            {
                localizedResource = new LocaleStringResource
                {
                    LanguageId = languageId,
                    ResourceName = resourceName,
                    ResourceValue = resourceValue
                };
                _localizationService.InsertLocaleStringResource(localizedResource);
            }
            else
            {
                localizedResource.ResourceValue = resourceValue;
                _localizationService.UpdateLocaleStringResource(localizedResource);
            }
        }

        private void SetDefaultLocaleStringResource(string resourceName, string settingValue, int storeScope = 0, bool saveStoreSpecificValue = false)
        {
            if (storeScope > 0 && saveStoreSpecificValue)
            {
                resourceName = $"{resourceName}-{storeScope}";
            }
            var localizedResource = _localizationService.GetLocaleStringResourceByName(resourceName);
            if (string.IsNullOrEmpty(settingValue))
            {
                settingValue = string.Empty;
            }
            if (localizedResource == null)
            {
                localizedResource = new LocaleStringResource
                {
                    LanguageId = _workContext.WorkingLanguage.Id,
                    ResourceName = resourceName,
                    ResourceValue = settingValue
                };
                _localizationService.InsertLocaleStringResource(localizedResource);
            }
            else
            {
                localizedResource.ResourceValue = settingValue;
                _localizationService.UpdateLocaleStringResource(localizedResource);
            }
        }

        protected void ReplaceThemeColor(string colorHexCode)
        {
            try
            {
                var workingThemeName = _themeContext.WorkingThemeName;
                var colorCssFile = $"~/Themes/{workingThemeName}/Content/css/color.css";
                var fileSystemPath = _nopFileProvider.MapPath(colorCssFile);
                if (_nopFileProvider.FileExists(fileSystemPath))
                {
                    var text = System.IO.File.ReadAllText(fileSystemPath);
                    text = Regex.Replace(text, "#([A-Fa-f0-9]{6});", $"{colorHexCode};");
                    System.IO.File.WriteAllText(fileSystemPath, text);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Could not save theme color", e);
            }
        }

        #endregion
    }
}
