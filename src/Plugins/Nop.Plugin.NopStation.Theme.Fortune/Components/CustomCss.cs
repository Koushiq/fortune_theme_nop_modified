using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.Theme.Fortune.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Themes;

namespace Nop.Plugin.NopStation.Theme.Fortune.Components
{
    public class CustomCssViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IThemeContext _themeContext;
        private readonly CommonSettings _commonSettings;
        private readonly IStoreContext _storeContext;
        private readonly INopFileProvider _nopFileProvider;

        #endregion

        #region Ctor

        public CustomCssViewComponent(
            ILogger logger,
            ISettingService settingService,
            IThemeContext themeContext, 
            CommonSettings commonSettings,
            IStoreContext storeContext,
            INopFileProvider nopFileProvider)
        {
            _logger = logger;
            _settingService = settingService;
            _themeContext = themeContext;
            _commonSettings = commonSettings;
            _storeContext = storeContext;
            _nopFileProvider = nopFileProvider;
        }

        #endregion

        #region Methods
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (string.Equals(widgetZone, FortuneThemeDefaults.CustomCssWidgetZone, StringComparison.InvariantCultureIgnoreCase))
            {
                var workingThemeName = _themeContext.WorkingThemeName;
                var colorCssFile = $"~/Themes/{workingThemeName}/Content/css/color.css";
                if (!File.Exists(_nopFileProvider.MapPath(colorCssFile)))
                {
                    _logger.Error("Color.css file missing.");
                }
                if (!_commonSettings.EnableCssBundling)
                {
                    colorCssFile = colorCssFile + "?v=" + _settingService.GetSettingByKey($"{workingThemeName}themesettings.themeCustomCSSFileVersion", 0, _storeContext.CurrentStore.Id, false);
                }
                return View("~/Plugins/NopStation.Theme.Fortune/Views/Shared/_CustomCss.cshtml", colorCssFile);
            }
            
            return Content("");
        }
        #endregion
    }
}
