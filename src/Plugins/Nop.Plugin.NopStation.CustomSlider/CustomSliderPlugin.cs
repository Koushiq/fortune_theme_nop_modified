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
using Nop.Plugin.NopStation.CustomSlider.Helpers;
using Nop.Plugin.NopStation.CustomSlider.Data;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.NopStation.Core.Helpers;
using Nop.Plugin.NopStation.CustomSlider.Services;
using Nop.Plugin.NopStation.CustomSlider.Domains;
using System;
using Nop.Services.Media;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.NopStation.CustomSlider
{
    public class CustomSliderPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        #region Fields

        public bool HideInWidgetList => false;

        private readonly IWebHelper _webHelper;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ISliderService _sliderService;
        private readonly IPictureService _pictureService;
        private readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public CustomSliderPlugin(IWebHelper webHelper,
            INopStationCoreService nopStationCoreService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            ISliderService sliderService,
            IPictureService pictureService,
            INopFileProvider fileProvider)
        {
            _webHelper = webHelper;
            _nopStationCoreService = nopStationCoreService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _sliderService = sliderService;
            _pictureService = pictureService;
            _fileProvider = fileProvider;
        }

        #endregion

        #region Utilities

        protected void CreateSampleData()
        {
            var sliderSetting = new CustomSliderSettings()
            {
                EnableSlider = true
            };
            _settingService.SaveSetting(sliderSetting);

            var slider = new Slider()
            {
                Active = true,
                AutoPlay = true,
                AutoPlayTimeout = 3000,
                AutoPlayHoverPause = true,
                CreatedOnUtc = DateTime.UtcNow,
                Name = "Home page top",
                Loop = true,
                UpdatedOnUtc = DateTime.UtcNow,
                Nav = true,
                DisplayOrder = 0,
                StartPosition = 0,
                WidgetZoneId = 1,
            };
            _sliderService.InsertSlider(slider);

            var sampleImagesPath = _fileProvider.MapPath("~/Plugins/NopStation.CustomSlider/Contents/sample/");
            slider.SliderItems.Add(new SliderItem()
            {
                PictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "slider-1.jpg")), MimeTypes.ImageJpeg, "slider-1").Id,
                MobilePictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "slider-1-mobile.jpg")), MimeTypes.ImageJpeg, "slider-1").Id,
                Title = "Liquid for Chicken",
                ShortDescription = "The Best General Tso's Chicken"
            });
            _sliderService.UpdateSlider(slider);

            slider.SliderItems.Add(new SliderItem()
            {
                PictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "slider-2.jpg")), MimeTypes.ImageJpeg, "slider-2").Id,
                MobilePictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "slider-2-mobile.jpg")), MimeTypes.ImageJpeg, "slider-2").Id,
                Title = "Pressure Cooker",
                ShortDescription = "Ribollita Into a Weeknight Meal"
            });
            _sliderService.UpdateSlider(slider);

            slider.SliderItems.Add(new SliderItem()
            {
                PictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "slider-3.jpg")), MimeTypes.ImageJpeg, "slider-3").Id,
                MobilePictureId = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "slider-3-mobile.jpg")), MimeTypes.ImageJpeg, "slider-3").Id,
                Title = "Ingredients",
                ShortDescription = "The Best General Tso's Chicken"
            });
            _sliderService.UpdateSlider(slider);
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/CustomSlider/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.Footer)
                return "CustomSliderFooter";

            return "CustomSlider";
        }

        public IList<string> GetWidgetZones()
        {
            var widgetZones = SliderHelper.GetCustomWidgetZones();
            widgetZones.Add(PublicWidgetZones.Footer);

            return widgetZones;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Admin.NopStation.CustomSlider.Menu.CustomSlider"),
                Visible = true,
                IconClass = "fa-circle-o",
            };

            if (_permissionService.Authorize(CustomSliderPermissionProvider.ManageSliders))
            {
                var listItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.CustomSlider.Menu.List"),
                    Url = "/Admin/CustomSlider/List",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "CustomSlider"
                };
                menuItem.ChildNodes.Add(listItem);

                var configItem = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.CustomSlider.Menu.Configuration"),
                    Url = "/Admin/CustomSlider/Configure",
                    Visible = true,
                    IconClass = "fa-genderless",
                    SystemName = "CustomSlider.Configuration"
                };
                menuItem.ChildNodes.Add(configItem);
            }

            var documentation = new SiteMapNode()
            {
                Title = _localizationService.GetResource("Admin.NopStation.Common.Menu.Documentation"),
                Url = "https://www.nop-station.com/anywhere-slider-documentation",
                Visible = true,
                IconClass = "fa-genderless",
                OpenUrlInNewTab = true
            };
            menuItem.ChildNodes.Add(documentation);

            _nopStationCoreService.ManageSiteMap(rootNode, menuItem, NopStationMenuType.Plugin);
        }

        public override void Install()
        {
            CreateSampleData();
            this.NopStationPluginInstall(new CustomSliderPermissionProvider());
            base.Install();
        }

        public override void Uninstall()
        {
            this.NopStationPluginUninstall(new CustomSliderPermissionProvider());
            base.Uninstall();
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.List.SearchActive.Active", "Active"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.List.SearchActive.Inactive", "Inactive"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Menu.CustomSlider", "Anywhere slider"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Menu.Configuration", "Configuration"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Menu.List", "List"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Configuration", "Slider settings"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Tab.Info", "Info"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Tab.Properties", "Properties"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Tab.SliderItems", "Slider items"));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderList", "Sliders"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.EditDetails", "Edit slider details"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.BackToList", "back to slider list"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.AddNew", "Add new slider"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.SaveBeforeEdit", "You need to save the slider before you can add items for this slider page."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.AddNew", "Add new item"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Pictures.Alert.PictureAdd", "Failed to add product picture."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Configuration.Fields.EnableSlider", "Enable slider"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Configuration.Fields.EnableSlider.Hint", "Check to enable slider for your store."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Configuration.Updated", "Slider configuration updated successfully."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Created", "Slider has been created successfully."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Updated", "Slider has been updated successfully."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Deleted", "Slider has been deleted successfully."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.DisplayOrder", "Display order"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.Picture", "Picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.MobilePicture", "Mobile picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.ImageAltText", "Alt"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.Title", "Title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.ShortDescription", "Short description"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.Link", "Link"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.DisplayOrder.Hint", "The display order for this slider item. 1 represents the top of the list."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.Picture.Hint", "Picture of this slider item."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.MobilePicture.Hint", "Mobile view picture of this slider item."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.ImageAltText.Hint", "Override \"alt\" attribute for \"img\" HTML element."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.Title.Hint", "Override \"title\" attribute for \"img\" HTML element."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.Link.Hint", "Custom link for slider item picture."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.ShortDescription.Hint", "Short description for this slider item."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Name", "Name"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Name.Hint", "The slider name."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Title", "Title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Title.Hint", "The slider title."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.DisplayTitle", "Display title"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.DisplayTitle.Hint", "Determines whether title should be displayed on public site (depends on theme design)."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Active", "Active"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Active.Hint", "Determines whether this slider is active (visible on public store)."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.WidgetZone", "Widget zone"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.WidgetZone.Hint", "The widget zone where this slider will be displayed."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Picture", "Picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Picture.Hint", "The slider picture."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.CustomUrl", "Custom url"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.CustomUrl.Hint", "The slider custom url."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlay", "Auto play"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlay.Hint", "Check to enable auto play."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.CustomCssClass", "Custom css class"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.CustomCssClass.Hint", "Enter the custom CSS class to be applied."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.DisplayOrder", "Display order"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.DisplayOrder.Hint", "Display order of the slider. 1 represents the top of the list."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Loop", "Loop"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Loop.Hint", "Check to enable loop."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.StartPosition", "Start position"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.StartPosition.Hint", "Starting position."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.ShowBackgroundPicture", "Show background picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.ShowBackgroundPicture.Hint", "Check to show a background picture for this slider."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Center", "Center"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Center.Hint", "Check to center item. It works well with even and odd number of items."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Nav", "NAV"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Nav.Hint", "Check to enable next/prev buttons."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.LazyLoad", "Lazy load"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.LazyLoad.Hint", "Check to enable lazy load."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.LazyLoadEager", "Lazy load eager"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.LazyLoadEager.Hint", "Specify how many items you want to pre-load images to the right (and left when loop is enabled)."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlayTimeout", "Auto play timeout"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlayTimeout.Hint", "It's autoplay interval timeout. (e.g 5000)"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlayHoverPause", "Auto play hover pause"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AutoPlayHoverPause.Hint", "Check to enable pause on mouse hover."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AnimateOut", "Animate out"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AnimateOut.Hint", "Animate out."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AnimateIn", "Animate in"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.AnimateIn.Hint", "Animate in."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.CreatedOn", "Created on"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.CreatedOn.Hint", "The create date of this slider."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.UpdatedOn", "Updated on"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.UpdatedOn.Hint", "The last update date of this slider."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.SelectedStoreIds", "Limited to stores"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.SelectedStoreIds.Hint", "Option to limit this slider to a certain store. If you have multiple stores, choose one or several from the list. If you don't use this option just leave this field empty."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.Name.Required", "The name field is required."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.BackgroundPicture.Required", "The background picture is required."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.List.SearchWidgetZones", "Widget zones"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.List.SearchWidgetZones.Hint", "The search widget zones."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.List.SearchStore", "Store"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.List.SearchStore.Hint", "The search store."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.List.SearchActive", "Active"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.List.SearchActive.Hint", "The search active."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.EditDetails", "Edit details"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.Title.Required", "Title is required."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.Picture.Required", "Picture is required."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Fields.MobilePicture.Required", "Title is required."));

            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.SliderItems.Pictures.Alert.AddNew", "Upload picture first."));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.BackgroundPicture", "Background picture"));
            list.Add(new KeyValuePair<string, string>("Admin.NopStation.CustomSlider.Sliders.Fields.BackgroundPicture.Hint", "Background picture for this slider."));
            list.Add(new KeyValuePair<string, string>("NopStation.CustomSlider.Public.ShopNow", "Shop Now"));

            return list;
        }

        #endregion
    }
}
