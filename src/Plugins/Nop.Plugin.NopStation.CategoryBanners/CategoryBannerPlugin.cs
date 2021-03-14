using Nop.Core;
using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.Core;
using Nop.Services.Security;
using Nop.Plugin.NopStation.Core.Helpers;
using Nop.Plugin.NopStation.CategoryBanners.Data;

namespace Nop.Plugin.NopStation.CategoryBanners
{
    public class CategoryBannerPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        #region Fields

        public bool HideInWidgetList => false;

        private readonly IWebHelper _webHelper;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public CategoryBannerPlugin(IWebHelper webHelper,
            INopStationCoreService nopStationCoreService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _nopStationCoreService = nopStationCoreService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/CategoryBanner/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == AdminWidgetZones.CategoryDetailsBlock)
                return "NopStationCategoryBannerAdmin";
            return "NopStationCategoryBanner";
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                AdminWidgetZones.CategoryDetailsBlock,
                PublicWidgetZones.CategoryDetailsTop
            };
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Admin.NopStation.CategoryBanners.Menu.CategoryBanner"),
                Visible = true,
                IconClass = "fa-circle-o",
            };

            if (_permissionService.Authorize(CategoryBannerPermissionProvider.ManageCategoryBanner))
            {
                var categoryIconItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.CategoryBanners.Menu.Configuration"),
                    Url = "/Admin/CategoryBanner/Configure",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "CategoryBanner.Configure"
                };
                menuItem.ChildNodes.Add(categoryIconItem);
            }

            var documentation = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Admin.NopStation.Common.Menu.Documentation"),
                Url = "https://www.nop-station.com/category-banner-documentation",
                Visible = true,
                IconClass = "fa-genderless",
                OpenUrlInNewTab = true
            };
            menuItem.ChildNodes.Add(documentation);

            _nopStationCoreService.ManageSiteMap(rootNode, menuItem, NopStationMenuType.Plugin);
        }

        public override void Install()
        {
            var settings = new CategoryBannerSettings()
            {
                HideInPublicStore = false,
                AutoPlay = true,
                AutoPlayHoverPause = true,
                AutoPlayTimeout = 3000,
                BannerPictureSize = 800,
                Loop = true,
                Nav = true
            };
            _settingService.SaveSetting(settings);

            this.NopStationPluginInstall(new CategoryBannerPermissionProvider());
            base.Install();
        }

        public override void Uninstall()
        {
            this.NopStationPluginUninstall(new CategoryBannerPermissionProvider());
            base.Uninstall();
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Menu.CategoryBanner", "Category banner"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Menu.Configuration", "Configuration"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration", "Category banner settings"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Tab.Banners", "Banners"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.AddNew", "Add a new banner"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.AddButton", "Add category banner"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Alert.AddNew", "Upload picture first."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Alert.BannerAdd", "Failed to add product banner."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.SaveBeforeEdit", "You need to save the category before you can upload banner for this category page."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.DisplayOrder", "Display order"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.DisplayOrder.Hint", "Display order of the banner. 1 represents the top of the list."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.ForMobile", "For mobile"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.ForMobile.Hint", "Check to display banner for mobile device. To display on computer, keep it uncheked."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.OverrideAltAttribute", "Alt"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.OverrideAltAttribute.Hint", "Override \"alt\" attribute for \"img\" HTML element. If empty, then a default rule will be used (e.g. category name)."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.OverrideTitleAttribute", "Title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.OverrideTitleAttribute.Hint", "Override \"title\" attribute for \"img\" HTML element. If empty, then a default rule will be used (e.g. category name)."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.Picture", "Picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.CategoryBanners.Fields.Picture.Hint", "Choose a picture to upload. If the picture size exceeds your stores max image size setting, it will be automatically resized."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.HideInPublicStore", "Hide in public store"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.HideInPublicStore.Hint", "Check to hide category banner in public store."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.Loop", "Loop"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.Loop.Hint", "Check to enable loop. It will be applied for banner slider, when multiple banners uploaded for same category."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlay", "Auto play"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlay.Hint", "Check to enable auto play. It will be applied for banner slider, when multiple banners uploaded for same category."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.Nav", "NAV"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.Nav.Hint", "Check to enable next/prev buttons. It will be applied for banner slider, when multiple banners uploaded for same category."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.BannerPictureSize", "Banner picture size"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.BannerPictureSize.Hint", "Set banner picture size in pixel."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlayTimeout", "Auto play timeout"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlayTimeout.Hint", "It's autoplay interval timeout in miliseconds (e.g 5000). It will be applied for banner slider, when multiple banners uploaded for same category."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlayHoverPause", "Auto play hover pause"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CategoryBanners.Configuration.Fields.AutoPlayHoverPause.Hint", "Check to enable pause on mouse hover. It will be applied for banner slider, when multiple banners uploaded for same category."));

            return list;
        }
        
        #endregion
    }
}
