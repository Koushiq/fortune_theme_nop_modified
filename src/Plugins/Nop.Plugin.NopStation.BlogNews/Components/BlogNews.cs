using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.BlogNews.Infrastructure;
using Nop.Plugin.NopStation.BlogNews.Models;
using Nop.Plugin.NopStation.BlogNews.Services;
using Nop.Services.Blogs;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Framework.Components;
using System;
using System.Linq;
using Nop.Plugin.NopStation.BlogNews.Domains;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Services.Stores;
using Nop.Services.Caching;
using Nop.Plugin.NopStation.BlogNews.Infrastructure.Cache;

namespace Nop.Plugin.NopStation.BlogNews.Components
{
    public partial class BlogNewsViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IBlogNewsPictureService _blogNewsPictureService;
        private readonly INopStationLicenseService _licenseService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly BlogNewsSettings _blogNewsSettings;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IPictureService _pictureService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IBlogService _blogService;
        private readonly INewsService _newsService;
        private readonly IWorkContext _workContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICacheKeyService _cacheKeyService;

        public BlogNewsViewComponent(
            IStoreContext storeContext,
            IBlogNewsPictureService blogNewsPictureService,
            INopStationLicenseService licenseService,
            IUrlRecordService urlRecordService,
            BlogNewsSettings blogNewsSettings,
            IStaticCacheManager cacheManager,
            IPictureService pictureService,
            IDateTimeHelper dateTimeHelper,
            IBlogService blogService,
            INewsService newsService,
            IWorkContext workContext,
            IStoreMappingService storeMappingService,
            ICacheKeyService cacheKeyService)
        {
            _blogNewsPictureService = blogNewsPictureService;
            _blogNewsSettings = blogNewsSettings;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _blogService = blogService;
            _newsService = newsService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
            _storeMappingService = storeMappingService;
            _cacheKeyService = cacheKeyService;
            _storeContext = storeContext;
            _cacheManager = cacheManager;
            _licenseService = licenseService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            var model = PreparePublicModel();
            return View(model);
        }

        private PublicModel PreparePublicModel()
        {
            var newsPageSize = _blogNewsSettings.NumberOfNewsItemsToShow == 0 ? int.MaxValue :
                _blogNewsSettings.NumberOfNewsItemsToShow;
            var blogPageSize = _blogNewsSettings.NumberOfBlogPostsToShow == 0 ? int.MaxValue :
                _blogNewsSettings.NumberOfBlogPostsToShow;

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(BlogNewsCacheEventConsumer.HOMEPAGE_BLOGNEWS_MODEL_KEY,
                _workContext.WorkingLanguage, 
                _storeContext.CurrentStore, 
                newsPageSize, 
                blogPageSize, 
                _blogNewsSettings.ShowNewsInStore, 
                _blogNewsSettings.NewsItemPictureSize, 
                _blogNewsSettings.ShowBlogsInStore, 
                _blogNewsSettings.BlogPostPictureSize);

            var defaultCacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new PublicModel();
                if (_blogNewsSettings.ShowBlogsInStore)
                {
                    var blogPictures = _blogNewsPictureService.GetAllPictures(EntityType.Blog, true, true,
                        _storeContext.CurrentStore.Id, _workContext.WorkingLanguage.Id, 0, blogPageSize);

                    if (blogPictures.Any())
                    {
                        var blogs = blogPictures.Select(x => _blogService.GetBlogPostById(x.EntityId));
                        foreach (var blogPicture in blogPictures)
                        {
                            var blog = blogs.FirstOrDefault(x => x.Id == blogPicture.EntityId);

                            var picture = _pictureService.GetPictureById(blogPicture.PictureId);

                            var mm = new BlogPostModel()
                            {
                                AltAttribute = picture != null ? picture.AltAttribute : "",
                                TitleAttribute = picture != null ? picture.AltAttribute : "",
                                PictureUrl = picture != null ? _pictureService.GetPictureUrl(picture.Id, _blogNewsSettings.BlogPostPictureSize) :
                                    _pictureService.GetDefaultPictureUrl(_blogNewsSettings.BlogPostPictureSize),
                                SeName = _urlRecordService.GetSeName(blog,blog.LanguageId,false,false),
                                CreatedOnUtcStr = _dateTimeHelper.ConvertToUserTime(blog.CreatedOnUtc, DateTimeKind.Utc).ToString("MMMM dd, yyyy"),
                                BodyOverview = blog.BodyOverview,
                                Id = blog.Id,
                                TotalComments = _blogService.GetBlogCommentsCount(blog, isApproved: true),
                                Title = blog.Title,
                                AllowComments = blog.AllowComments
                            };
                            model.BlogPosts.Add(mm);
                        }
                    }
                }

                if (_blogNewsSettings.ShowNewsInStore)
                {
                    var newsPictures = _blogNewsPictureService.GetAllPictures(EntityType.News, true, true,
                        _storeContext.CurrentStore.Id, _workContext.WorkingLanguage.Id, 0, newsPageSize);
                     
                    if (newsPictures.Any())
                    {
                        var newsItems = _newsService.GetNewsByIds(newsPictures.Select(x => x.EntityId).ToArray());
                        foreach (var newsPicture in newsPictures)
                        {
                            var news = newsItems.FirstOrDefault(x => x.Id == newsPicture.EntityId);

                            var picture = _pictureService.GetPictureById(newsPicture.PictureId);

                            var mm = new NewsItemModel()
                            {
                                AltAttribute = picture != null ? picture.AltAttribute : "",
                                TitleAttribute = picture != null ? picture.AltAttribute : "",
                                PictureUrl = picture != null ? _pictureService.GetPictureUrl(picture.Id, _blogNewsSettings.BlogPostPictureSize) :
                                    _pictureService.GetDefaultPictureUrl(_blogNewsSettings.BlogPostPictureSize),
                                SeName = _urlRecordService.GetSeName(news,news.LanguageId,false,false),
                                CreatedOnUtcStr = _dateTimeHelper.ConvertToUserTime(news.CreatedOnUtc, DateTimeKind.Utc).ToString("MMMM dd, yyyy"),
                                ShortDescription = news.Short,
                                Id = news.Id,
                                TotalComments = _newsService.GetNewsCommentsCount(news, isApproved: true),
                                Title = news.Title,
                                AllowComments = news.AllowComments
                            };
                            model.NewsItems.Add(mm);
                        }
                    }
                }
                return model;
            });

            return defaultCacheModel;
        }
    }
}
