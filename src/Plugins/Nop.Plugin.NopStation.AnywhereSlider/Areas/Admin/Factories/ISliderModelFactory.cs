using Nop.Plugin.NopStation.AnywhereSlider.Areas.Admin.Models;
using Nop.Plugin.NopStation.AnywhereSlider.Domains;

namespace Nop.Plugin.NopStation.AnywhereSlider.Areas.Admin.Factories
{
    public interface ISliderModelFactory
    {
        ConfigurationModel PrepareConfigurationModel();

        SliderSearchModel PrepareSliderSearchModel(SliderSearchModel sliderSearchModel);

        SliderListModel PrepareSliderListModel(SliderSearchModel searchModel);

        SliderModel PrepareSliderModel(SliderModel model, Slider slider, bool excludeProperties = false);

        SliderItemListModel PrepareSliderItemListModel(SliderItemSearchModel searchModel);

        SliderItemModel PrepareSliderItemModel(SliderItemModel model, Slider slider, 
            SliderItem sliderItem, bool excludeProperties = false);
    }
}