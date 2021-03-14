using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Framework.Components;
using Nop.Plugin.NopStation.AnywhereSlider.Helpers;
using System.Collections.Generic;
using Nop.Plugin.NopStation.AnywhereSlider.Services;
using Nop.Plugin.NopStation.AnywhereSlider.Factories;
using System.Linq;

namespace Nop.Plugin.NopStation.AnywhereSlider.Components
{
    public class AnywhereSliderViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContex;
        private readonly INopStationLicenseService _licenseService;
        private readonly ISliderModelFactory _sliderModelFactory;
        private readonly ISliderService _sliderService;
        private readonly AnywhereSliderSettings _anywhereSliderSettings;

        public AnywhereSliderViewComponent(IStoreContext storeContext,
            INopStationLicenseService licenseService,
            ISliderModelFactory sliderModelFactory,
            ISliderService sliderService,
            AnywhereSliderSettings anywhereSliderSettings)
        {
            _storeContex = storeContext;
            _licenseService = licenseService;
            _sliderModelFactory = sliderModelFactory;
            _sliderService = sliderService;
            _anywhereSliderSettings = anywhereSliderSettings;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (!_anywhereSliderSettings.EnableSlider)
                return Content("");
            var model = _sliderModelFactory.PrepareSliderListModel(widgetZone);
            return View(model);
        }
    }
}
