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
using System.Linq;

namespace Nop.Plugin.NopStation.QuickView
{
    public class QuickViewPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        #region Fields

        public bool HideInWidgetList => false;

        private readonly IWebHelper _webHelper;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly NopStationCoreSettings _nopStationCoreSettings;

        #endregion

        #region Ctor

        public QuickViewPlugin(IWebHelper webHelper,
            INopStationCoreService nopStationCoreService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            NopStationCoreSettings nopStationCoreSettings)
        {
            _webHelper = webHelper;
            _nopStationCoreService = nopStationCoreService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _nopStationCoreSettings = nopStationCoreSettings;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/QuickView/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if(widgetZone== PublicWidgetZones.Footer)
                return "QuickViewFooter";

            return "QuickView";
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.Footer, "ProductBoxButtonAfter" };
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Admin.NopStation.QuickView.Menu.QuickView"),
                Visible = true,
                IconClass = "fa-circle-o",
            };

            if (_permissionService.Authorize(QuickViewPermissionProvider.ManageQuickView))
            {
                var configItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.QuickView.Menu.Configuration"),
                    Url = "/Admin/QuickView/Configure",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "QuickView.Configuration"
                };
                menuItem.ChildNodes.Add(configItem);
            }

            var documentation = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Admin.NopStation.Common.Menu.Documentation"),
                Url = "https://www.nop-station.com/quick-view-documentation",
                Visible = true,
                IconClass = "fa-genderless",
                OpenUrlInNewTab = true
            };
            menuItem.ChildNodes.Add(documentation);

            _nopStationCoreService.ManageSiteMap(rootNode, menuItem, NopStationMenuType.Plugin);
        }

        public override void Install()
        {
            var quickViewSettings = new QuickViewSettings()
            {
                ShowAlsoPurchasedProducts = true,
                ShowRelatedProducts = true,
                ShowAvailability = false,
                ShowAddToWishlistButton = true,
                ShowProductEmailAFriendButton = false,
                EnablePictureZoom = true,
                ShowCompareProductsButton = false,
                ShowDeliveryInfo = false,
                ShowFullDescription = false,
                ShowProductManufacturers = false,
                ShowProductReviewOverview = false,
                ShowProductSpecifications = false,
                ShowProductTags = false,
                ShowShortDescription = false
            };
            _settingService.SaveSetting(quickViewSettings);

            this.NopStationPluginInstall(new QuickViewPermissionProvider());
            base.Install();
        }

        public override void Uninstall()
        {
            this.NopStationPluginUninstall(new QuickViewPermissionProvider());
            base.Uninstall();
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("NopStation.QuickView.Button.QuickView", "Quick view"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Menu.QuickView", "Quick view"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Menu.Configuration", "Configuration"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration", "Quick view settings"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowAlsoPurchasedProducts", "Show also purchased products"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowAlsoPurchasedProducts.Hint", "Check to show \"Also purchased products\" on quick view page."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowRelatedProducts", "Show related products"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowRelatedProducts.Hint", "Check to show \"Related products\" on quick view page."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.EnablePictureZoom", "Enable picture zoom"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.EnablePictureZoom.Hint", "Check to enable picture zoom. Make sure Nop-Station picture zoom plugin is installed and activated for your store."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowShortDescription", "Show short description"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowShortDescription.Hint", "Check to show short description in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowFullDescription", "Show full description"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowFullDescription.Hint", "Check to show full description in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowAddToWishlistButton", "Show add to wishlist button"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowAddToWishlistButton.Hint", "Check to show 'Add To Wishlist' button in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowCompareProductsButton", "Show compare products button"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowCompareProductsButton.Hint", "Check to show 'Add to compare list' button in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductEmailAFriendButton", "Show producte mail a friend button"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductEmailAFriendButton.Hint", "Check to show 'Email a friend' button in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductReviewOverview", "Show product review overview"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductReviewOverview.Hint", "Check to show product review overview in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductManufacturers", "Show product manufacturers"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductManufacturers.Hint", "Check to show product manufacturers in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowAvailability", "Show availability"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowAvailability.Hint", "Check to show product availability in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowDeliveryInfo", "Show delivery info"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowDeliveryInfo.Hint", "Check to show product delivery information in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductSpecifications", "Show product specifications"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductSpecifications.Hint", "Check to show product specifications in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductTags", "Show product tags"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Fields.ShowProductTags.Hint", "Check to show product tags in quick view modal."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.QuickView.Configuration.Updated", "Quick view configuration updated successfully."));
            
            return list;
        }

        #endregion
    }
}
