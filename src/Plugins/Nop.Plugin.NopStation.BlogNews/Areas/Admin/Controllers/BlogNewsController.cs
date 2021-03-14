using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.BlogNews.Areas.Admin.Models;
using Nop.Plugin.NopStation.BlogNews.Domains;
using Nop.Plugin.NopStation.BlogNews.Infrastructure.Cache;
using Nop.Plugin.NopStation.BlogNews.Services;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Services.Blogs;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using System;

namespace Nop.Plugin.NopStation.BlogNews.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class BlogNewsController : BaseAdminController
    {
        #region Fields

        private readonly INewsService _newsService;
        private readonly IBlogNewsPictureService _blogNewsPictureService;
        private readonly IPermissionService _permissionService;
        private readonly BlogNewsSettings _blogNewsSettings;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IBlogService _blogService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public BlogNewsController(IBlogService blogService,
            IBlogNewsPictureService blogNewsPictureService,
            IPermissionService permissionService,
            BlogNewsSettings blogNewsSettings,
            IStaticCacheManager cacheManager,
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext,
            INewsService newsService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _newsService = newsService;
            _blogNewsPictureService = blogNewsPictureService;
            _permissionService = permissionService;
            _blogNewsSettings = blogNewsSettings;
            _pictureService = pictureService;
            _settingService = settingService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _blogService = blogService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(BlogNewsPermissionProvider.ManageBlogNews))
                return AccessDeniedView();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var blogNewsSettings = _settingService.LoadSetting<BlogNewsSettings>(storeId);
            var model = blogNewsSettings.ToSettingsModel<ConfigurationModel>();

            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return View(model);

            model.WidgetZone_OverrideForStore = _settingService.SettingExists(blogNewsSettings, x => x.WidgetZone, storeId);
            model.BlogPostPictureSize_OverrideForStore = _settingService.SettingExists(blogNewsSettings, x => x.BlogPostPictureSize, storeId);
            model.NewsItemPictureSize_OverrideForStore = _settingService.SettingExists(blogNewsSettings, x => x.NewsItemPictureSize, storeId);
            model.ShowBlogsInStore_OverrideForStore = _settingService.SettingExists(blogNewsSettings, x => x.ShowBlogsInStore, storeId);
            model.NumberOfBlogPostsToShow_OverrideForStore = _settingService.SettingExists(blogNewsSettings, x => x.NumberOfBlogPostsToShow, storeId);
            model.ShowNewsInStore_OverrideForStore = _settingService.SettingExists(blogNewsSettings, x => x.ShowNewsInStore, storeId);
            model.NumberOfNewsItemsToShow_OverrideForStore = _settingService.SettingExists(blogNewsSettings, x => x.NumberOfNewsItemsToShow, storeId);

            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(BlogNewsPermissionProvider.ManageBlogNews))
                return AccessDeniedView();
            
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var blogNewsSettings = _settingService.LoadSetting<BlogNewsSettings>(storeScope);
            blogNewsSettings = model.ToSettings(blogNewsSettings);

            _settingService.SaveSettingOverridablePerStore(blogNewsSettings, x => x.WidgetZone, model.WidgetZone_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogNewsSettings, x => x.ShowBlogsInStore, model.ShowBlogsInStore_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogNewsSettings, x => x.BlogPostPictureSize, model.BlogPostPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogNewsSettings, x => x.NumberOfBlogPostsToShow, model.NumberOfBlogPostsToShow_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(blogNewsSettings, x => x.ShowNewsInStore, model.ShowNewsInStore_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogNewsSettings, x => x.NewsItemPictureSize, model.NewsItemPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(blogNewsSettings, x => x.NumberOfNewsItemsToShow, model.NumberOfNewsItemsToShow_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            _cacheManager.RemoveByPrefix(BlogNewsCacheEventConsumer.HOMEPAGE_BLOGNEWS_PATTERN_KEY);

            _notificationService.SuccessNotification(_localizationService.GetResource("NopStation.BlogNews.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        public IActionResult BlogPostPictureSave(int pictureId, string overrideAltAttribute, 
            string overrideTitleAttribute, int blogPostId, bool showOnHomePage)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            //try to get a product with the specified id
            var blogPost = _blogService.GetBlogPostById(blogPostId)
                ?? throw new ArgumentException("No blog found with the specified id");

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(blogPost.Title));

            var blogNewsPicture = _blogNewsPictureService.GetBlogNewsPictureByEntytiId(blogPostId, EntityType.Blog);
            if (blogNewsPicture != null)
            {
                if (blogNewsPicture.PictureId != pictureId)
                {
                    var oldPicture = _pictureService.GetPictureById(blogNewsPicture.PictureId);
                    if (oldPicture != null)
                        _pictureService.DeletePicture(oldPicture);
                }

                blogNewsPicture.PictureId = pictureId;
                blogNewsPicture.ShowInStore = showOnHomePage;
                _blogNewsPictureService.UpdateBlogNewsPicture(blogNewsPicture);
            }
            else
            {
                var newBlogNewsPicture = new BlogNewsPicture()
                {
                    EntityId = blogPostId,
                    PictureId = pictureId,
                    EntityType = EntityType.Blog,
                    ShowInStore = showOnHomePage
                };
                _blogNewsPictureService.InsertBlogNewsPicture(newBlogNewsPicture);
            }

            return Json(new { Result = true, PictureUrl = _pictureService.GetPictureUrl(picture.Id, _blogNewsSettings.BlogPostPictureSize) });
        }

        public IActionResult NewsItemPictureSave(int pictureId, string overrideAltAttribute, 
            string overrideTitleAttribute, int newsItemId, bool showOnHomePage)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            //try to get a product with the specified id
            var blogPost = _newsService.GetNewsById(newsItemId)
                ?? throw new ArgumentException("No news found with the specified id");

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(blogPost.Title));

            var blogNewsPicture = _blogNewsPictureService.GetBlogNewsPictureByEntytiId(newsItemId, EntityType.News);
            if (blogNewsPicture != null)
            {
                if (blogNewsPicture.PictureId != pictureId)
                {
                    var oldPicture = _pictureService.GetPictureById(blogNewsPicture.PictureId);
                    if (oldPicture != null)
                        _pictureService.DeletePicture(oldPicture);
                }

                blogNewsPicture.PictureId = pictureId;
                blogNewsPicture.ShowInStore = showOnHomePage;
                _blogNewsPictureService.UpdateBlogNewsPicture(blogNewsPicture);
            }
            else
            {
                var newBlogNewsPicture = new BlogNewsPicture()
                {
                    EntityId = newsItemId,
                    PictureId = pictureId,
                    EntityType = EntityType.News,
                    ShowInStore = showOnHomePage
                };
                _blogNewsPictureService.InsertBlogNewsPicture(newBlogNewsPicture);
            }

            return Json(new { Result = true, PictureUrl = _pictureService.GetPictureUrl(picture.Id, _blogNewsSettings.NewsItemPictureSize) });
        }

        #endregion
    }
}
