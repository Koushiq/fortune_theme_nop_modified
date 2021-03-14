using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.OCarousels.Factories;
using Nop.Plugin.NopStation.OCarousels.Helpers;
using Nop.Plugin.NopStation.OCarousels.Services;
using Nop.Web.Framework.Components;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.OCarousels.Components
{
    public class OCarouselViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContex;
        private readonly INopStationLicenseService _licenseService;
        private readonly IOCarouselService _carouselService;
        private readonly IOCarouselModelFactory _carouselModelFactory;
        private readonly OCarouselSettings _carouselSettings;

        public OCarouselViewComponent(IStoreContext storeContext,
            IOCarouselModelFactory carouselModelFactory,
            INopStationLicenseService licenseService,
            IOCarouselService carouselService,
            OCarouselSettings carouselSettings)
        {
            _storeContex = storeContext;
            _carouselModelFactory = carouselModelFactory;
            _carouselService = carouselService;
            _licenseService = licenseService;
            _carouselSettings = carouselSettings;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (!_carouselSettings.EnableOCarousel)
                return Content("");

            if (!OCarouselHelper.TryGetWidgetZoneId(widgetZone, out int widgetZoneId))
                return Content("");

            var carousels = _carouselService.GetAllCarousels(new List<int> { widgetZoneId }, storeId: _storeContex.CurrentStore.Id,  active: true).ToList();
            if (!carousels.Any())
                return Content("");
            
            var model = _carouselModelFactory.PrepareCarouselListModel(carousels);

            return View(model);
        }
    }
}
