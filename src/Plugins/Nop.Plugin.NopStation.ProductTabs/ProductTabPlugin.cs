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
using Nop.Plugin.NopStation.ProductTabs.Helpers;
using Nop.Plugin.NopStation.ProductTabs.Data;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.NopStation.Core.Helpers;
using System.Linq;

namespace Nop.Plugin.NopStation.ProductTabs
{
    public class ProductTabPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ProductTabSettings _sliderSettings;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly NopStationCoreSettings _nopStationCoreSettings;

        #endregion

        #region Ctor

        public ProductTabPlugin(IWebHelper webHelper,
            INopStationCoreService nopStationCoreService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ProductTabSettings sliderSettings,
            ISettingService settingService,
            IWorkContext workContext,
            NopStationCoreSettings nopStationCoreSettings)
        {
            _webHelper = webHelper;
            _nopStationCoreService = nopStationCoreService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _sliderSettings = sliderSettings;
            _settingService = settingService;
            _workContext = workContext;
            _nopStationCoreSettings = nopStationCoreSettings;
        }

        #endregion

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ProductTab/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.HeadHtmlTag)
                return "ProductTabHeadHtmlTag";

            return "ProductTab";
        }

        public IList<string> GetWidgetZones()
        {
            var widgetZones = ProductTabHelper.GetCustomWidgetZones();
            widgetZones.Add(PublicWidgetZones.HeadHtmlTag);

            return widgetZones;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (_permissionService.Authorize(ProductTabPermissionProvider.ManageProductTab))
            {
                var menuItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("NopStation.ProductTabs.Menu.ProductTab"),
                    Visible = true,
                    IconClass = "fa-circle-o",
                };

                var listItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("NopStation.ProductTabs.Menu.List"),
                    Url = "/Admin/ProductTab/List",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "ProductTabs"
                };
                menuItem.ChildNodes.Add(listItem);

                var configItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("NopStation.ProductTabs.Menu.Configuration"),
                    Url = "/Admin/ProductTab/Configure",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "ProductTabs.Configuration"
                };
                menuItem.ChildNodes.Add(configItem);

                _nopStationCoreService.ManageSiteMap(rootNode, menuItem, NopStationMenuType.Plugin);
            }
        }

        public override void Install()
        {
            _permissionService.InstallPermissions(new ProductTabPermissionProvider());

            //PluginResouces().Install(_localizationService);
            this.NopStationPluginInstall(new ProductTabPermissionProvider());
            _nopStationCoreSettings.ActiveNopStationSystemNames.Add("NopStation.ProductTabs");
            _settingService.SaveSetting(_nopStationCoreSettings);

            base.Install();
        }

        public override void Uninstall()
        {
            _permissionService.UninstallPermissions(new ProductTabPermissionProvider());

            //PluginResouces().Uninstall(_localizationService);
            this.NopStationPluginUninstall(new ProductTabPermissionProvider());
            if (_nopStationCoreSettings.ActiveNopStationSystemNames.Any(x => x == "NopStation.ProductTabs"))
                _nopStationCoreSettings.ActiveNopStationSystemNames.Remove("NopStation.ProductTabs");
            _settingService.SaveSetting(_nopStationCoreSettings);

            base.Uninstall();
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.Menu.ProductTab", "Product tab"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.Menu.List", "List"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.Menu.Configuration", "Configuration"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.List.SearchActive.Active", "Active"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.List.SearchActive.Inactive", "Inactive"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Name.Required", "The product tab name is required."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Title.Required", "The product tab title is required."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Picture.Required", "The product tab picture is required."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Fields.Name.Required", "The product tab item name is required."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Name", "Name"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Name.Hint", "The name of the product tab."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Title", "Title"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Title.Hint", "The title of the product tab."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.DisplayTitle", "Display title"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.DisplayTitle.Hint", "Check to display title."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Picture", "Picture"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Picture.Hint", "Select product tab picture."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.DisplayOrder", "Display Order"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.DisplayOrder.Hint", "Display order of the product tab. 1 represents the top of the list."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Active", "Active"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Active.Hint", "Determines whether product tab is active or not."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.CustomUrl", "Custom URL"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.CustomUrl.Hint", "The custom url."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.WidgetZone", "Widget zone"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.WidgetZone.Hint", "The widget-zone of the product tab."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.AutoPlay", "Auto play"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.AutoPlay.Hint", "Check to enable auto-play."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.CustomCssClass", "Custom CSS Class"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.CustomCssClass.Hint", "Enter the custom CSS class to be applied."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Loop", "Loop"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Loop.Hint", "Check to enable 'infinity loop' which duplicates last and first items to get loop illusion. (e.g false)"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Margin", "Margin"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Margin.Hint", "It's margin-right(px) on item. (Default 0)"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.StartPosition", "Starting position"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.StartPosition.Hint", "Starting position (e.g 0)"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Center", "Center"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Center.Hint", "Check to center item. It works well with even an odd number of items. (e.g false)"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Nav", "NAV"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Nav.Hint", "Check to enable next/prev buttons. (e.g false)"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.LazyLoad", "Lazy load"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.LazyLoad.Hint", "Check to enable lazy-load images (e.g false)"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.LazyLoadEager", "Lazy load eager"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.LazyLoadEager.Hint", "Check to eagerly pre-load images to the right (and left when loop is enabled) based on how many items you want to preload."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.AutoPlayTimeout", "Auto play timeout"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.AutoPlayTimeout.Hint", "It's autoplay interval timeout. (e.g 5000)"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.AutoPlayHoverPause", "Auto play hover pause"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.AutoPlayHoverPause.Hint", "Check to enable pause on mouse hover. (e.g false)"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.SelectedStoreIds", "Limited to stores"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.SelectedStoreIds.Hint", "Option to limit this product tab to a certain store. If you have multiple stores, choose one or several from the list. If you don't use this option just leave this field empty."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Deleted", "Product tab deleted successfully."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Updated", "Product tab updated successfully."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.Created", "Product tab created successfully."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Fields.Name", "Name"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Fields.Name.Hint", "Product tab item name."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Fields.DisplayOrder", "Display order"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Fields.DisplayOrder.Hint", "Display order of the product tab item. 1 represents the top of the list."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Updated", "Product tab item updated successfully"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Created", "Product tab item created successfully"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Tab.Info", "Info"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Tab.Properties", "Properties"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Tab.ProductTabItems", "Product tab items"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.ProductTabItems.BtnAddNew", "Add new tab item"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.ProductTabItems.SaveBeforeEdit", "You need to save the product tab before you can add item for this product tab page."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Tab.Info", "Info"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Tab.Products", "Products"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.ProductTabItemProducts.BtnAddNew", "Add new product"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.ProductTabItemProducts.SaveBeforeEdit", "You need to save the product tab item before you can add product for this product tab item page."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.Configuration", "Product tab settings"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.AddNew", "Add new product tab"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.EditDetails", "Edit product tab"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.BackToList", "back to tab list"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.AddNew", "Add new tab item"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.EditDetails", "Edit tab item"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItema.BackToProductTab", "back to product tab"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabList", "Product tabs"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItems.Products.AddNew", "Add new products"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.Configuration.Fields.EnableProductTab", "Enable product tab"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.Configuration.Fields.EnableProductTab.Hint", "Check to enable product tab."));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItemProducts.Fields.ProductTabItem", "Tab item"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItemProducts.Fields.Product", "Product"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabItemProducts.Fields.DisplayOrder", "Display order"));

            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.CreatedOn", "CreatedOn Utc"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.CreatedOn.Hint", "Created Date"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.UpdatedOn", "UpdatedOn Utc"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.ProductTabs.Fields.UpdatedOn.Hint", "Updated Date"));
            list.Add(new KeyValuePair<string, string>("NopStation.ProductTabs.Configuration.Updated", "Configuration Has been Updated"));


            return list;
        }

        public bool HideInWidgetList => false;
    }
}
