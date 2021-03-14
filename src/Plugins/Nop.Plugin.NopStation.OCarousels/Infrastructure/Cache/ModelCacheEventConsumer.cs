using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Plugin.NopStation.OCarousels.Domains;
using Nop.Core.Domain.Media;

namespace Nop.Plugin.NopStation.OCarousels.Infrastructure.Cache
{
    public partial class ModelCacheEventConsumer :
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        IConsumer<EntityInsertedEvent<OCarousel>>,
        IConsumer<EntityUpdatedEvent<OCarousel>>,
        IConsumer<EntityDeletedEvent<OCarousel>>,
        IConsumer<EntityInsertedEvent<OCarouselItem>>,
        IConsumer<EntityUpdatedEvent<OCarouselItem>>,
        IConsumer<EntityDeletedEvent<OCarouselItem>>
    {
        /// <summary>
        /// Key for caching background picture
        /// </summary>
        /// <remarks>
        /// {0} : carousel id
        /// {1} : store id
        /// </remarks>
        public static CacheKey OCAROUSEL_BACKGROUND_PICTURE_MODEL_KEY = new CacheKey("Nopstation.ocarousel.items.background_picture.{0}-{1}", OCAROUSEL_BACKGROUND_PICTURE_PATTERN_KEY);
        public const string OCAROUSEL_BACKGROUND_PICTURE_PATTERN_KEY = "Nopstation.ocarousel.items.background_picture.";

        /// <summary>
        /// Key for caching category
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : customer roles
        /// {2} : language id
        /// {3} : store id
        /// </remarks>
        public static CacheKey OCAROUSEL_CATEGORY_MODEL_KEY = new CacheKey("Nopstation.ocarousel.items.category.{0}-{1}-{2}-{3}", OCAROUSEL_CATEGORY_PATTERN_KEY);
        public const string OCAROUSEL_CATEGORY_PATTERN_KEY = "Nopstation.ocarousel.items.category.";

        /// <summary>
        /// Key for caching manufacturer
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : customer roles
        /// {2} : language id
        /// {3} : store id
        /// </remarks>
        public static CacheKey OCAROUSEL_MANUFACTURER_MODEL_KEY = new CacheKey("Nopstation.ocarousel.items.manufacturer.{0}-{1}-{2}-{3}", OCAROUSEL_MANUFACTURER_PATTERN_KEY);
        public const string OCAROUSEL_MANUFACTURER_PATTERN_KEY = "Nopstation.ocarousel.items.manufacturer.";

        /// <summary>
        /// Key for caching custom product ids
        /// </summary>
        /// <remarks>
        /// {0} : carousel id
        /// </remarks>
        public static CacheKey OCAROUSEL_CUSTOMRODUCTIDS_MODEL_KEY = new CacheKey("Nopstation.ocarousel.items.customproductids.{0}", OCAROUSEL_CUSTOMPRODUCTIDS_PATTERN_KEY);
        public const string OCAROUSEL_CUSTOMPRODUCTIDS_PATTERN_KEY = "Nopstation.ocarousel.items.customproductids.";

        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_MANUFACTURER_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_MANUFACTURER_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_MANUFACTURER_PATTERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CATEGORY_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CATEGORY_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CATEGORY_PATTERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<OCarousel> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CUSTOMPRODUCTIDS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OCAROUSEL_BACKGROUND_PICTURE_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<OCarousel> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CUSTOMPRODUCTIDS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OCAROUSEL_BACKGROUND_PICTURE_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<OCarousel> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CUSTOMPRODUCTIDS_PATTERN_KEY);
            _cacheManager.RemoveByPrefix(OCAROUSEL_BACKGROUND_PICTURE_PATTERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<OCarouselItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CUSTOMPRODUCTIDS_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<OCarouselItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CUSTOMPRODUCTIDS_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<OCarouselItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(OCAROUSEL_CUSTOMPRODUCTIDS_PATTERN_KEY);
        }
    }
}
