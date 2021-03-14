using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.NopStation.Core;
using Nop.Plugin.NopStation.Core.Helpers;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Infrastructure;
using Nop.Services.Media;
using Nop.Core.Infrastructure;
using System.Web;
using Nop.Plugin.NopStation.Theme.Fortune.Models;

namespace Nop.Plugin.NopStation.Theme.Fortune
{
    public class FortunePlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        #region Fields

        public bool HideInWidgetList => false;

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IPictureService _pictureService;
        private readonly INopFileProvider _fileProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly INopStationCoreService _nopStationCoreService;

        #endregion

        #region Ctor

        public FortunePlugin(ISettingService settingService, 
            IWebHelper webHelper,
            INopFileProvider nopFileProvider,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            INopStationCoreService nopStationCoreService)
        {
            _settingService = settingService;
            _webHelper = webHelper;
            _fileProvider = nopFileProvider;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _nopStationCoreService = nopStationCoreService;
        }

        #endregion

        #region Utilities

        private void CreateSampleData()
        {
            var sampleImagesPath = _fileProvider.MapPath("~/Plugins/NopStation.Theme.Fortune/Content/sample/");

            var settings = new FortuneSettings()
            {
                EnableImageLazyLoad = true,
                FooterEmail = "sales@nop-station.com",
                LazyLoadPictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "lazy-load.png")), MimeTypes.ImagePng, "lazy-load").Id,
                SupportedCardsPictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "footer-card-icons.png")), MimeTypes.ImagePng, "footer-cards").Id,
                FooterLogoPictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "footer-logo-white.png")), MimeTypes.ImagePng, "footer-logo").Id,
                ShowLogoAtPageFooter = true,
                ShowSupportedCardsPictureAtPageFooter = true,
                DescriptionBoxOneTitle = "Support 24/7",
                DescriptionBoxOneText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
                DescriptionBoxOnePictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "support-24-7.png")), MimeTypes.ImagePng, "support-24-7").Id,
                DescriptionBoxTwoTitle = "30 Day Return Policy",
                DescriptionBoxTwoText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
                DescriptionBoxTwoPictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "30-days-return-policy.png")), MimeTypes.ImagePng, "30-days-return-policy").Id,
                DescriptionBoxThreeTitle = "Worldwide Shipping",
                DescriptionBoxThreeText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
                DescriptionBoxThreePictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "worldwide-shpping.png")), MimeTypes.ImagePng, "worldwide-shpping").Id,
                DescriptionBoxFourTitle = "Free Delivery",
                DescriptionBoxFourText = "Lorem ipsum dolor sit amet, ei vix mucius nominavi, sea ut causae",
                DescriptionBoxFourPictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "free-delivery-icon.png")), MimeTypes.ImagePng, "free-delivery-icon").Id,
            };
            _settingService.SaveSetting(settings);
        }

        #endregion

        #region Methods

        public IList<string> GetWidgetZones()
        {
            return new List<string> { 
                PublicWidgetZones.HeadHtmlTag, 
                FortuneThemeDefaults.FooterDiscriptionWidgetZone, 
                FortuneThemeDefaults.CustomCssWidgetZone 
            };
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Fortune/Configure";
        }

        public override void Install()
        {
            CreateSampleData();
            this.NopStationPluginInstall(new FortunePermissionProvider());
            base.Install();
        }

        public override void Uninstall()
        {
            this.NopStationPluginUninstall(new FortunePermissionProvider());
            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (_permissionService.Authorize(FortunePermissionProvider.ManageFortune))
            {
                var menuItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.Theme.Fortune.Menu.Fortune"),
                    Visible = true,
                    IconClass = "fa-circle-o",
                };

                var configItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.Theme.Fortune.Menu.Configuration"),
                    Url = "/Admin/Fortune/Configure",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "Fortune.Configuration"
                };
                menuItem.ChildNodes.Add(configItem);

                var documentation = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.Common.Menu.Documentation"),
                    Url = "https://www.nop-station.com/fortune-theme-documentation?utm_source=admin-panel",
                    Visible = true,
                    IconClass = "fa-genderless",
                    OpenUrlInNewTab = true
                };
                menuItem.ChildNodes.Add(documentation);

                _nopStationCoreService.ManageSiteMap(rootNode, menuItem, NopStationMenuType.Theme);
            }
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if(widgetZone == FortuneThemeDefaults.CustomCssWidgetZone)
            {
                return "CustomCss";
            }
            else if(widgetZone == FortuneThemeDefaults.FooterDiscriptionWidgetZone)
            {
                return "FooterTopDescription";
            }
            return "Fortune";
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Menu.Fortune", "Fortune"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Menu.Configuration", "Configuration"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.CustomThemeColor", "Custom theme color"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.CustomThemeColor.Hint", "Choose a color that you want to site theme to display"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableImageLazyLoad", "Enable image lazy-load"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableImageLazyLoad.Hint", "Check to enable lazy-load for product box image."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.LazyLoadPictureId", "Lazy-load picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.LazyLoadPictureId.Hint", "This picture will be displayed initially in product box. Uploaded picture size should not be more than 4-5 KB."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.PanelTitle.DescriptionBoxOne", "Footer description box one"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.PanelTitle.DescriptionBoxTwo", "Footer description box two"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.PanelTitle.DescriptionBoxThree", "Footer description box three"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.PanelTitle.DescriptionBoxFour", "Footer description box four"));
            
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxOne", "Enable description box one"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxOne.Hint", "Check to show the description box"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTitle", "Description box one title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneTitle.Hint", "Title of the description box one"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOnePictureId", "Description box one picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOnePictureId.Hint", "Picture to display at the description box one"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneText", "Description box one text"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneText.Hint", "Text you want to display at the description box one"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneUrl", "Description box one url"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxOneUrl.Hint", "Url you want to redirect to when clicked on the description box"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxTwo", "Enable description box two"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxTwo.Hint", "Check to show the description box"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTitle", "Description box two title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoTitle.Hint", "Title of the description box two"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoPictureId", "Description box two picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoPictureId.Hint", "Picture to display at the description box two"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoText", "Description box two text"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoText.Hint", "Text you want to display at the description box two"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoUrl", "Description box two url"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxTwoUrl.Hint", "Url you want to redirect to when clicked on the description box"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxThree", "Enable description box three"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxThree.Hint", "Check to show the description box"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTitle", "Description box three title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeTitle.Hint", "Title of the description box three"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreePictureId", "Description box three picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreePictureId.Hint", "Picture to display at the description box three"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeText", "Description box three text"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeText.Hint", "Text you want to display at the description box three"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeUrl", "Description box three url"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxThreeUrl.Hint", "Url you want to redirect to when clicked on the description box"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxFour", "Enable description box four"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.EnableDescriptionBoxFour.Hint", "Check to show the description box"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTitle", "Description box four title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourTitle.Hint", "Title of the description box four"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourPictureId", "Description box four picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourPictureId.Hint", "Picture to display at the description box four"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourText", "Description box four text"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourText.Hint", "Text you want to display at the description box four"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourUrl", "Description box four url"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.DescriptionBoxFourUrl.Hint", "Url you want to redirect to when clicked on the description box"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.ShowSupportedCardsPictureAtPageFooter", "Show supported cards picture at page footer"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.ShowSupportedCardsPictureAtPageFooter.Hint", "Determines whether supported cards picture will be displayed at page footer or not."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.SupportedCardsPictureId", "Supported cards picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.SupportedCardsPictureId.Hint", "The single picture of supported cards (expected image height 30px)."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.ShowLogoAtPageFooter", "Show logo at page footer"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.ShowLogoAtPageFooter.Hint", "Determines whether logo will be displayed at page footer or not."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.FooterLogoPictureId", "Footer logo"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.FooterLogoPictureId.Hint", "This footer logo for this store (expected image height 40px)."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.CustomCss", "Custom Css"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.CustomCss.Hint", "Write custom CSS for your site. It will be rendered in head section of html page."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.FooterEmail", "Footer email"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Fields.FooterEmail.Hint", "Specify email which which will be displayed at page footer."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration", "Fortune settings"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.Updated", "Fortune configuration updated successfully."));

            list.Add(new KeyValuePair<string, string>("NopStation.Theme.Fortune.ProductDetailsPage.Overview", "Overview"));
            list.Add(new KeyValuePair<string, string>("NopStation.Theme.Fortune.ProductDetailsPage.Specifications", "Specifications"));
            list.Add(new KeyValuePair<string, string>("NopStation.Theme.Fortune.ProductDetailsPage.ProductTags", "Product Tags"));
            list.Add(new KeyValuePair<string, string>("NopStation.Theme.Fortune.ShoppingCart.Info", "Info"));
            list.Add(new KeyValuePair<string, string>("NopStation.Fontune.newsletter.description", "News Letter description"));


            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.TabTitle.GeneralSettings", "General Settings"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.Theme.Fortune.Configuration.TabTitle.FooterTopDescription", "Footer Top Description"));

            //missing resource string from Theme
            list.Add(new KeyValuePair<string, string>("products.tab.title.description", "Full Description"));
            list.Add(new KeyValuePair<string, string>("products.tab.title.tags", "Tags"));
            list.Add(new KeyValuePair<string, string>("products.tab.title.specifications", "Specifications"));
            list.Add(new KeyValuePair<string, string>("catalog.filters", "Filters"));
            list.Add(new KeyValuePair<string, string>("catalog.filtersattributes", "Filter Attributes"));
            list.Add(new KeyValuePair<string, string>("shoppingcart.mini.empty", "Your shopping bag is empty. Start shopping"));
            list.Add(new KeyValuePair<string, string>("ShoppingCart.Mini.Banner", "Mini Banner"));
            return list;
        }

        #endregion
    }
}