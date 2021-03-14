using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.QuickView.Models;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.NopStation.QuickView.Components
{
    public class QuickViewViewComponent : NopViewComponent
    {
        private readonly QuickViewSettings _quickViewSettings;
        private readonly INopStationLicenseService _licenseService;

        public QuickViewViewComponent(QuickViewSettings quickViewSettings,
            INopStationLicenseService licenseService)
        {
            _quickViewSettings = quickViewSettings;
            _licenseService = licenseService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (additionalData.GetType() != typeof(ProductOverviewModel))
                return Content("");

            var pm = (ProductOverviewModel)additionalData;
            var model = new PublicModel()
            {
                ProductId = pm.Id
            };

            return View(model);
        }
    }
}
