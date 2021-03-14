using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.ProductRibbon.Factories;
using Nop.Services.Catalog;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System;

namespace Nop.Plugin.NopStation.ProductRibbon.Components
{
    public class ProductRibbonViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IProductRibbonModelFactory _productRibbonModelFactory;
        private readonly INopStationLicenseService _nopStationLicenseService;
        private readonly ProductRibbonSettings _productRibbonSettings;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public ProductRibbonViewComponent(IStoreContext storeContext,
            IProductRibbonModelFactory productRibbonModelFactory,
            INopStationLicenseService nopStationLicenseService,
            ProductRibbonSettings productRibbonSettings,
            IProductService prdouctService)
        {
            _storeContext = storeContext;
            _productRibbonModelFactory = productRibbonModelFactory;
            _nopStationLicenseService = nopStationLicenseService;
            _productRibbonSettings = productRibbonSettings;
            _productService = prdouctService;
        }

        #endregion

        #region Method

        public IViewComponentResult Invoke(string widgetZone, object additionalData = null)
        {
            if (!_nopStationLicenseService.IsLicensed())
                return Content("");

            var productId = 0;
            if (additionalData.GetType() == typeof(ProductDetailsModel))
            {
                var m = additionalData as ProductDetailsModel;
                productId = m.Id;
            }
            else if (additionalData.GetType() == typeof(ProductOverviewModel))
            {
                var m = additionalData as ProductOverviewModel;
                productId = m.Id;
            }
            else if (additionalData.GetType() == typeof(int))
            {
                productId = Convert.ToInt32(additionalData);
            }

            if (productId == 0)
                return Content("");

            var product = _productService.GetProductById(productId);
            var model = _productRibbonModelFactory.PrepareProductRibbonModel(product);

            if (model == null)
                return Content("");

            return View(model);
        }

        #endregion
    }
}
