using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.QuickView.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("NopStation.QuickView.ProductDetails", "quickview_product_details",
                new { controller = "QuickView", action = "ProductDetails" });
        }

        public int Priority
        {
            get { return 10; }
        }
    }
}
 