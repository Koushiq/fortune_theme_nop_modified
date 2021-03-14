using Nop.Core;
using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Plugin.NopStation.BlogNews.Data;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.Core;
using Nop.Services.Security;
using Nop.Plugin.NopStation.Core.Helpers;
using Nop.Data;
using Nop.Services.Logging;

namespace Nop.Plugin.NopStation.BlogNews
{
    public class BlogNewsPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        #region Fields

        public bool HideInWidgetList => false;

        private readonly IWebHelper _webHelper;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly BlogNewsSettings _blogNewsSettings;
        private readonly ISettingService _settingService;
        private readonly INopDataProvider _nopDataProvider;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public BlogNewsPlugin(IWebHelper webHelper,
            INopStationCoreService nopStationCoreService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            BlogNewsSettings blogNewsSettings,
            ISettingService settingService,
            INopDataProvider nopDataProvider,
            ILogger logger
            )
        {
            _webHelper = webHelper;
            _nopStationCoreService = nopStationCoreService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _blogNewsSettings = blogNewsSettings;
            _settingService = settingService;
            _nopDataProvider = nopDataProvider;
            _logger = logger;
        }

        #endregion


        #region Sql

        private string _createSpSql = @"CREATE PROCEDURE [dbo].[BlogNewsPictureLoadPaged]
        (
         @EntityTypeId		int = 10,
         @StoreId			int = 0,
         @LanguageId			int = 0,
         @ShowInStore		int = -1,
         @Published			int = -1,
         @PageIndex			int = 0, 
         @PageSize			int = 2147483644,
         @TotalRecords		int = null OUTPUT
        )
        AS
        BEGIN

         DECLARE @sql nvarchar(max)

         CREATE TABLE #SearchBlogNews
         (
          [IndexId] int identity (1,1),
          [BlogNewsPictureId] int NOT NULL
         )

         if(@EntityTypeId = 10)
          BEGIN
           SET @sql = '
            INSERT INTO #SearchBlogNews ([BlogNewsPictureId])
            SELECT bnp.Id
            FROM
             [NS_BlogNewsPicture] bnp INNER JOIN BlogPost bp on bp.Id = bnp.EntityId WHERE 
             bnp.EntityTypeId = ' + CAST(@EntityTypeId as nvarchar(max)) + ' AND (bp.StartDateUtc is null or 
             bp.StartDateUtc <= GETUTCDATE()) and (bp.EndDateUtc is null or bp.EndDateUtc >= GETUTCDATE()) '

           if(@ShowInStore > -1)
            SET @sql = @sql + ' and bnp.ShowInStore = ' + CAST(@ShowInStore as nvarchar(max))

           if(@LanguageId > 0)
            SET @sql = @sql + ' and bp.LanguageId = ' + CAST(@LanguageId as nvarchar(max))

           --filter by store
           IF @StoreId > 0
           BEGIN
            SET @sql = @sql + '
            AND (bp.LimitedToStores = 0 OR EXISTS (
             SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
             WHERE [sm].EntityId = bp.Id AND [sm].EntityName = ''BlogPost'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
             ))'
           END

           SET @sql = @sql + ' order by bp.Id desc'
          END
         ELSE
          BEGIN
           SET @sql = '
            INSERT INTO #SearchBlogNews ([BlogNewsPictureId])
            SELECT bnp.Id
            FROM
             [NS_BlogNewsPicture] bnp INNER JOIN News ni on ni.Id = bnp.EntityId WHERE ni.Published = 1
             AND bnp.EntityTypeId = ' + CAST(@EntityTypeId as nvarchar(max)) + ' AND (ni.StartDateUtc is 
             null or ni.StartDateUtc <= GETUTCDATE()) and (ni.EndDateUtc is null or ni.EndDateUtc >= GETUTCDATE())'

           if(@ShowInStore > -1)
            SET @sql = @sql + ' and bnp.ShowInStore = ' + CAST(@ShowInStore as nvarchar(max))

           if(@Published > -1)
            SET @sql = @sql + ' and ni.Published = ' + CAST(@Published as nvarchar(max))

           if(@LanguageId > -1)
            SET @sql = @sql + ' and ni.LanguageId = ' + CAST(@LanguageId as nvarchar(max))

           --filter by store
           IF @StoreId > 0
           BEGIN
            SET @sql = @sql + '
            AND (ni.LimitedToStores = 0 OR EXISTS (
             SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
             WHERE [sm].EntityId = ni.Id AND [sm].EntityName = ''NewsItem'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
             ))'
           END

           SET @sql = @sql + ' order by ni.Id desc'
          END

