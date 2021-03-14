using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.CategoryBanners.Infrastructure
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
                viewLocations = new string[] {
                    $"/Plugins/NopStation.CategoryBanners/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.CategoryBanners/Areas/Admin/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new string[] {
                    $"/Plugins/NopStation.CategoryBanners/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.CategoryBanners/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);

                if (context.Values.TryGetValue(THEME_KEY, out string theme))
                {
                    viewLocations = new string[] {
                        $"/Plugins/NopStation.CategoryBanners/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/NopStation.CategoryBanners/Themes/{theme}/Views/Shared/{{0}}.cshtml"
                    }.Concat(viewLocations);
                }
            }
            return viewLocations;
        }
    }
}
