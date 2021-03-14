using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.Theme.Fortune.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public class ModelCacheEventConsumer :
        IConsumer<EntityInsertedEvent<Setting>>,
        IConsumer<EntityUpdatedEvent<Setting>>,
        IConsumer<EntityDeletedEvent<Setting>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : connection type (http/https)
        /// </remarks>
        public static CacheKey PICTURE_URL_MODEL_KEY = new CacheKey("Nop.plugin.nopstation.theme.fortune.pictureurl-{0}-{1}", PICTURE_URL_PATTERN_KEY);
        public const string PICTURE_URL_PATTERN_KEY = "Nop.plugin.nopstation.theme.fortune.pictureurl";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store id
        /// {1} : language id
        /// </remarks>
        public static CacheKey FOOTER_DESCRIPTION_MODEL_KEY = new CacheKey("Nop.plugin.nopstation.theme.fortune.footer.description-{0}-{1}", FOOTER_DESCRIPTION_PATTERN_KEY);
        public const string FOOTER_DESCRIPTION_PATTERN_KEY = "Nop.plugin.nopstation.theme.fortune.footer.description";

        private readonly IStaticCacheManager _staticCacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(PICTURE_URL_PATTERN_KEY);
            _staticCacheManager.RemoveByPrefix(FOOTER_DESCRIPTION_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(PICTURE_URL_PATTERN_KEY);
            _staticCacheManager.RemoveByPrefix(FOOTER_DESCRIPTION_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<Setting> eventMessage)
        {
            _staticCacheManager.RemoveByPrefix(PICTURE_URL_PATTERN_KEY);
            _staticCacheManager.RemoveByPrefix(FOOTER_DESCRIPTION_PATTERN_KEY);
        }
    }
}
