using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.ProductTabs.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.ProductTabs.Components
{
    public class ProductTabViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly ProductTabSettings _productTabSettings;
        private readonly IProductTabModelFactory _productTabModelFactory;

        public ProductTabViewComponent(
            INopStationLicenseService licenseService,
            IProductTabModelFactory productTabModelFactory,
            ProductTabSettings productTabSettings)
        {
            _licenseService = licenseService;
            _productTabSettings = productTabSettings;
            _productTabModelFactory = productTabModelFactory;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (!_productTabSettings.EnableProductTab)
                return Content("");

            var productTabModels = _productTabModelFactory.PrepareProductTabListModel(widgetZone);
            return View(productTabModels);
        }
    }
}
