﻿using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.Theme.Fortune.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] { 
                    $"/Plugins/NopStation.Theme.Fortune/Areas/Admin/Views/Shared/{{0}}.cshtml",
                    $"/Plugins/NopStation.Theme.Fortune/Areas/Admin/Views/{{1}}/{{0}}.cshtml" 
                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] { 
                    $"/Plugins/NopStation.Theme.Fortune/Views/Shared/{{0}}.cshtml",
                    $"/Plugins/NopStation.Theme.Fortune/Views/{{1}}/{{0}}.cshtml" 
                }.Concat(viewLocations);
            }
            return viewLocations;
        }
    }
}
