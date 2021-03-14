using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.MegaMenu.Infrastructure.Cache
{
    public class MegaMenuModelCacheEventConsumer:
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        IConsumer<EntityInsertedEvent<Topic>>,
        IConsumer<EntityUpdatedEvent<Topic>>,
        IConsumer<EntityDeletedEvent<Topic>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : customer role ids
        /// {2} : store id
        /// </remarks>
        public static CacheKey MEGAMENU_TOPICS_MODEL_KEY = new CacheKey("Nopstation.megamenu.topic-{0}-{1}-{2}", MEGAMENU_TOPICS_PATERN_KEY);
        public const string MEGAMENU_TOPICS_PATERN_KEY = "Nopstation.megamenu.topic";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : customer role ids
        /// {2} : store id
        /// </remarks>
        public static CacheKey MEGAMENU_CATEGORIES_MODEL_KEY = new CacheKey("Nopstation.megamenu.categories-{0}-{1}-{2}", MEGAMENU_CATEGORIES_PATERN_KEY);
        public const string MEGAMENU_CATEGORIES_PATERN_KEY = "Nopstation.megamenu.categories";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : customer role ids
        /// {2} : store id
        /// </remarks>
        public static CacheKey MEGAMENU_MANUFACTURERS_MODEL_KEY = new CacheKey("Nopstation.megamenu.manufacturers-{0}-{1}-{2}", MEGAMENU_MANUFACTURERS_PATERN_KEY);
        public const string MEGAMENU_MANUFACTURERS_PATERN_KEY = "Nopstation.megamenu.manufacturers";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : customer role ids
        /// {2} : store id
        /// </remarks>
        public static CacheKey MEGAMENU_MODEL_KEY = new CacheKey("Nopstation.megamenu.all-{0}-{1}-{2}", MEGAMENU_PATERN_KEY);
        public const string MEGAMENU_PATERN_KEY = "Nopstation.megamenu.";

        private readonly IStaticCacheManager _cacheManager;

        public MegaMenuModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_MANUFACTURERS_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_MANUFACTURERS_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_MANUFACTURERS_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_CATEGORIES_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_CATEGORIES_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_CATEGORIES_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }
        public void HandleEvent(EntityInsertedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_TOPICS_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_TOPICS_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPrefix(MEGAMENU_TOPICS_PATERN_KEY);
            _cacheManager.RemoveByPrefix(MEGAMENU_PATERN_KEY);
        }
    }
}
