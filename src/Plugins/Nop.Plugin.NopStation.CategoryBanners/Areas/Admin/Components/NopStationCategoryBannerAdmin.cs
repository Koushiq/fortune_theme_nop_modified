using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Models;
using Nop.Plugin.NopStation.CategoryBanners.Services;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.CategoryBanners.Areas.Admin.Components
{
    public class NopStationCategoryBannerAdminViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly ICategoryBannerService _categoryBannerService;

        public NopStationCategoryBannerAdminViewComponent(INopStationLicenseService licenseService,
            ICategoryBannerService categoryBannerService)
        {
            _licenseService = licenseService;
            _categoryBannerService = categoryBannerService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if(!_licenseService.IsLicensed())
                return Content("");

            if (additionalData.GetType() != typeof(CategoryModel))
                return Content("");

            var categoryModel = additionalData as CategoryModel;
            var searchModel = new CategoryBannerSearchModel()
            {
                CategoryId = categoryModel.Id
            };
            searchModel.SetGridPageSize();

            return View(searchModel);
        }
    }
}
