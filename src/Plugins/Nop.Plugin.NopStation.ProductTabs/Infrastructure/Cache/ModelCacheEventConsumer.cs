using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Plugin.NopStation.ProductTabs.Domains;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.ProductTabs.Infrastructure.Cache
{
    public class ModelCacheEventConsumer : 
        IConsumer<EntityInsertedEvent<ProductTab>>,
        IConsumer<EntityUpdatedEvent<ProductTab>>,
        IConsumer<EntityDeletedEvent<ProductTab>>,
        IConsumer<EntityInsertedEvent<ProductTabItem>>,
        IConsumer<EntityUpdatedEvent<ProductTabItem>>,
        IConsumer<EntityDeletedEvent<ProductTabItem>>,
        IConsumer<EntityInsertedEvent<ProductTabItemProduct>>,
        IConsumer<EntityUpdatedEvent<ProductTabItemProduct>>,
        IConsumer<EntityDeletedEvent<ProductTabItemProduct>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : widget zone id
        /// {1} : customer role ids
        /// {2} : language id
        /// {3} : store id
        /// </remarks>
        public static CacheKey PRODUCT_TAB_MODEL_KEY = new CacheKey("Nopstation.producttabs.producttab-{0}-{1}-{2}-{3}", PRODUCT_TAB_PATTERN_KEY);
        public const string PRODUCT_TAB_PATTERN_KEY = "Nopstation.producttabs.producttab";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product tab id
        /// {1} : customer role ids
        /// {2} : language id
        /// {3} : store id
        /// </remarks>
        public static CacheKey PRODUCT_TAB_ITEM_MODEL_KEY = new CacheKey("Nopstation.producttabs.producttab.item-{0}-{1}-{2}-{3}", PRODUCT_TAB_ITEM_PATTERN_KEY);
        public const string PRODUCT_TAB_ITEM_PATTERN_KEY = "Nopstation.producttabs.producttab.item";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product tab item id
        /// {1} : customer role ids
        /// {2} : language id
        /// {3} : store id
        /// </remarks>
        public static CacheKey PRODUCT_TAB_ITEM_PRODUCT_MODEL_KEY = new CacheKey("Nopstation.producttabs.producttab.item.products-{0}-{1}-{2}-{3}", PRODUCT_TAB_ITEM_PRODUCT_PATTERN_KEY);
        public const string PRODUCT_TAB_ITEM_PRODUCT_PATTERN_KEY = "Nopstation.producttabs.producttab.item.products";

        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<ProductTab> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<ProductTab> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<ProductTab> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_PATTERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<ProductTabItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_ITEM_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<ProductTabItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_ITEM_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<ProductTabItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_ITEM_PATTERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<ProductTabItemProduct> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_ITEM_PRODUCT_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<ProductTabItemProduct> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_ITEM_PRODUCT_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<ProductTabItemProduct> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_TAB_ITEM_PRODUCT_PATTERN_KEY);
        }
    }
}
