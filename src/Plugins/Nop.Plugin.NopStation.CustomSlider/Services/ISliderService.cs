using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.NopStation.CustomSlider.Domains;

namespace Nop.Plugin.NopStation.CustomSlider.Services
{
    public interface ISliderService
    {
        #region Slider

        IPagedList<Slider> GetAllSliders(List<int> widgetZoneIds = null, int storeId = 0,
            bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Slider GetSliderById(int sliderId);

        void InsertSlider(Slider slider);

        void UpdateSlider(Slider slider);

        void DeleteSlider(Slider slider);

        #endregion

        #region Slider items

        IPagedList<SliderItem> GetSliderItemsBySliderId(int sliderId, int pageIndex = 0, int pageSize = int.MaxValue);

        SliderItem GetSliderItemById(int sliderItemId);

        void InsertSliderItem(SliderItem sliderItem);

        void UpdateSliderItem(SliderItem sliderItem);

        void DeleteSliderItem(SliderItem sliderItem);

        #endregion        
    }
}