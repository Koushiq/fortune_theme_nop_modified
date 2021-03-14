using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.CategoryBanners.Infrastructure.Cache;
using Nop.Plugin.NopStation.CategoryBanners.Models;
using Nop.Plugin.NopStation.CategoryBanners.Services;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Services.Caching;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Plugin.NopStation.CategoryBanners.Components
{
    public class NopStationCategoryBannerViewComponent : NopViewComponent
    {
        private readonly INopStationLicenseService _licenseService;
        private readonly ICategoryBannerService _categoryBannerService;
        private readonly CategoryBannerSettings _categoryBannerSettings;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly INopStationContext _nopStationContext;
        private readonly ICacheKeyService _cacheKeyService;

        public NopStationCategoryBannerViewComponent(INopStationLicenseService licenseService,
            ICategoryBannerService categoryBannerService,
            CategoryBannerSettings categoryBannerSettings,
            IStaticCacheManager staticCacheManager,
            IPictureService pictureService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            INopStationContext nopStationContext,
            ICacheKeyService cacheKeyService
            )
        {
            _licenseService = licenseService;
            _categoryBannerService = categoryBannerService;
            _categoryBannerSettings = categoryBannerSettings;
            _cacheManager = staticCacheManager;
            _pictureService = pictureService;
            _workContext = workContext;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _localizationService = localizationService;
            _nopStationContext = nopStationContext;
            _cacheKeyService = cacheKeyService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_licenseService.IsLicensed())
                return Content("");

            if (_categoryBannerSettings.HideInPublicStore)
                return Content("");

            if (additionalData.GetType() != typeof(CategoryModel))
                return Content("");

            var cm = additionalData as CategoryModel;
            var banners = _categoryBannerService.GetCategoryBannersByCategoryId(cm.Id, _nopStationContext.MobileDevice);
            var model = new CategoryBannerModel()
            { 
                AutoPlay = _categoryBannerSettings.AutoPlay,
                AutoPlayHoverPause = _categoryBannerSettings.AutoPlayHoverPause,
                AutoPlayTimeout = _categoryBannerSettings.AutoPlayTimeout,
                Loop = _categoryBannerSettings.Loop,
                Nav = _categoryBannerSettings.Nav,
                Id = cm.Id
            };

            var pictureSize = _categoryBannerSettings.BannerPictureSize;

            foreach (var item in banners)
            {
                var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(new CacheKey(ModelCacheDefaults.CategoryBannerPictureModelKey, ModelCacheDefaults.CategoryBannerPicturePrefixCacheKey),
                                                item,
                                                pictureSize,
                                                true,
                                                _workContext.WorkingLanguage,
                                                _webHelper.IsCurrentConnectionSecured(),
                                                _storeContext.CurrentStore);

                var defaultPictureModel = _cacheManager.Get(cacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(item.PictureId);
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                        //"title" attribute
                        Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                            ? picture.TitleAttribute
                            : string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"),
                                cm.Name),
                        //"alt" attribute
                        AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                            ? picture.AltAttribute
                            : string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"),
                                cm.Name)
                    };

                    return pictureModel;
                });

                model.Banners.Add(defaultPictureModel);
            }

            return View(model);
        }
    }
}
