using System.Collections.Generic;
using Nop.Plugin.NopStation.AnywhereSlider.Domains;
using Nop.Plugin.NopStation.AnywhereSlider.Models;

namespace Nop.Plugin.NopStation.AnywhereSlider.Factories
{
    public interface ISliderModelFactory
    {
        IList<SliderModel> PrepareSliderListModel(List<Slider> sliders);
        IList<SliderModel> PrepareSliderListModel(string widgetZone);
    }
}