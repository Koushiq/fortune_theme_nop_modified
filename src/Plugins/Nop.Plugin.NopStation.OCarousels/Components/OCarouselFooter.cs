using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.OCarousels.Components
{
    public class OCarouselFooterViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly OCarouselSettings _carouselSettings;

        public OCarouselFooterViewComponent(INopStationLicenseService licenseService,
            OCarouselSettings carouselSettings)
        {
            _licenseService = licenseService;
            _carouselSettings = carouselSettings;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (!_carouselSettings.EnableOCarousel)
                return Content("");

            return View();
        }
    }
}