         EXEC sp_executesql @sql

         SET @TotalRecords = @@rowcount

         --paging
         DECLARE @PageLowerBound int
         DECLARE @PageUpperBound int
         DECLARE @RowsToReturn int
         SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
         SET @PageLowerBound = @PageSize * @PageIndex
         SET @PageUpperBound = @PageLowerBound + @PageSize + 1

         SELECT TOP (@RowsToReturn)
          bnp.*
         FROM
          #SearchBlogNews [sbn]
          INNER JOIN [NS_BlogNewsPicture] bnp with (NOLOCK) on bnp.Id = [sbn].[BlogNewsPictureId]
         WHERE
          [sbn].IndexId > @PageLowerBound AND 
          [sbn].IndexId < @PageUpperBound
         ORDER BY
          [sbn].IndexId

         DROP TABLE #SearchBlogNews

        END";

        private string _createSpMySql = @"CREATE PROCEDURE BlogNewsPictureLoadPaged
        (
	        `EntityTypeId`		int, 
	        `StoreId`			int,
	        `LanguageId`			int,
	        `ShowInStore`		int,
	        `Published`			int,
	        `PageIndex`			int,
	        `PageSize`			int,
            OUT `TotalRecords`		int
        )
        READS SQL DATA
        SQL SECURITY INVOKER

        BEGIN
            SET @sql = '';
            drop temporary TABLE if exists `SearchBlogNews`;
            
            CREATE temporary  TABLE `SearchBlogNews`
	        (
		        `IndexId` int NOT NULL AUTO_INCREMENT,
		        `BlogNewsPictureId` int NOT NULL,
                PRIMARY KEY (IndexId)
	        );
            
            IF `EntityTypeId` = 10 THEN
                SET @sql = concat(@sql , 'INSERT INTO `SearchBlogNews` (BlogNewsPictureId)  SELECT bnp.Id   FROM  `NS_BlogNewsPicture` bnp 						INNER JOIN BlogPost bp on bp.Id = bnp.EntityId WHERE  bnp.EntityTypeId = ' , `EntityTypeId` , ' AND (bp.StartDateUtc is 						null or bp.StartDateUtc <= UTC_DATE()) and (bp.EndDateUtc is null or bp.EndDateUtc >= UTC_DATE())');

        IF `ShowInStore` > -1 then
               SET @sql = concat(@sql , ' and bnp.ShowInStore = ' , `ShowInStore`);
        END IF;

        IF `LanguageId` > 0 then
                SET @sql = concat(@sql , ' and bp.LanguageId = ' , `LanguageId`);
        END IF;

# filter by store
        IF `StoreId` > 0 THEN
                SET @sql = concat(@sql , ' AND (bp.LimitedToStores = 0 OR EXISTS ( SELECT 1 FROM `StoreMapping` sm      WHERE 													sm.EntityId = bp.Id AND sm.EntityName = ''BlogPost'' and sm.StoreId=' , `StoreId` , ' ))');
        END IF;

        SET @sql = concat(@sql, ' order by bp.Id desc');
        ELSE
            SET @sql = concat(' INSERT INTO `SearchBlogNews` (BlogNewsPictureId) SELECT bnp.Id FROM `NS_BlogNewsPicture` bnp INNER JOIN News ni 									on ni.Id = bnp.EntityId WHERE ni.Published = 1 AND bnp.EntityTypeId = ' , `EntityTypeId` , ' AND (ni.StartDateUtc 								is null or ni.StartDateUtc <= UTC_DATE()) and (ni.EndDateUtc is null or ni.EndDateUtc >= UTC_DATE())');

        IF `ShowInStore` > -1 THEN
               SET @sql = concat(@sql , ' and bnp.ShowInStore = ' , `ShowInStore`);
        END IF;

        IF `Published` > -1 THEN
               SET @sql = concat(@sql , ' and ni.Published = ' , `Published`);
        END IF;
					
			     if `LanguageId` > -1 THEN
                        SET @sql = concat(@sql , ' and ni.LanguageId = ' , `LanguageId`);
        END IF;

        IF `StoreId` > 0 THEN
            SET @sql = concat(@sql , ' AND (ni.LimitedToStores = 0 OR EXISTS ( SELECT 1 FROM `StoreMapping` sm WHERE sm.EntityId = 										ni.Id AND sm.EntityName = ''News'' and sm.StoreId=' , `StoreId` , '))');
        END IF;
        SET @sql = concat(@sql, ' order by ni.Id desc');
        END IF;

