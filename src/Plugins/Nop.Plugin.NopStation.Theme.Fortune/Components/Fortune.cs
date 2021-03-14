using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.Theme.Fortune.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.Theme.Fortune.Components
{
    public class FortuneViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly FortuneSettings _fortuneSettings;

        public FortuneViewComponent(INopStationLicenseService licenseService,
            FortuneSettings fortuneSettings)
        {
            _licenseService = licenseService;
            _fortuneSettings = fortuneSettings;
        }

        public IViewComponentResult Invoke(string widgetZone)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            var model = new PublicModel()
            {
                CustomCss = _fortuneSettings.CustomCss,
                EnableImageLazyLoad = _fortuneSettings.EnableImageLazyLoad
            };

            return View(model);
        }
    }
}
