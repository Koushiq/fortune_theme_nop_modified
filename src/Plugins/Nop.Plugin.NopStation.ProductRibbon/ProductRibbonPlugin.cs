using Nop.Core;
using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.Core;
using Nop.Services.Security;
using Nop.Plugin.NopStation.Core.Helpers;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.NopStation.ProductRibbon
{
    public class ProductRibbonPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        #region Fields

        public bool HideInWidgetList => false;

        private readonly IWebHelper _webHelper;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly ProductRibbonSettings _productRibbonSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ProductRibbonPlugin(IWebHelper webHelper,
            INopStationCoreService nopStationCoreService,
            ProductRibbonSettings productRibbonSettings,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _nopStationCoreService = nopStationCoreService;
            _productRibbonSettings = productRibbonSettings;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ProductRibbon/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if(widgetZone== PublicWidgetZones.HeadHtmlTag)
                return "ProductRibbonHeader";
            return "ProductRibbon";
        }

        public IList<string> GetWidgetZones()
        {
            var detailsPageWidgetZone = string.IsNullOrWhiteSpace(_productRibbonSettings.ProductDetailsPageWidgetZone) ?
                PublicWidgetZones.ProductDetailsBeforePictures : _productRibbonSettings.ProductDetailsPageWidgetZone;

            var overviewWidgetZone = string.IsNullOrWhiteSpace(_productRibbonSettings.ProductOverviewBoxWidgetZone) ?
                "product_picture_before" : _productRibbonSettings.ProductOverviewBoxWidgetZone;

            return new List<string> { detailsPageWidgetZone, overviewWidgetZone, PublicWidgetZones.HeadHtmlTag };
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Admin.NopStation.ProductRibbon.Menu.ProductRibbon"),
                Visible = true,
                IconClass = "fa-circle-o",
            };

            if (_permissionService.Authorize(ProductRibbonPermissionProvider.ManageProductRibbon))
            {
                var configItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.ProductRibbon.Menu.Configuration"),
                    Url = "/Admin/ProductRibbon/Configure",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "ProductRibbon.Configuration"
                };
                menuItem.ChildNodes.Add(configItem);
            }

            var documentation = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Admin.NopStation.Common.Menu.Documentation"),
                Url = "https://www.nop-station.com/product-ribbon-documentation",
                Visible = true,
                IconClass = "fa-genderless",
                OpenUrlInNewTab = true
            };
            menuItem.ChildNodes.Add(documentation);

            _nopStationCoreService.ManageSiteMap(rootNode, menuItem, NopStationMenuType.Plugin);
        }

        public override void Install()
        {
            var settings = new ProductRibbonSettings()
            {
                EnableBestSellerRibbon = true,
                EnableDiscountRibbon = true,
                EnableNewRibbon = true,
                ProductDetailsPageWidgetZone = PublicWidgetZones.ProductDetailsBeforePictures,
                ProductOverviewBoxWidgetZone = "product_picture_before"
            };
            _settingService.SaveSetting(settings);

            this.NopStationPluginInstall(new ProductRibbonPermissionProvider());
            base.Install();
        }

        public override void Uninstall()
        {
            this.NopStationPluginUninstall(new ProductRibbonPermissionProvider());
            base.Uninstall();
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("NopStation.ProductRibbon.RibbonText.New", "New"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductRibbon.RibbonText.Discount", "{0} Off"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductRibbon.RibbonText.BestSeller", "Best Seller"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Menu.ProductRibbon", "Product ribbon"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Menu.Configuration", "Configuration"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration", "Product ribbon settings"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableNewRibbon", "Enable 'New' ribbon"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableDiscountRibbon", "Enable 'Discount' ribbon"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableBestSellerRibbon", "Enable 'Best Seller' ribbon"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableNewRibbon.Hint", "Check to enable 'New' ribbon on product view (product overview box and details page)."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableDiscountRibbon.Hint", "Check to enable 'Discount' ribbon on product view (product overview box and details page)"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.EnableBestSellerRibbon.Hint", "Check to enable 'Best Seller' ribbon on product view (product overview box and details page)"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductDetailsPageWidgetZone.Required", "Product details page widget zone is required."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductOverviewBoxWidgetZone.Required", "Product overview box widget zone is required."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductDetailsPageWidgetZone", "Product details page widget zone"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductDetailsPageWidgetZone.Hint", "Specify the widget zone where the ribbon will be appeared in product details page. (i.e. productdetails_before_pictures)"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductOverviewBoxWidgetZone", "Product overview box widget zone"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Fields.ProductOverviewBoxWidgetZone.Hint", "Specify the widget zone where the ribbon will be appeared in product overview box. (i.e. productbox_addinfo_before)"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.ProductRibbon.Configuration.Updated", "Product ribbon settings updated successfully."));

            return list;
        }

        #endregion
    }
}
