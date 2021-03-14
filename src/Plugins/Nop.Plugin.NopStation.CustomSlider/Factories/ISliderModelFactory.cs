using System.Collections.Generic;
using Nop.Plugin.NopStation.CustomSlider.Domains;
using Nop.Plugin.NopStation.CustomSlider.Models;

namespace Nop.Plugin.NopStation.CustomSlider.Factories
{
    public interface ISliderModelFactory
    {
        IList<SliderModel> PrepareSliderListModel(List<Slider> sliders);
        IList<SliderModel> PrepareSliderListModel(string widgetZone);
    }
}