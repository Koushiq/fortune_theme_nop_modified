using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.QuickView.Models;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.QuickView.Components
{
    public class QuickViewFooterViewComponent : NopViewComponent
    {
        private readonly QuickViewSettings _quickViewSettings;
        private readonly INopStationLicenseService _licenseService;
        private readonly IPluginService _pluginService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IWidgetPluginManager _widgetPluginManager;

        public QuickViewFooterViewComponent(QuickViewSettings quickViewSettings,
            INopStationLicenseService licenseService,
            IPluginService pluginService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWidgetPluginManager widgetPluginManager)
        {
            _quickViewSettings = quickViewSettings;
            _licenseService = licenseService;
            _pluginService = pluginService;
            _storeContext = storeContext;
            _workContext = workContext;
            _widgetPluginManager = widgetPluginManager;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            var pluginDescriptor = _pluginService.GetPluginDescriptorBySystemName<IPlugin>("NopStation.PictureZoom",
                LoadPluginsMode.InstalledOnly, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

            var model = new PublicFooterModel()
            {
                PictureZoomEnabled = pluginDescriptor != null &&
                    _widgetPluginManager.IsPluginActive(pluginDescriptor.Instance<IWidgetPlugin>()) && 
                    _quickViewSettings.EnablePictureZoom
            };

            return View(model);
        }
    }
}
