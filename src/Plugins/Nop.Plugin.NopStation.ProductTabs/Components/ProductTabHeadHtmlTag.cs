using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Framework.Components;
using System;

namespace Nop.Plugin.NopStation.ProductTabs.Components
{
    public class ProductTabHeadHtmlTagViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;

        public ProductTabHeadHtmlTagViewComponent(INopStationLicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            return View();
        }
    }
}
