using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.QuickView.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";
        private const string PICTUREZOOM_KEY = "nopstation.quickview.picturezoom";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.ControllerName == "QuickView" && context.ViewName == "_ProductDetailsPictures")
            { 
                var pluginService = EngineContext.Current.Resolve<IPluginService>();
                var storeContext = EngineContext.Current.Resolve<IStoreContext>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var widgetPluginManager = EngineContext.Current.Resolve<IWidgetPluginManager>();
                var qvSettings = EngineContext.Current.Resolve<QuickViewSettings>();

                var pluginDescriptor = pluginService.GetPluginDescriptorBySystemName<IWidgetPlugin>("NopStation.PictureZoom",
                    LoadPluginsMode.InstalledOnly, workContext.CurrentCustomer, storeContext.CurrentStore.Id);
                
                if (pluginDescriptor != null && widgetPluginManager.IsPluginActive(pluginDescriptor.Instance<IWidgetPlugin>()) && qvSettings.EnablePictureZoom)
                    context.Values[PICTUREZOOM_KEY] = "true";
            }
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                    $"/Plugins/NopStation.QuickView/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.QuickView/Areas/Admin/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] {
                    $"/Plugins/NopStation.QuickView/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.QuickView/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);

                var displayZoomPictureView = DisplayZoomPictureView(context);
                if (displayZoomPictureView)
                {
                    viewLocations = new string[] {
                        $"/Plugins/NopStation.PictureZoom/Views/Shared/PictureZoom.cshtml"
                    }.Concat(viewLocations);
                }

                if (context.Values.TryGetValue(THEME_KEY, out string theme))
                {
                    viewLocations = new[] {
                        $"/Plugins/NopStation.QuickView/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/NopStation.QuickView/Themes/{theme}/Views/Shared/{{0}}.cshtml"
                    }.Concat(viewLocations);

                    if (displayZoomPictureView)
                    {
                        viewLocations = new string[] {
                            $"/Plugins/NopStation.PictureZoom/Themes/{theme}/Views/Shared/PictureZoom.cshtml"
                        }.Concat(viewLocations);
                    }
                }
            }

            return viewLocations;
        }

        private bool DisplayZoomPictureView(ViewLocationExpanderContext context)
        {
            if (context.Values.TryGetValue(PICTUREZOOM_KEY, out string val))
                return val == "true";

            return false;
        }
    }
}