        PREPARE sql_do_stmts FROM @sql;
        EXECUTE sql_do_stmts;
        DEALLOCATE PREPARE sql_do_stmts;
            
            select count(IndexId) from `SearchBlogNews` into `TotalRecords`;
            #paging
	        
	        SET @RowsToReturn = `PageSize` * (`PageIndex` + 1);
	        SET @PageLowerBound = `PageSize` * `PageIndex`;
	        SET @PageUpperBound = @PageLowerBound + `PageSize` + 1;

			SELECT 
            	bnp.* 
            FROM
		        `SearchBlogNews` as sbn
                INNER JOIN `NS_BlogNewsPicture` as bnp on bnp.Id = sbn.BlogNewsPictureId
            
            WHERE

                sbn.IndexId > @PageLowerBound AND

                sbn.IndexId<@PageUpperBound
            
            ORDER BY
	
                sbn.IndexId
            LIMIT `PageSize`;


            
	        DROP TABLE `SearchBlogNews`;
         
        END";

        private string _dropSpSql = @"DROP PROCEDURE [dbo].[BlogNewsPictureLoadPaged]";
        private string _dropSpMySql = @"DROP PROCEDURE BlogNewsPictureLoadPaged";

        #endregion


        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/BlogNews/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == AdminWidgetZones.BlogPostDetailsBlock)
                return "BlogPostPicture";
            if (widgetZone == AdminWidgetZones.NewsItemsDetailsBlock)
                return "NewsItemPicture";
            return "BlogNews";
        }

        public IList<string> GetWidgetZones()
        {
            var widgetZone = string.IsNullOrWhiteSpace(_blogNewsSettings.WidgetZone) ?
                PublicWidgetZones.HomepageBeforeNews : _blogNewsSettings.WidgetZone;

            return new List<string>
            {
                widgetZone,
                AdminWidgetZones.BlogPostDetailsBlock,
                AdminWidgetZones.NewsItemsDetailsBlock
            };
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (_permissionService.Authorize(BlogNewsPermissionProvider.ManageBlogNews))
            {
                var menuItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("NopStation.BlogNews.Menu.BlogNews"),
                    Visible = true,
                    IconClass = "fa-circle-o",
                };

                var categoryIconItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("NopStation.BlogNews.Menu.Configuration"),
                    Url = "/Admin/BlogNews/Configure",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "BlogNews.Configure"
                };
                menuItem.ChildNodes.Add(categoryIconItem);

                var documentation = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("NopStation.Common.Menu.Documentation"),
                    Url = "https://www.nop-station.com/blog-news-documentation?utm_source=admin-panel",
                    Visible = true,
                    IconClass = "fa-genderless",
                    OpenUrlInNewTab = true
                };
                menuItem.ChildNodes.Add(documentation);

                _nopStationCoreService.ManageSiteMap(rootNode, menuItem, NopStationMenuType.Plugin);
            }
        }

        public override void Install()
        {
            var settings = new BlogNewsSettings()
            {
                NumberOfBlogPostsToShow = 2,
                BlogPostPictureSize = 400,
                NewsItemPictureSize = 400,
                NumberOfNewsItemsToShow = 2,
                ShowBlogsInStore = true,
                ShowNewsInStore = true,
                WidgetZone = PublicWidgetZones.HomepageBeforeNews
            };
            _settingService.SaveSetting(settings);
            this.NopStationPluginInstall(new BlogNewsPermissionProvider());

            var dataSettings = DataSettingsManager.LoadSettings();

            if (dataSettings != null)
            {
                if (dataSettings.DataProvider == DataProviderType.SqlServer)
                {
                    _nopDataProvider.ExecuteNonQuery(_createSpSql);
                }
                else if (dataSettings.DataProvider == DataProviderType.MySql)
                {
                    _nopDataProvider.ExecuteNonQuery(_createSpMySql);
                }
                else
                {
                    _logger.Error("BlogNewsPlugin: BlogNewsPictureLoadPaged couldn't create. No data provider found.");
                }
            }

            base.Install();
        }

        public override void Uninstall()
        {
            this.NopStationPluginUninstall(new BlogNewsPermissionProvider());

            var dataSettings = DataSettingsManager.LoadSettings();

            if (dataSettings != null)
            {
                if (dataSettings.DataProvider == DataProviderType.SqlServer)
                {
                    _nopDataProvider.ExecuteNonQuery(_dropSpSql);
                }
                else if (dataSettings.DataProvider == DataProviderType.MySql)
                {
                    _nopDataProvider.ExecuteNonQuery(_dropSpMySql);
                }
                else
                {
                    _logger.Error("BlogNewsPlugin: BlogNewsPictureLoadPaged couldn't drop. No data provider found.");
                }
            }

            base.Uninstall();
        }

        #endregion


        #region Nop-station

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Menu.BlogNews", "Blogs and news"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Menu.Configuration", "Configuration"));

            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Tab.BlogPost.Picture", "Picture"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Tab.NewsItem.Picture", "Picture"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.BlogPost.Picture.SaveSuccess", "Blog post picture saved successfully."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.NewsItem.Picture.SaveSuccess", "News item picture saved successfully."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.BlogPost.Picture.SaveFailed", "Failed to save blog post picture."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.NewsItem.Picture.SaveFailed", "Failed to save news item picture."));

            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Blogs.Fields.Picture", "Picture"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Blogs.Fields.Picture.Hint", "Choose a picture to upload. If the picture size exceeds your stores max image size setting, it will be automatically resized."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Blogs.Fields.OverrideAltAttribute", "Alt"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Blogs.Fields.OverrideAltAttribute.Hint", "Override \"alt\" attribute for \"img\" HTML element. If empty, then a default rule will be used (e.g. blog title)."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Blogs.Fields.OverrideTitleAttribute", "Title"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Blogs.Fields.OverrideTitleAttribute.Hint", "Override \"title\" attribute for \"img\" HTML element. If empty, then a default rule will be used (e.g. blog title)."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Blogs.Fields.ShowInStore", "Show in store"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Blogs.Fields.ShowInStore.Hint", "Check to display this blog on your store. Recommended for your most popular blogs."));

            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.News.Fields.Picture", "Picture"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.News.Fields.Picture.Hint", "Choose a picture to upload. If the picture size exceeds your stores max image size setting, it will be automatically resized."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.News.Fields.OverrideAltAttribute", "Alt"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.News.Fields.OverrideAltAttribute.Hint", "Override \"alt\" attribute for \"img\" HTML element. If empty, then a default rule will be used (e.g. news title)."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.News.Fields.OverrideTitleAttribute", "Title"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.News.Fields.OverrideTitleAttribute.Hint", "Override \"title\" attribute for \"img\" HTML element. If empty, then a default rule will be used (e.g. news title)."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.News.Fields.ShowInStore", "Show in store"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.News.Fields.ShowInStore.Hint", "Check to display this blog on your store. Recommended for your most popular blogs."));

            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Button.SaveBlogPostPicture", "Save picture"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Button.SaveNewsItemPicture", "Save picture"));

            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.BlogPost.SaveBeforeEdit", "You need to save the blog post before you can upload picture for this page."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.NewsItem.SaveBeforeEdit", "You need to save the news item before you can upload picture for this page."));

            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration", "Blog News settings"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.ShowBlogsInStore", "Show blogs in store"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.ShowBlogsInStore.Hint", "Check to display blog posts in store page."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.BlogPostPictureSize", "Blog post picture size"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.BlogPostPictureSize.Hint", "Determines the value of blog post picture size display in store."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.NumberOfBlogPostsToShow", "Number of blog posts to show"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.NumberOfBlogPostsToShow.Hint", "Determines the number of blog posts to be displayed in store. Keep it 0 to show all blog posts."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.ShowNewsInStore", "Show news in store"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.ShowNewsInStore.Hint", "Check to display news items in store."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.NewsItemPictureSize", "News item picture size"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.NewsItemPictureSize.Hint", "Determines the value of news item picture size display in store."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.NumberOfNewsItemsToShow", "Number of news items to show"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.NumberOfNewsItemsToShow.Hint", "Determines the number of news items to be displayed in store. Keep it 0 to show all news items."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.BlogPosts.Hint", "Go to \"Content management > Blog posts > Edit\", add picture to the blog and check \"Show in store\""));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.NewsItems.Hint", "Go to \"Content management > News items > Edit\", add picture to the news and check \"Show in store\""));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.WidgetZone", "Widget zone"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.WidgetZone.Hint", "Specify the widget zone where the blog and news will be appeared. (i.e. home_page_before_news)"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Fields.WidgetZone.Required", "Widget zone is required."));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Configuration.Updated", "Blog News configuration updated successfully."));

            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Home.LatestBlog", "Latest Blog"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Home.LatestNews", "Latest News"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Home.LatestBlog.ReadMore", "Read More"));
            list.Add(new KeyValuePair<string, string>("NopStation.BlogNews.Home.LatestNews.ReadMore", "Read More"));

            return list;
        }

        public string SystemName => "NopStation.BlogNews";

        #endregion
    }
}
