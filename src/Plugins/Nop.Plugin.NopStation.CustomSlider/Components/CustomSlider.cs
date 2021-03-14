using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Framework.Components;
using Nop.Plugin.NopStation.CustomSlider.Helpers;
using System.Collections.Generic;
using Nop.Plugin.NopStation.CustomSlider.Services;
using Nop.Plugin.NopStation.CustomSlider.Factories;
using System.Linq;

namespace Nop.Plugin.NopStation.CustomSlider.Components
{
    public class CustomSliderViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContex;
        private readonly INopStationLicenseService _licenseService;
        private readonly ISliderModelFactory _sliderModelFactory;
        private readonly ISliderService _sliderService;
        private readonly CustomSliderSettings _CustomSliderSettings;

        public CustomSliderViewComponent(IStoreContext storeContext,
            INopStationLicenseService licenseService,
            ISliderModelFactory sliderModelFactory,
            ISliderService sliderService,
            CustomSliderSettings CustomSliderSettings)
        {
            _storeContex = storeContext;
            _licenseService = licenseService;
            _sliderModelFactory = sliderModelFactory;
            _sliderService = sliderService;
            _CustomSliderSettings = CustomSliderSettings;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (!_CustomSliderSettings.EnableSlider)
                return Content("");
            var model = _sliderModelFactory.PrepareSliderListModel(widgetZone);
            return View(model);
        }
    }
}
