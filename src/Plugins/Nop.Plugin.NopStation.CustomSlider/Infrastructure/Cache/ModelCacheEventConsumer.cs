using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Plugin.NopStation.CustomSlider.Domains;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.CustomSlider.Infrastructure.Cache
{
    public class ModelCacheEventConsumer :
        IConsumer<EntityInsertedEvent<Slider>>,
        IConsumer<EntityUpdatedEvent<Slider>>,
        IConsumer<EntityDeletedEvent<Slider>>,
        IConsumer<EntityInsertedEvent<SliderItem>>,
        IConsumer<EntityUpdatedEvent<SliderItem>>,
        IConsumer<EntityDeletedEvent<SliderItem>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : widget zone id
        /// {1} : is mobile device
        /// {2} : language id
        /// {3} : store id
        /// </remarks>
        public static CacheKey CustomSlider_SLIDER_MODEL_KEY = new CacheKey("Nopstation.CustomSlider.slider-{0}-{1}-{2}-{3}", CustomSlider_SLIDER_PATTERN_KEY);
        public const string CustomSlider_SLIDER_PATTERN_KEY = "Nopstation.CustomSlider.slider";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : widget zone id
        /// {1} : is mobile device
        /// {2} : language id
        /// {3} : store id
        /// </remarks>
        public static CacheKey CustomSlider_SLIDER_ITEM_MODEL_KEY = new CacheKey("Nopstation.CustomSlider.slider.item-{0}-{1}-{2}-{3}", CustomSlider_SLIDER_ITEM_PATTERN_KEY);
        public const string CustomSlider_SLIDER_ITEM_PATTERN_KEY = "Nopstation.CustomSlider.slider.item";

        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Slider> eventMessage)
        {
            _cacheManager.RemoveByPrefix(CustomSlider_SLIDER_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Slider> eventMessage)
        {
            _cacheManager.RemoveByPrefix(CustomSlider_SLIDER_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Slider> eventMessage)
        {
            _cacheManager.RemoveByPrefix(CustomSlider_SLIDER_PATTERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<SliderItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(CustomSlider_SLIDER_ITEM_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<SliderItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(CustomSlider_SLIDER_ITEM_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<SliderItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(CustomSlider_SLIDER_ITEM_PATTERN_KEY);
        }
    }
}
