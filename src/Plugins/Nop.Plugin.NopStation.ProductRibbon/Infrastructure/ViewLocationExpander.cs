using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.ProductRibbon.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                    $"/Plugins/NopStation.ProductRibbon/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.ProductRibbon/Areas/Admin/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] {
                    $"/Plugins/NopStation.ProductRibbon/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.ProductRibbon/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);

                if (context.Values.TryGetValue(THEME_KEY, out string theme))
                {
                    viewLocations = new[] {
                        $"/Plugins/NopStation.ProductRibbon/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/NopStation.ProductRibbon/Themes/{theme}/Views/Shared/{{0}}.cshtml"
                    }.Concat(viewLocations);
                }
            }

            return viewLocations;
        }
    }
}
