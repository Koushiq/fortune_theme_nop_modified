using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.ProductRibbon.Components
{
    public class ProductRibbonHeader : NopViewComponent
    {
        #region Fields

        private readonly INopStationLicenseService _nopStationLicenseService;

        #endregion

        #region Ctor

        public ProductRibbonHeader(INopStationLicenseService nopStationLicenseService)
        {
            _nopStationLicenseService = nopStationLicenseService;
        }

        #endregion

        #region Method

        public IViewComponentResult Invoke(string widgetZone, object additionalData = null)
        {
            if (!_nopStationLicenseService.IsLicensed())
                return Content("");
            return View();
        }

        #endregion
    }
}
