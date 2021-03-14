using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.CustomSlider.Components
{
    public class CustomSliderFooterViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly CustomSliderSettings _CustomSliderSettings;

        public CustomSliderFooterViewComponent(INopStationLicenseService licenseService,
            CustomSliderSettings CustomSliderSettings)
        {
            _licenseService = licenseService;
            _CustomSliderSettings = CustomSliderSettings;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (!_CustomSliderSettings.EnableSlider)
                return Content("");

            return View();
        }
    }
}
