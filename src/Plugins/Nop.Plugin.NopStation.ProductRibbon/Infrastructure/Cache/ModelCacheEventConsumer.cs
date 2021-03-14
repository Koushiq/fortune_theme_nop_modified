using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.ProductRibbon.Infrastructure.Cache
{
    public class ModelCacheEventConsumer :
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : customer role ids
        /// {2} : language id
        /// </remarks>
        public static CacheKey PRODUCT_RIBBON_MODEL_KEY = new CacheKey("Nopstation.productribbon.{0}-{1}-{2}", PRODUCT_RIBBON_PATTERN_KEY);
        public const string PRODUCT_RIBBON_PATTERN_KEY = "Nopstation.productribbon.";


        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_RIBBON_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_RIBBON_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(PRODUCT_RIBBON_PATTERN_KEY);
        }
    }
}
