using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Nop.Plugin.NopStation.BlogNews.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //throw new System.NotImplementedException();
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}