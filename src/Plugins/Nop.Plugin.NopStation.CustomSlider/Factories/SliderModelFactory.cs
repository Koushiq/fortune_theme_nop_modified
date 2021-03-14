using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.NopStation.CustomSlider.Services;
using Nop.Plugin.NopStation.CustomSlider.Domains;
using Nop.Plugin.NopStation.CustomSlider.Models;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.CustomSlider.Helpers;
using System.Linq;
using Nop.Services.Caching;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.CustomSlider.Infrastructure.Cache;
using Nop.Services.Customers;

namespace Nop.Plugin.NopStation.CustomSlider.Factories
{
    public class SliderModelFactory : ISliderModelFactory
    {
        private readonly ICustomerService _customerService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IPictureService _pictureService;
        private readonly ISliderService _sliderService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly INopStationContext _nopStationContext;

        public SliderModelFactory(
            ICustomerService customerService,
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IPictureService pictureService,
            ISliderService sliderService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            INopStationContext nopStationContext)
        {
            _customerService = customerService;
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _pictureService = pictureService;
            _sliderService = sliderService;
            _localizationService = localizationService;
            _workContext = workContext;
            _nopStationContext = nopStationContext;
        }

        public IList<SliderModel> PrepareSliderListModel(List<Slider> sliders)
        {
            if (sliders == null)
                throw new ArgumentNullException(nameof(sliders));

            var mobileDevice = _nopStationContext.MobileDevice;
            var model = new List<SliderModel>();
            foreach (var slider in sliders)
            {
                model.Add(PrepareSliderModel(slider, mobileDevice));
            }
            return model;
        }

        public IList<SliderModel> PrepareSliderListModel(string widgetZone)
        {
            if (string.IsNullOrEmpty(widgetZone))
                throw new ArgumentNullException(nameof(widgetZone));

            if (!SliderHelper.TryGetWidgetZoneId(widgetZone, out var widgetZoneId))
                return new List<SliderModel>();

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.CustomSlider_SLIDER_MODEL_KEY, 
                widgetZoneId,
                _nopStationContext.MobileDevice,
                _workContext.WorkingLanguage,
                _storeContext.CurrentStore);

            var sliderModelList = _staticCacheManager.Get(cacheKey, () =>
            {
                var sliders = _sliderService.GetAllSliders(new List<int> { widgetZoneId }, storeId: _storeContext.CurrentStore.Id, active: true).ToList();

                if (!sliders.Any())
                    return new List<SliderModel>();

                var sliderModelList = PrepareSliderListModel(sliders);
                
                return sliderModelList;
            });

            return sliderModelList;
        }

        public SliderModel PrepareSliderModel(Slider slider, bool mobileDevice) 
        {
            var sliderModel = new SliderModel()
            {
                Id = slider.Id,
                Name = slider.Name,
                WidgetZoneId = slider.WidgetZoneId,
                Nav = slider.Nav,
                AutoPlayHoverPause = slider.AutoPlayHoverPause,
                StartPosition = slider.StartPosition,
                LazyLoad = slider.LazyLoad,
                LazyLoadEager = slider.LazyLoadEager,
                Video = slider.Video,
                AnimateOut = slider.AnimateOut,
                AnimateIn = slider.AnimateIn,
                Loop = slider.Loop,
                AutoPlay = slider.AutoPlay,
                AutoPlayTimeout = slider.AutoPlayTimeout,
                RTL = _workContext.WorkingLanguage.Rtl
            };

            if (slider.ShowBackgroundPicture)
            {
                sliderModel.ShowBackgroundPicture = true;
                sliderModel.BackgroundPictureUrl = _pictureService.GetPictureUrl(slider.BackgroundPictureId);
            }

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.CustomSlider_SLIDER_ITEM_MODEL_KEY,
                slider,
                mobileDevice,
                _workContext.WorkingLanguage,
                _storeContext.CurrentStore);

            sliderModel.Items = _staticCacheManager.Get(cacheKey, () =>
            {
                var sliderItemModels = new List<SliderItemModel>();
                var sliderItems = _sliderService.GetSliderItemsBySliderId(slider.Id);
                foreach (var sliderItem in sliderItems)
                {
                    sliderItemModels.Add(new SliderItemModel()
                    {
                        Id = sliderItem.Id,
                        Title = _localizationService.GetLocalized(sliderItem, x => x.Title),
                        Link = _localizationService.GetLocalized(sliderItem, x => x.Link),
                        PictureUrl = _pictureService.GetPictureUrl(mobileDevice ? sliderItem.MobilePictureId : sliderItem.PictureId),
                        ImageAltText = _localizationService.GetLocalized(sliderItem, x => x.ImageAltText),
                        ShortDescription = _localizationService.GetLocalized(sliderItem, x => x.ShortDescription)
                    });
                }
                return sliderItemModels;
            });

            return sliderModel;
        }
    }
}
