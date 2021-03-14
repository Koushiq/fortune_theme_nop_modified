using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.News;
using Nop.Core.Events;
using Nop.Plugin.NopStation.BlogNews.Domains;
using Nop.Services.Events;

namespace Nop.Plugin.NopStation.BlogNews.Infrastructure.Cache
{
    public class BlogNewsCacheEventConsumer : IConsumer<EntityInsertedEvent<BlogNewsPicture>>,
        IConsumer<EntityUpdatedEvent<BlogNewsPicture>>,
        IConsumer<EntityDeletedEvent<BlogNewsPicture>>,

        IConsumer<EntityInsertedEvent<BlogPost>>,
        IConsumer<EntityUpdatedEvent<BlogPost>>,
        IConsumer<EntityDeletedEvent<BlogPost>>,

        IConsumer<EntityInsertedEvent<NewsItem>>,
        IConsumer<EntityUpdatedEvent<NewsItem>>,
        IConsumer<EntityDeletedEvent<NewsItem>>
    {
        #region Fields

        private readonly BlogNewsSettings _blogNewsSettings;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public BlogNewsCacheEventConsumer(BlogNewsSettings blogNewsSettings, IStaticCacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
            this._blogNewsSettings = blogNewsSettings;
        }

        #endregion

        #region Cache keys 

        public static CacheKey HOMEPAGE_BLOGNEWS_MODEL_KEY = new CacheKey("NopStation.blognews.homepage-{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}", HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        public const string HOMEPAGE_BLOGNEWS_PATTERN_KEY = "NopStation.blognews.homepage";

        #endregion

        #region Methods

        public void HandleEvent(EntityInsertedEvent<BlogNewsPicture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<BlogNewsPicture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<BlogNewsPicture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        public void HandleEvent(EntityInsertedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(HOMEPAGE_BLOGNEWS_PATTERN_KEY);
        }

        #endregion
    }
}
