using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.AnywhereSlider.Components
{
    public class AnywhereSliderFooterViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly AnywhereSliderSettings _anywhereSliderSettings;

        public AnywhereSliderFooterViewComponent(INopStationLicenseService licenseService,
            AnywhereSliderSettings anywhereSliderSettings)
        {
            _licenseService = licenseService;
            _anywhereSliderSettings = anywhereSliderSettings;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (!_anywhereSliderSettings.EnableSlider)
                return Content("");

            return View();
        }
    }
}
