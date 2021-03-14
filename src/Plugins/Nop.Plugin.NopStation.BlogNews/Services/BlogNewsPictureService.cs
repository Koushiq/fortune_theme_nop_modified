using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.News;
using Nop.Plugin.NopStation.BlogNews.Domains;
using Nop.Plugin.NopStation.BlogNews.Data;
using Nop.Services.Events;
using System;
using System.Linq;
using Nop.Data;

namespace Nop.Plugin.NopStation.BlogNews.Services
{
    public class BlogNewsPictureService : IBlogNewsPictureService
    {
        #region Fields

        
        private readonly IRepository<BlogNewsPicture> _blogNewsPictureRepository;
        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly INopDataProvider _nopDataProvider;

        #endregion

        #region Ctor

        public BlogNewsPictureService(
            IRepository<BlogNewsPicture> blogNewsPictureRepository,
            IRepository<BlogPost> blogPostRepository,
            IRepository<NewsItem> newsItemRepository,
            IEventPublisher eventPublisher,
            INopDataProvider nopDataProvider
            )
        {
            _blogNewsPictureRepository = blogNewsPictureRepository;
            _blogPostRepository = blogPostRepository;
            _newsItemRepository = newsItemRepository;
            _eventPublisher = eventPublisher;
            _nopDataProvider = nopDataProvider;
        }

        #endregion

        #region Methods

        public void InsertBlogNewsPicture(BlogNewsPicture blogNewsPicture)
        {
            _blogNewsPictureRepository.Insert(blogNewsPicture);

            _eventPublisher.EntityInserted(blogNewsPicture);
        }

        public void UpdateBlogNewsPicture(BlogNewsPicture blogNewsPicture)
        {
            _blogNewsPictureRepository.Update(blogNewsPicture);

            _eventPublisher.EntityUpdated(blogNewsPicture);
        }

        public void DeleteBlogNewsPicture(BlogNewsPicture blogNewsPicture)
        {
            _blogNewsPictureRepository.Delete(blogNewsPicture);

            _eventPublisher.EntityDeleted(blogNewsPicture);
        }

        public BlogNewsPicture GetBlogNewsPictureByEntytiId(int entityId, EntityType entityType)
        {
            return _blogNewsPictureRepository.Table
                .FirstOrDefault(x => x.EntityId == entityId && 
                x.EntityTypeId == (int)entityType);
        }

        public IPagedList<BlogNewsPicture> GetAllPictures(EntityType entityType,
            bool? showOnHomePage = null, bool? published = null, int storeId = 0,
            int languageId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            pageSize = pageSize == int.MaxValue ? pageSize - 1 : pageSize;
            var pEntityTypeId = SqlParameterHelper.GetInt32Parameter("EntityTypeId", (int)entityType);
            var pStoreId = SqlParameterHelper.GetInt32Parameter("StoreId", storeId);
            var pLanguageId = SqlParameterHelper.GetInt32Parameter("LanguageId", languageId);
            var pShowInStore = SqlParameterHelper.GetInt32Parameter("ShowInStore", showOnHomePage.HasValue? Convert.ToInt32(showOnHomePage.Value): -1);
            var pPublished = SqlParameterHelper.GetInt32Parameter("Published", published.HasValue? Convert.ToInt32(published.Value): -1);
            var pPageIndex = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);
            var pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");

            var blogNewsPictures = _nopDataProvider.QueryProc<BlogNewsPicture>("BlogNewsPictureLoadPaged",
                pEntityTypeId,
                pStoreId,
                pLanguageId,
                pShowInStore,
                pPublished,
                pPageIndex,
                pPageSize,
                pTotalRecords).ToList();

            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<BlogNewsPicture>(blogNewsPictures, pageIndex, pageSize, totalRecords);
        }

        #endregion
    }
}
